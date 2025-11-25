#!/bin/bash

# Script to remove roles from users in Keycloak
# Usage: ./remove-roles.sh <username> <role1> <role2> ...
# Example: ./remove-roles.sh "dorin.baba@gmail.com" can_manage_entities

KEYCLOAK_URL="http://localhost:8080"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"
REALM="tems"

if [ "$#" -lt 2 ]; then
    echo "Usage: $0 <username> <role1> [role2] [role3] ..."
    echo ""
    echo "Available roles:"
    echo "  - can_view_entities"
    echo "  - can_manage_entities"
    echo "  - can_allocate_keys"
    echo "  - can_send_emails"
    echo "  - can_manage_announcements"
    echo "  - can_manage_system_configuration"
    echo ""
    echo "Example: $0 'user@example.com' can_manage_entities"
    exit 1
fi

USERNAME="$1"
shift
ROLES=("$@")

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

# Get user ID
echo "Looking up user: ${USERNAME}..."
USER_ID=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/users?username=${USERNAME}" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" | jq -r '.[0].id')

if [ -z "$USER_ID" ] || [ "$USER_ID" = "null" ]; then
    echo "User not found: ${USERNAME}"
    exit 1
fi

echo "Found user with ID: ${USER_ID}"
echo ""

# Remove each role
echo "Removing roles..."
for role in "${ROLES[@]}"; do
    echo "  Removing role: ${role}"
    
    # Get role data
    ROLE_DATA=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/roles/${role}" \
        -H "Authorization: Bearer ${ADMIN_TOKEN}")
    
    ROLE_ID=$(echo "$ROLE_DATA" | jq -r '.id')
    
    if [ -z "$ROLE_ID" ] || [ "$ROLE_ID" = "null" ]; then
        echo "  ❌ Role not found: ${role}"
        continue
    fi
    
    # Remove role from user
    HTTP_CODE=$(curl -s -w "%{http_code}" -o /dev/null -X DELETE "${KEYCLOAK_URL}/admin/realms/${REALM}/users/${USER_ID}/role-mappings/realm" \
        -H "Authorization: Bearer ${ADMIN_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "[${ROLE_DATA}]")
    
    if [ "$HTTP_CODE" = "204" ]; then
        echo "  ✓ Removed: ${role}"
    else
        echo "  ❌ Failed to remove ${role} (HTTP ${HTTP_CODE})"
    fi
done

echo ""
echo "=========================================="
echo "Roles removed successfully!"
echo "=========================================="
echo "Username: ${USERNAME}"
echo "Removed roles: ${ROLES[*]}"
echo ""
echo "Note: User needs to log out and log back in for changes to take effect"
