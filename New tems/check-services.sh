#!/bin/bash
echo "=== TEMS Services Status ==="
echo ""

# MongoDB
if curl -s http://localhost:27017 > /dev/null 2>&1; then
    echo "✅ MongoDB (27017): Running"
else
    echo "❌ MongoDB (27017): Not running"
fi

# Duende IdentityServer
if curl -s http://localhost:5001/.well-known/openid-configuration > /dev/null 2>&1; then
    echo "✅ Duende IdentityServer (5001): Running"
else
    echo "❌ Duende IdentityServer (5001): Not running"
fi

# Keycloak
if curl -s http://localhost:8080/health > /dev/null 2>&1; then
    echo "✅ Keycloak (8080): Running"
    # Check if realm exists
    if curl -s http://localhost:8080/realms/tems/.well-known/openid-configuration > /dev/null 2>&1; then
        echo "   ✅ TEMS Realm: Configured"
    else
        echo "   ❌ TEMS Realm: Not configured"
    fi
else
    echo "❌ Keycloak (8080): Not running"
fi

# Backend API
if curl -s http://localhost:5158 > /dev/null 2>&1; then
    echo "✅ Backend API (5158): Running"
else
    echo "❌ Backend API (5158): Not running"
fi

# Frontend
if lsof -ti:4200 > /dev/null 2>&1; then
    echo "✅ Frontend (4200): Running"
else
    echo "❌ Frontend (4200): Not running"
fi

echo ""
echo "=== Test Login ==="
echo "1. Open browser to: http://localhost:4200"
echo "2. Click 'Login with Duende IdentityServer'"
echo "3. Login with: admin / Admin123!"
echo "4. You should be redirected to /dashboard"
