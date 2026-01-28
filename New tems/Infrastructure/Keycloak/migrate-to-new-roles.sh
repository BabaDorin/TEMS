#!/bin/bash

# Keycloak Configuration
KEYCLOAK_URL="http://localhost:8080"
REALM="tems"

# Get admin token
echo "Getting admin access token..."
ADMIN_TOKEN=$(curl -s -X POST "$KEYCLOAK_URL/realms/master/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "username=admin" \
  -d "password=admin" \
  -d "grant_type=password" \
  -d "client_id=admin-cli" | jq -r '.access_token')

if [ -z "$ADMIN_TOKEN" ] || [ "$ADMIN_TOKEN" = "null" ]; then
    echo "❌ Failed to get admin token"
    exit 1
fi

echo "✅ Got admin token"

# Define new roles
NEW_ROLES=(
    "can_manage_assets"
    "can_manage_tickets"
    "can_open_tickets"
)

# Define old roles to remove
OLD_ROLES=(
    "can_view_entities"
    "can_manage_entities"
    "can_allocate_keys"
    "can_send_emails"
    "can_manage_announcements"
    "can_manage_system_configuration"
)

# Create new realm roles
echo ""
echo "Creating new realm roles..."
for ROLE in "${NEW_ROLES[@]}"; do
    EXISTING_ROLE=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/roles/$ROLE" \
      -H "Authorization: Bearer $ADMIN_TOKEN" 2>/dev/null | jq -r '.name')
    
    if [ "$EXISTING_ROLE" = "$ROLE" ]; then
        echo "  ✅ Role '$ROLE' already exists"
    else
        echo "  Creating role '$ROLE'..."
        curl -s -X POST "$KEYCLOAK_URL/admin/realms/$REALM/roles" \
          -H "Authorization: Bearer $ADMIN_TOKEN" \
          -H "Content-Type: application/json" \
          -d "{\"name\": \"$ROLE\", \"description\": \"$ROLE role\"}"
        echo "  ✅ Created role '$ROLE'"
    fi
done

# Get admin user ID
echo ""
echo "Finding admin user..."
ADMIN_USER_ID=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/users?username=admin&exact=true" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')

if [ -z "$ADMIN_USER_ID" ] || [ "$ADMIN_USER_ID" = "null" ]; then
    echo "❌ Admin user not found"
    exit 1
fi

echo "✅ Found admin user: $ADMIN_USER_ID"

# Assign new roles to admin user
echo ""
echo "Assigning new roles to admin user..."
for ROLE in "${NEW_ROLES[@]}"; do
    ROLE_DATA=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/roles/$ROLE" \
      -H "Authorization: Bearer $ADMIN_TOKEN")
    
    curl -s -X POST "$KEYCLOAK_URL/admin/realms/$REALM/users/$ADMIN_USER_ID/role-mappings/realm" \
      -H "Authorization: Bearer $ADMIN_TOKEN" \
      -H "Content-Type: application/json" \
      -d "[$ROLE_DATA]"
    
    echo "  ✅ Assigned role '$ROLE'"
done

# Remove old roles from admin user
echo ""
echo "Removing old roles from admin user..."
for ROLE in "${OLD_ROLES[@]}"; do
    ROLE_DATA=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/roles/$ROLE" \
      -H "Authorization: Bearer $ADMIN_TOKEN" 2>/dev/null)
    
    if [ -n "$ROLE_DATA" ] && [ "$ROLE_DATA" != "null" ]; then
        curl -s -X DELETE "$KEYCLOAK_URL/admin/realms/$REALM/users/$ADMIN_USER_ID/role-mappings/realm" \
          -H "Authorization: Bearer $ADMIN_TOKEN" \
          -H "Content-Type: application/json" \
          -d "[$ROLE_DATA]"
        echo "  ✅ Removed role '$ROLE' from user"
    fi
done

# Delete old roles from realm
echo ""
echo "Deleting old roles from realm..."
for ROLE in "${OLD_ROLES[@]}"; do
    EXISTING=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/roles/$ROLE" \
      -H "Authorization: Bearer $ADMIN_TOKEN" 2>/dev/null | jq -r '.name')
    
    if [ "$EXISTING" = "$ROLE" ]; then
        curl -s -X DELETE "$KEYCLOAK_URL/admin/realms/$REALM/roles/$ROLE" \
          -H "Authorization: Bearer $ADMIN_TOKEN"
        echo "  ✅ Deleted role '$ROLE'"
    fi
done

echo ""
echo "✅ Role migration complete!"
echo ""
echo "📝 New roles:"
echo "  - can_manage_assets (Asset management - create types, view, edit)"
echo "  - can_manage_tickets (Ticket management - create types, view all)"
echo "  - can_open_tickets (Open tickets using existing types)"
echo ""
echo "🔄 Please log out and log back in for changes to take effect"
