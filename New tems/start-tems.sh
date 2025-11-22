#!/bin/bash

# TEMS Startup Script
# This script starts all TEMS services in the correct order

set -e

echo "=========================================="
echo "TEMS Application Startup"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to check if a service is running on a port
check_port() {
    local port=$1
    lsof -i :$port >/dev/null 2>&1
}

# Function to wait for a URL to be available
wait_for_url() {
    local url=$1
    local service_name=$2
    local max_wait=120
    local waited=0
    
    echo -e "${YELLOW}Waiting for ${service_name} to be ready at ${url}...${NC}"
    
    while ! curl -sf "${url}" > /dev/null; do
        if [ $waited -ge $max_wait ]; then
            echo -e "${RED}Timeout waiting for ${service_name}${NC}"
            return 1
        fi
        echo -n "."
        sleep 2
        waited=$((waited + 2))
    done
    
    echo -e "\n${GREEN}${service_name} is ready!${NC}"
    return 0
}

# Step 1: Start Docker services (MongoDB and Keycloak)
echo -e "\n${BLUE}Step 1: Starting Docker services (MongoDB & Keycloak)...${NC}"
cd "/Users/babadorin/repos/tems/New tems/Backend/Tems"

if docker compose ps | grep -q "tems-mongodb.*Up"; then
    echo -e "${GREEN}MongoDB is already running${NC}"
else
    echo "Starting MongoDB..."
    docker compose up -d mongodb
    sleep 5
fi

if docker compose ps | grep -q "tems-keycloak.*Up"; then
    echo -e "${GREEN}Keycloak is already running${NC}"
else
    echo "Starting Keycloak..."
    docker compose up -d keycloak
fi

# Wait for MongoDB
wait_for_url "http://localhost:27017" "MongoDB" || true

# Wait for Keycloak
wait_for_url "http://localhost:8080/health/ready" "Keycloak"

# Step 2: Configure Keycloak (if needed)
echo -e "\n${BLUE}Step 2: Checking Keycloak configuration...${NC}"
cd "/Users/babadorin/repos/tems/New tems/Infrastructure/Keycloak"

if [ -f "configure-keycloak.sh" ]; then
    if [ ! -f ".configured" ]; then
        echo "Configuring Keycloak..."
        chmod +x configure-keycloak.sh
        ./configure-keycloak.sh
        touch .configured
    else
        echo -e "${GREEN}Keycloak is already configured${NC}"
    fi
else
    echo -e "${YELLOW}Keycloak configuration script not found. You may need to configure it manually.${NC}"
fi

# Step 3: Start Duende IdentityServer
echo -e "\n${BLUE}Step 3: Starting Duende IdentityServer...${NC}"
cd "/Users/babadorin/repos/tems/New tems/Backend/Tems/Tems.IdentityServer"

if check_port 5001; then
    echo -e "${YELLOW}Port 5001 is already in use. Stopping existing process...${NC}"
    pkill -f "Tems.IdentityServer" || true
    sleep 2
fi

echo "Starting IdentityServer on http://localhost:5001"
dotnet run --urls "http://localhost:5001" > /tmp/tems-identityserver.log 2>&1 &
IDENTITYSERVER_PID=$!
echo $IDENTITYSERVER_PID > /tmp/tems-identityserver.pid

# Wait for IdentityServer
wait_for_url "http://localhost:5001/.well-known/openid-configuration" "IdentityServer"

# Step 4: Start Backend API
echo -e "\n${BLUE}Step 4: Starting Backend API...${NC}"
cd "/Users/babadorin/repos/tems/New tems/Backend/Tems/Tems.Host"

if check_port 14721; then
    echo -e "${YELLOW}Port 14721 is already in use. Stopping existing process...${NC}"
    pkill -f "Tems.Host" || true
    sleep 2
fi

echo "Starting Backend API on http://localhost:14721"
dotnet run --urls "http://localhost:14721" > /tmp/tems-backend.log 2>&1 &
BACKEND_PID=$!
echo $BACKEND_PID > /tmp/tems-backend.pid

# Wait for Backend
sleep 5

# Step 5: Start Frontend
echo -e "\n${BLUE}Step 5: Starting Angular Frontend...${NC}"
cd "/Users/babadorin/repos/tems/New tems/Frontend/Tems"

if check_port 4200; then
    echo -e "${YELLOW}Port 4200 is already in use. Stopping existing process...${NC}"
    pkill -f "ng serve" || true
    sleep 2
fi

# Ensure correct Node version
export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"
nvm use 20.19.5 || true

echo "Starting Frontend on http://localhost:4200"
npm run start > /tmp/tems-frontend.log 2>&1 &
FRONTEND_PID=$!
echo $FRONTEND_PID > /tmp/tems-frontend.pid

echo -e "\n${GREEN}=========================================="
echo "TEMS Application Started Successfully!"
echo "==========================================${NC}"
echo ""
echo "Services:"
echo "  - MongoDB:         http://localhost:27017"
echo "  - Keycloak:        http://localhost:8080"
echo "  - Keycloak Admin:  http://localhost:8080/admin (admin/admin)"
echo "  - IdentityServer:  http://localhost:5001"
echo "  - Backend API:     http://localhost:14721"
echo "  - Frontend:        http://localhost:4200"
echo ""
echo "Process IDs:"
echo "  - IdentityServer: $IDENTITYSERVER_PID"
echo "  - Backend:        $BACKEND_PID"
echo "  - Frontend:       $FRONTEND_PID"
echo ""
echo "Logs:"
echo "  - IdentityServer: /tmp/tems-identityserver.log"
echo "  - Backend:        /tmp/tems-backend.log"
echo "  - Frontend:       /tmp/tems-frontend.log"
echo ""
echo "To stop all services, run: ./stop-tems.sh"
echo "=========================================="
