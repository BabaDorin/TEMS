#!/bin/bash
KEYCLOAK_URL="http://localhost:8080"
REALM="tems"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"

echo "Getting admin token..."
ADMIN_TOKEN=$(curl -s -X POST "${KEYCLOAK_URL}/realms/master/protocol/openid-connect/token" \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "username=${ADMIN_USER}" \
    -d "password=${ADMIN_PASSWORD}" \
    -d "grant_type=password" \
    -d "client_id=admin-cli" | jq -r '.access_token')

if [ -z "$ADMIN_TOKEN" ] || [ "$ADMIN_TOKEN" = "null" ]; then
    echo "Failed to get admin token"
    exit 1
fi

echo "Getting client ID..."
CLIENT_UUID=$(curl -s "${KEYCLOAK_URL}/admin/realms/${REALM}/clients" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" | jq -r '.[] | select(.clientId=="tems-angular-spa") | .id')

if [ -z "$CLIENT_UUID" ]; then
    echo "Failed to find client"
    exit 1
fi

echo "Updating client redirect URIs to include /callback..."
curl -s -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}/clients/${CLIENT_UUID}" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d "{
        \"clientId\": \"tems-angular-spa\",
        \"name\": \"TEMS Angular SPA\",
        \"enabled\": true,
        \"publicClient\": true,
        \"redirectUris\": [
            \"http://localhost:4200/*\",
            \"http://localhost:4200/callback\",
            \"http://localhost:4200/home\",
            \"http://localhost:4200/silent-refresh.html\"
        ],
        \"webOrigins\": [\"http://localhost:4200\"],
        \"standardFlowEnabled\": true,
        \"implicitFlowEnabled\": false,
        \"directAccessGrantsEnabled\": false,
        \"attributes\": {
            \"pkce.code.challenge.method\": \"S256\"
        }
    }"

echo ""
echo "Client redirect URIs updated successfully!"
echo "Valid redirect URIs:"
echo "  - http://localhost:4200/callback"
echo "  - http://localhost:4200/home"
echo "  - http://localhost:4200/silent-refresh.html"
echo "  - http://localhost:4200/*"
