#!/bin/bash

# TEMS Stop Script
# This script stops all TEMS services

set -e

echo "=========================================="
echo "Stopping TEMS Application"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Stop Frontend
if [ -f /tmp/tems-frontend.pid ]; then
    PID=$(cat /tmp/tems-frontend.pid)
    echo -e "${YELLOW}Stopping Frontend (PID: $PID)...${NC}"
    kill $PID 2>/dev/null || true
    pkill -f "ng serve" || true
    rm /tmp/tems-frontend.pid
else
    echo "Stopping any running ng serve processes..."
    pkill -f "ng serve" || true
fi

# Stop Backend
if [ -f /tmp/tems-backend.pid ]; then
    PID=$(cat /tmp/tems-backend.pid)
    echo -e "${YELLOW}Stopping Backend API (PID: $PID)...${NC}"
    kill $PID 2>/dev/null || true
    pkill -f "Tems.Host" || true
    rm /tmp/tems-backend.pid
else
    echo "Stopping any running Tems.Host processes..."
    pkill -f "Tems.Host" || true
fi

# Stop IdentityServer
if [ -f /tmp/tems-identityserver.pid ]; then
    PID=$(cat /tmp/tems-identityserver.pid)
    echo -e "${YELLOW}Stopping IdentityServer (PID: $PID)...${NC}"
    kill $PID 2>/dev/null || true
    pkill -f "Tems.IdentityServer" || true
    rm /tmp/tems-identityserver.pid
else
    echo "Stopping any running Tems.IdentityServer processes..."
    pkill -f "Tems.IdentityServer" || true
fi

# Stop Docker services
echo -e "${YELLOW}Stopping Docker services...${NC}"
cd "/Users/babadorin/repos/tems/New tems/Backend/Tems"
docker compose down

echo -e "\n${GREEN}=========================================="
echo "TEMS Application Stopped"
echo "==========================================${NC}"
