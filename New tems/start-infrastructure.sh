#!/bin/bash
# TEMS Infrastructure Startup Script
# Starts MongoDB, Keycloak, and Duende Identity Server

set -e

echo "🚀 Starting TEMS Infrastructure Services..."
echo ""

# Navigate to backend directory
cd "$(dirname "$0")/Backend/Tems"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "⚠️  Docker is not running. Starting Docker..."
    open -a Docker
    echo "⏳ Waiting for Docker to start (30 seconds)..."
    sleep 30
fi

# Stop any existing containers
echo "🛑 Stopping existing containers..."
docker compose down 2>/dev/null || true

# Remove old conflicting containers if they exist
echo "🧹 Cleaning up old containers..."
docker rm -f tems-keycloak tems-mongodb tems-sqlserver tems-app 2>/dev/null || true

# Start services
echo "📦 Building and starting infrastructure services..."
docker compose up -d --build

# Wait for services to be ready
echo "⏳ Waiting for services to be healthy..."
sleep 15

# Check service status
echo ""
echo "📊 Infrastructure Status:"
docker compose ps

# Test connectivity
echo ""
echo "🔍 Testing Service Connectivity:"
echo ""

# Test MongoDB
echo -n "MongoDB (port 27017): "
if docker exec tems-mongodb mongosh --eval "db.adminCommand('ping')" > /dev/null 2>&1; then
    echo "✅ HEALTHY"
else
    echo "❌ NOT READY"
fi

# Test Keycloak
echo -n "Keycloak (port 8080): "
if curl -s http://localhost:8080/health/ready > /dev/null 2>&1; then
    echo "✅ HEALTHY"
else
    echo "⏳ STARTING (may take 30-60 seconds)"
fi

# Test Identity Server
echo -n "Identity Server (port 5001): "
if curl -s http://localhost:5001/.well-known/openid-configuration > /dev/null 2>&1; then
    echo "✅ HEALTHY"
else
    echo "⏳ STARTING (may take 10-30 seconds)"
fi

echo ""
echo "✨ Infrastructure services are starting!"
echo ""
echo "📝 Service URLs:"
echo "   • MongoDB:         mongodb://localhost:27017"
echo "   • Keycloak Admin:  http://localhost:8080 (admin/admin)"
echo "   • Identity Server: http://localhost:5001"
echo ""
echo "💡 Next steps:"
echo "   1. Start Backend:  cd Backend/Tems/Tems.Host && dotnet run"
echo "   2. Start Frontend: cd Frontend/Tems && npm start"
echo ""
echo "📋 To view logs: docker compose logs -f [service-name]"
echo "🛑 To stop:      docker compose down"
