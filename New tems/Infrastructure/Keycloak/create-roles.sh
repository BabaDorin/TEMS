#!/bin/bash

# Script to create realm roles in Keycloak
# Usage: ./create-roles.sh

KEYCLOAK_URL="http://localhost:8080"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"
REALM="tems"

# Get admin token
echo "Getting admin access token..."
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

echo "Got admin token successfully"
echo ""

# Create realm roles for permissions
echo "Creating realm roles for permissions..."

for role in "can_view_entities" "can_manage_entities" "can_allocate_keys" "can_send_emails" "can_manage_announcements" "can_manage_system_configuration"; do
    echo "  Creating role: ${role}"
    
    HTTP_CODE=$(curl -s -w "%{http_code}" -o /dev/null -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/roles" \
        -H "Authorization: Bearer ${ADMIN_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"name\": \"${role}\",
            \"description\": \"Permission to ${role}\",
            \"composite\": false,
            \"clientRole\": false
        }")
    
    if [ "$HTTP_CODE" = "201" ]; then
        echo "  ✓ Created: ${role}"
    elif [ "$HTTP_CODE" = "409" ]; then
        echo "  ℹ Already exists: ${role}"
    else
        echo "  ❌ Failed to create ${role} (HTTP ${HTTP_CODE})"
    fi
done

echo ""
echo "=========================================="
echo "Realm roles created!"
echo "=========================================="
