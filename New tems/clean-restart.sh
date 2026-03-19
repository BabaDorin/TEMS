#!/bin/bash
# TEMS - Complete Docker Cleanup and Restart Script
# Kills all containers, cleans Docker cache, and rebuilds everything
#
# Usage:
#   ./clean-restart.sh           - Start infrastructure only (MongoDB, Keycloak, Identity Server)
#   ./clean-restart.sh --all     - Start everything including backend and frontend
#   ./clean-restart.sh --all-docker - Start everything in Docker containers (infra + backend + frontend)

set -e

# Parse command line arguments
START_ALL=false
START_ALL_DOCKER=false

for arg in "$@"; do
    if [ "$arg" = "--all" ]; then
        START_ALL=true
    fi
    if [ "$arg" = "--all-docker" ]; then
        START_ALL_DOCKER=true
    fi
done

if [ "$START_ALL" = true ] && [ "$START_ALL_DOCKER" = true ]; then
    echo "❌ Use either --all or --all-docker, not both."
    exit 1
fi

echo "🧹 TEMS Complete Docker Cleanup and Restart"
echo "=============================================="
echo ""

# Store the root directory path
ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"

# Navigate to backend directory
cd "${ROOT_DIR}/Backend/Tems"

# Compose file handling
COMPOSE_ARGS=(-f compose.yaml)
GENERATED_COMPOSE_FILE="compose.dev-all-docker.generated.yaml"

if [ "$START_ALL_DOCKER" = true ]; then
    cat > "${GENERATED_COMPOSE_FILE}" << 'EOF'
services:
  backend-api:
    image: mcr.microsoft.com/dotnet/sdk:9.0
    container_name: tems-backend-api
    working_dir: /src/Tems.Host
    command: >
      bash -lc "dotnet restore ../Tems.sln &&
      dotnet run --project Tems.Host.csproj --urls http://0.0.0.0:5158"
    volumes:
      - ./:/src
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://0.0.0.0:5158
    ports:
      - "5158:5158"
    depends_on:
      mongodb:
        condition: service_healthy
      keycloak:
        condition: service_started
      identity-server:
        condition: service_started
    networks:
      - tems-network

  frontend:
    image: node:20
    container_name: tems-frontend
    working_dir: /app
    command: >
      bash -lc "npm install &&
      npx ng serve --host 0.0.0.0 --port 4200"
    volumes:
      - ../../Frontend/Tems:/app
    ports:
      - "4200:4200"
    depends_on:
      - backend-api
    networks:
      - tems-network
EOF
    COMPOSE_ARGS+=(-f "${GENERATED_COMPOSE_FILE}")
fi

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "⚠️  Docker is not running. Starting Docker..."
    open -a Docker
    echo "⏳ Waiting for Docker to start (30 seconds)..."
    sleep 30
fi

echo "🛑 Step 1: Stopping all TEMS containers..."
docker compose "${COMPOSE_ARGS[@]}" down --remove-orphans 2>/dev/null || true

echo ""
echo "🗑️  Step 2: Removing TEMS containers..."
docker rm -f tems-keycloak tems-mongodb tems-identity-server tems-sqlserver tems-app tems-backend-api tems-frontend 2>/dev/null || true

echo ""
echo "🧽 Step 3: Removing TEMS images..."
docker images | grep tems | awk '{print $3}' | xargs docker rmi -f 2>/dev/null || true

echo ""
echo "💾 Step 4: Preserving database volumes (MongoDB data will be kept)..."
echo "   ℹ️  Volumes tems_mongodb_data and tems_identity-keys are preserved"

echo ""
echo "💨 Step 5: Pruning Docker system (cache, unused images, build cache)..."
docker system prune -af

echo ""
echo "🏗️  Step 6: Building fresh images (no cache)..."
docker compose "${COMPOSE_ARGS[@]}" build --no-cache

echo ""
echo "🚀 Step 7: Starting all services..."
docker compose "${COMPOSE_ARGS[@]}" up -d

echo ""
echo "⏳ Step 8: Waiting for services to be healthy (30 seconds)..."
sleep 30

echo ""
echo "📊 Infrastructure Status:"
docker compose "${COMPOSE_ARGS[@]}" ps

echo ""
echo "🔍 Testing Service Connectivity:"
echo ""

# Test MongoDB
echo -n "MongoDB (port 27017): "
if docker exec tems-mongodb mongosh --eval "db.adminCommand('ping')" > /dev/null 2>&1; then
    echo "✅ HEALTHY"
else
    echo "❌ NOT READY - Check: docker logs tems-mongodb"
fi

# Test Keycloak
echo -n "Keycloak (port 8080): "
if curl -s http://localhost:8080/health/ready > /dev/null 2>&1; then
    echo "✅ HEALTHY"
else
    echo "⏳ STARTING (may take 30-60 seconds) - Check: docker logs tems-keycloak"
fi

# Test Identity Server
echo -n "Identity Server (port 5001): "
if curl -s http://localhost:5001/.well-known/openid-configuration > /dev/null 2>&1; then
    echo "✅ HEALTHY"
else
    echo "⏳ STARTING (may take 10-30 seconds) - Check: docker logs tems-identity-server"
fi

if [ "$START_ALL_DOCKER" = true ]; then
    echo -n "Backend API (port 5158): "
    if curl -s http://localhost:5158/swagger/index.html > /dev/null 2>&1; then
        echo "✅ HEALTHY"
    else
        echo "⏳ STARTING - Check: docker logs tems-backend-api"
    fi

    echo -n "Frontend (port 4200): "
    if curl -s http://localhost:4200 > /dev/null 2>&1; then
        echo "✅ HEALTHY"
    else
        echo "⏳ STARTING - Check: docker logs tems-frontend"
    fi
fi

echo ""
echo "⚙️  Step 9: Configuring Keycloak..."
echo ""

# Navigate to Keycloak scripts and run configuration
cd "${ROOT_DIR}/Infrastructure/Keycloak"
if [ -f "./configure-keycloak.sh" ]; then
    ./configure-keycloak.sh
    echo ""
    echo "✅ Keycloak configuration complete!"
else
    echo "⚠️  Keycloak configuration script not found at Infrastructure/Keycloak/configure-keycloak.sh"
fi

# Navigate back to root
cd "${ROOT_DIR}"

echo ""
echo "✨ Clean restart complete!"
echo ""
echo "📝 Service URLs:"
echo "   • MongoDB:         mongodb://localhost:27017"
echo "   • Keycloak Admin:  http://localhost:8080 (admin/admin)"
echo "   • Identity Server: http://localhost:5001"

# Start backend and frontend if --all flag is provided
if [ "$START_ALL" = true ]; then
    echo ""
    echo "🚀 Starting Backend and Frontend..."
    echo ""
    
    # Start backend in a new terminal
    echo "   • Starting Backend API (port 5158)..."
    osascript -e "tell application \"Terminal\" to do script \"cd '${ROOT_DIR}/Backend/Tems/Tems.Host' && dotnet run\""
    
    # Wait a bit for backend to start
    sleep 3
    
    # Start frontend in a new terminal
    echo "   • Starting Frontend (port 4200)..."
    osascript -e "tell application \"Terminal\" to do script \"cd '${ROOT_DIR}/Frontend/Tems' && ng serve\""
    
    echo ""
    echo "📝 Application URLs:"
    echo "   • Backend API:     http://localhost:5158"
    echo "   • Frontend:        http://localhost:4200"
    echo ""
    echo "✅ All services started!"
elif [ "$START_ALL_DOCKER" = true ]; then
    echo ""
    echo "📝 Application URLs:"
    echo "   • Backend API:     http://localhost:5158"
    echo "   • Frontend:        http://localhost:4200"
    echo ""
    echo "✅ All services started in Docker!"
else
    echo ""
    echo "💡 Next steps:"
    echo "   1. Start Backend:      cd Backend/Tems/Tems.Host && dotnet run"
    echo "   2. Start Frontend:     cd Frontend/Tems && npm start"
    echo ""
    echo "   OR run: ./clean-restart.sh --all  (to start everything automatically)"
fi

echo ""
echo "📋 Useful commands:"
echo "   • View logs:        docker compose ${COMPOSE_ARGS[*]} logs -f [service-name]"
echo "   • Check status:     ./check-services.sh"
echo "   • Stop all:         docker compose ${COMPOSE_ARGS[*]} down"
echo "   • Clean restart:    ./clean-restart.sh [--all]"
echo "   • Docker all mode:  ./clean-restart.sh --all-docker"

# Cleanup generated compose file (if any)
if [ -f "${GENERATED_COMPOSE_FILE}" ]; then
    rm -f "${GENERATED_COMPOSE_FILE}"
fi
