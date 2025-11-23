#!/bin/bash

# Script to view user claims/attributes in Keycloak
# Usage: ./view-user-claims.sh <username>
# Example: ./view-user-claims.sh "dorin.baba@gmail.com"

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

# Extract attributes
CAN_VIEW=$(echo "$USER_DATA" | jq -r '.[0].attributes.can_view_entities[0] // "not set"')
CAN_MANAGE=$(echo "$USER_DATA" | jq -r '.[0].attributes.can_manage_entities[0] // "not set"')
CAN_ALLOCATE=$(echo "$USER_DATA" | jq -r '.[0].attributes.can_allocate_keys[0] // "not set"')
CAN_SEND=$(echo "$USER_DATA" | jq -r '.[0].attributes.can_send_emails[0] // "not set"')
CAN_ANNOUNCE=$(echo "$USER_DATA" | jq -r '.[0].attributes.can_manage_announcements[0] // "not set"')
CAN_CONFIG=$(echo "$USER_DATA" | jq -r '.[0].attributes.can_manage_system_configuration[0] // "not set"')

echo "=========================================="
echo "User Details"
echo "=========================================="
echo "Username: ${USERNAME}"
echo "Email: ${EMAIL}"
echo "Name: ${FIRST_NAME} ${LAST_NAME}"
echo "Enabled: ${ENABLED}"
echo ""
echo "Permission Claims:"
echo "  can_view_entities: ${CAN_VIEW}"
echo "  can_manage_entities: ${CAN_MANAGE}"
echo "  can_allocate_keys: ${CAN_ALLOCATE}"
echo "  can_send_emails: ${CAN_SEND}"
echo "  can_manage_announcements: ${CAN_ANNOUNCE}"
echo "  can_manage_system_configuration: ${CAN_CONFIG}"
echo "=========================================="
