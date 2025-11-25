#!/bin/bash

# TEMS Installation Verification Script

echo "=========================================="
echo "TEMS Installation Verification"
echo "=========================================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

CHECKS_PASSED=0
CHECKS_FAILED=0

# Function to check file existence
check_file() {
    local file=$1
    local description=$2
    
    if [ -f "$file" ]; then
        echo -e "${GREEN}✓${NC} $description"
        ((CHECKS_PASSED++))
    else
        echo -e "${RED}✗${NC} $description (missing: $file)"
        ((CHECKS_FAILED++))
    fi
}

# Function to check directory existence
check_dir() {
    local dir=$1
    local description=$2
    
    if [ -d "$dir" ]; then
        echo -e "${GREEN}✓${NC} $description"
        ((CHECKS_PASSED++))
    else
        echo -e "${RED}✗${NC} $description (missing: $dir)"
        ((CHECKS_FAILED++))
    fi
}

# Function to check executable
check_executable() {
    local file=$1
    local description=$2
    
    if [ -x "$file" ]; then
        echo -e "${GREEN}✓${NC} $description"
        ((CHECKS_PASSED++))
    else
        echo -e "${RED}✗${NC} $description (not executable: $file)"
        ((CHECKS_FAILED++))
    fi
}

echo "Checking Core Files..."
check_file "README.md" "Main README"
check_file "QUICKSTART.md" "Quick Start Guide"
check_file "AUTH_SETUP.md" "Authentication Documentation"
check_file "IMPLEMENTATION_SUMMARY.md" "Implementation Summary"
check_executable "start-tems.sh" "Start Script"
check_executable "stop-tems.sh" "Stop Script"
echo ""

echo "Checking Backend Structure..."
check_dir "Backend/Tems" "Backend Directory"
check_file "Backend/Tems/Tems.Host/Program.cs" "Backend API Program"
check_file "Backend/Tems/Tems.Host/Tems.Host.csproj" "Backend API Project"
check_file "Backend/Tems/Tems.IdentityServer/Program.cs" "IdentityServer Program"
check_file "Backend/Tems/Tems.IdentityServer/Tems.IdentityServer.csproj" "IdentityServer Project"
check_file "Backend/Tems/Tems.IdentityServer/Controllers/AccountController.cs" "Registration Controller"
check_file "Backend/Tems/Tems.IdentityServer/Config/IdentityConfig.cs" "Identity Configuration"
check_file "Backend/Tems/compose.yaml" "Docker Compose File"
echo ""

echo "Checking Frontend Structure..."
check_dir "Frontend/Tems" "Frontend Directory"
check_file "Frontend/Tems/src/app/app.config.ts" "App Configuration"
check_file "Frontend/Tems/src/app/app-routing.module.ts" "Routing Configuration"
check_file "Frontend/Tems/src/app/services/auth.service.ts" "Auth Service"
check_file "Frontend/Tems/src/app/auth.interceptor.ts" "Auth Interceptor"
check_file "Frontend/Tems/src/app/public/user-pages/login/login.component.ts" "Login Component"
check_file "Frontend/Tems/src/app/public/user-pages/register/register.component.ts" "Register Component"
check_file "Frontend/Tems/src/environments/environment.ts" "Environment Config"
check_file "Frontend/Tems/src/silent-refresh.html" "Silent Refresh Page"
check_file "Frontend/Tems/src/tailwind.css" "Tailwind CSS"
check_file "Frontend/Tems/tailwind.config.js" "Tailwind Config"
check_file "Frontend/Tems/postcss.config.js" "PostCSS Config"
echo ""

echo "Checking Infrastructure..."
check_dir "Infrastructure/Keycloak" "Keycloak Infrastructure"
check_file "Infrastructure/Keycloak/Pulumi.yaml" "Pulumi Config"
check_file "Infrastructure/Keycloak/package.json" "Pulumi Package"
check_file "Infrastructure/Keycloak/index.ts" "Pulumi Infrastructure Code"
check_executable "Infrastructure/Keycloak/configure-keycloak.sh" "Keycloak Configuration Script"
echo ""

echo "Checking Prerequisites..."

# Check Docker
if command -v docker &> /dev/null; then
    echo -e "${GREEN}✓${NC} Docker installed"
    ((CHECKS_PASSED++))
else
    echo -e "${RED}✗${NC} Docker not found"
    ((CHECKS_FAILED++))
fi

# Check Docker Compose
if command -v docker compose &> /dev/null || command -v docker-compose &> /dev/null; then
    echo -e "${GREEN}✓${NC} Docker Compose installed"
    ((CHECKS_PASSED++))
else
    echo -e "${RED}✗${NC} Docker Compose not found"
    ((CHECKS_FAILED++))
fi

# Check .NET
if command -v dotnet &> /dev/null; then
    DOTNET_VERSION=$(dotnet --version)
    echo -e "${GREEN}✓${NC} .NET SDK installed (version: $DOTNET_VERSION)"
    ((CHECKS_PASSED++))
else
    echo -e "${RED}✗${NC} .NET SDK not found"
    ((CHECKS_FAILED++))
fi

# Check Node.js
if command -v node &> /dev/null; then
    NODE_VERSION=$(node --version)
    echo -e "${GREEN}✓${NC} Node.js installed (version: $NODE_VERSION)"
    ((CHECKS_PASSED++))
    
    # Check if it's the right version
    if [[ $NODE_VERSION == v20* ]]; then
        echo -e "${GREEN}✓${NC} Node.js version is compatible"
        ((CHECKS_PASSED++))
    else
        echo -e "${YELLOW}⚠${NC} Node.js version should be v20.x (current: $NODE_VERSION)"
        echo "  Run: nvm use 20.19.5"
    fi
else
    echo -e "${RED}✗${NC} Node.js not found"
    ((CHECKS_FAILED++))
fi

# Check npm
if command -v npm &> /dev/null; then
    NPM_VERSION=$(npm --version)
    echo -e "${GREEN}✓${NC} npm installed (version: $NPM_VERSION)"
    ((CHECKS_PASSED++))
else
    echo -e "${RED}✗${NC} npm not found"
    ((CHECKS_FAILED++))
fi

echo ""
echo "=========================================="
echo "Verification Results"
echo "=========================================="
echo -e "Checks Passed: ${GREEN}$CHECKS_PASSED${NC}"
echo -e "Checks Failed: ${RED}$CHECKS_FAILED${NC}"
echo ""

if [ $CHECKS_FAILED -eq 0 ]; then
    echo -e "${GREEN}✓ All checks passed!${NC}"
    echo ""
    echo "You're ready to start TEMS!"
    echo "Run: ./start-tems.sh"
    exit 0
else
    echo -e "${RED}✗ Some checks failed.${NC}"
    echo ""
    echo "Please fix the issues above before starting TEMS."
    exit 1
fi
