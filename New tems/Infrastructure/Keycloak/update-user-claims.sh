#!/bin/bash

# Script to update user claims/attributes in Keycloak
# Usage: ./update-user-claims.sh <username> <can_view> <can_manage> <can_allocate> <can_send> <can_announce> <can_config>
# Example: ./update-user-claims.sh "dorin.baba@gmail.com" true true true true true true

KEYCLOAK_URL="http://localhost:8080"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"
REALM="tems"

if [ "$#" -ne 7 ]; then
    echo "Usage: $0 <username> <can_view_entities> <can_manage_entities> <can_allocate_keys> <can_send_emails> <can_manage_announcements> <can_manage_system_configuration>"
    echo "Example: $0 'dorin.baba@gmail.com' true true true true true true"
    exit 1
fi

USERNAME="$1"
CAN_VIEW="$2"
CAN_MANAGE="$3"
CAN_ALLOCATE="$4"
CAN_SEND="$5"
CAN_ANNOUNCE="$6"
CAN_CONFIG="$7"

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

# Get current user data
echo "Fetching current user data..."
USER_DATA=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/users/${USER_ID}" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}")

# Extract current attributes (preserve any existing ones)
CURRENT_ATTRS=$(echo "$USER_DATA" | jq -r '.attributes // {}')

# Merge new claims with existing attributes
NEW_ATTRS=$(echo "$CURRENT_ATTRS" | jq \
    --arg cv "$CAN_VIEW" \
    --arg cm "$CAN_MANAGE" \
    --arg ca "$CAN_ALLOCATE" \
    --arg cs "$CAN_SEND" \
    --arg can "$CAN_ANNOUNCE" \
    --arg cc "$CAN_CONFIG" \
    '. + {
        "can_view_entities": [$cv],
        "can_manage_entities": [$cm],
        "can_allocate_keys": [$ca],
        "can_send_emails": [$cs],
        "can_manage_announcements": [$can],
        "can_manage_system_configuration": [$cc]
    }')

# Update user with merged attributes
echo "Updating user claims..."
RESPONSE=$(curl -s -w "\n%{http_code}" -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}/users/${USER_ID}" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d "$(echo "$USER_DATA" | jq --argjson attrs "$NEW_ATTRS" '.attributes = $attrs')")

HTTP_CODE=$(echo "$RESPONSE" | tail -n1)

if [ "$HTTP_CODE" != "204" ]; then
    echo "Error updating user. HTTP status: $HTTP_CODE"
    echo "$RESPONSE"
    exit 1
fi

echo "=========================================="
echo "User claims updated successfully!"
echo "=========================================="
echo "Username: ${USERNAME}"
echo "User ID: ${USER_ID}"
echo ""
echo "Claims set:"
echo "  can_view_entities: ${CAN_VIEW}"
echo "  can_manage_entities: ${CAN_MANAGE}"
echo "  can_allocate_keys: ${CAN_ALLOCATE}"
echo "  can_send_emails: ${CAN_SEND}"
echo "  can_manage_announcements: ${CAN_ANNOUNCE}"
echo "  can_manage_system_configuration: ${CAN_CONFIG}"
echo "=========================================="
echo ""
echo "Note: User needs to log out and log back in for changes to take effect"
