#!/bin/bash

# Keycloak configuration
KEYCLOAK_URL="http://localhost:8080"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"
REALM="tems"

# Get admin token
echo "Getting admin token..."
ADMIN_TOKEN=$(curl -s -X POST "${KEYCLOAK_URL}/realms/master/protocol/openid-connect/token" \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "username=${ADMIN_USER}" \
    -d "password=${ADMIN_PASSWORD}" \
    -d "grant_type=password" \
    -d "client_id=admin-cli" \
    | jq -r '.access_token')

if [ -z "$ADMIN_TOKEN" ] || [ "$ADMIN_TOKEN" = "null" ]; then
    echo "Failed to get admin token"
    exit 1
fi

echo "Admin token retrieved"

# Get the client ID (internal UUID)
echo "Getting client internal ID..."
CLIENT_UUID=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/clients?clientId=tems-angular-spa" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    | jq -r '.[0].id')

if [ -z "$CLIENT_UUID" ] || [ "$CLIENT_UUID" = "null" ]; then
    echo "Failed to get client UUID"
    exit 1
fi

echo "Client UUID: $CLIENT_UUID"

# Update the client redirect URIs
echo "Updating redirect URIs..."
curl -s -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}/clients/${CLIENT_UUID}" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "redirectUris": [
            "http://localhost:4200/*",
            "http://localhost:4200/home"
        ],
        "webOrigins": [
            "http://localhost:4200"
        ]
    }'

echo "Redirect URIs updated successfully!"
echo "New redirect URIs:"
echo "  - http://localhost:4200/*"
echo "  - http://localhost:4200/home"
