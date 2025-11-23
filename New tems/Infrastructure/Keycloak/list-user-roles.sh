#!/bin/bash

# Script to list roles assigned to a user in Keycloak
# Usage: ./list-user-roles.sh <username>
# Example: ./list-user-roles.sh "dorin.baba@gmail.com"

KEYCLOAK_URL="http://localhost:8080"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"
REALM="tems"

if [ "$#" -ne 1 ]; then
    echo "Usage: $0 <username>"
    echo "Example: $0 'dorin.baba@gmail.com'"
    exit 1
fi

USERNAME="$1"

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

# Get user data
echo "Looking up user: ${USERNAME}..."
USER_DATA=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/users?username=${USERNAME}" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}")

USER_ID=$(echo "$USER_DATA" | jq -r '.[0].id')

if [ -z "$USER_ID" ] || [ "$USER_ID" = "null" ]; then
    echo "User not found: ${USERNAME}"
    exit 1
fi

echo "Found user with ID: ${USER_ID}"
echo ""

# Extract user info
EMAIL=$(echo "$USER_DATA" | jq -r '.[0].email')
FIRST_NAME=$(echo "$USER_DATA" | jq -r '.[0].firstName')
LAST_NAME=$(echo "$USER_DATA" | jq -r '.[0].lastName')
ENABLED=$(echo "$USER_DATA" | jq -r '.[0].enabled')

# Get user's realm roles
ROLES=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/users/${USER_ID}/role-mappings/realm" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}")

echo "=========================================="
echo "User Details"
echo "=========================================="
echo "Username: ${USERNAME}"
echo "Email: ${EMAIL}"
echo "Name: ${FIRST_NAME} ${LAST_NAME}"
echo "Enabled: ${ENABLED}"
echo ""
echo "Assigned Realm Roles:"
echo "=========================================="

# Filter only our permission roles
echo "$ROLES" | jq -r '.[] | select(.name | startswith("can_")) | "  ✓ " + .name'

# Check if user has no permission roles
ROLE_COUNT=$(echo "$ROLES" | jq -r '[.[] | select(.name | startswith("can_"))] | length')
if [ "$ROLE_COUNT" = "0" ]; then
    echo "  (No permission roles assigned)"
fi

echo "=========================================="
