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

# Define all roles needed
ROLES=(
    "Admin"
    "can_view_entities"
    "can_manage_entities"
    "can_allocate_keys"
    "can_send_emails"
    "can_manage_announcements"
    "can_manage_system_configuration"
)

# Create realm roles if they don't exist
echo ""
echo "Creating realm roles..."
for ROLE in "${ROLES[@]}"; do
    # Check if role exists
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

# Get admin user ID in Keycloak
echo ""
echo "Finding admin user in Keycloak..."
ADMIN_USER=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/users?username=admin&exact=true" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0]')

ADMIN_USER_ID=$(echo "$ADMIN_USER" | jq -r '.id')

if [ -z "$ADMIN_USER_ID" ] || [ "$ADMIN_USER_ID" = "null" ]; then
    echo "❌ Admin user not found in Keycloak"
    echo "   User will be auto-created on first login from Duende"
    echo "   Please log in first, then run this script again"
    exit 0
fi

echo "✅ Found admin user: $ADMIN_USER_ID"

# Assign all roles to admin user
echo ""
echo "Assigning roles to admin user..."
for ROLE in "${ROLES[@]}"; do
    # Get role representation
    ROLE_DATA=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/roles/$ROLE" \
      -H "Authorization: Bearer $ADMIN_TOKEN")
    
    # Assign role to user
    curl -s -X POST "$KEYCLOAK_URL/admin/realms/$REALM/users/$ADMIN_USER_ID/role-mappings/realm" \
      -H "Authorization: Bearer $ADMIN_TOKEN" \
      -H "Content-Type: application/json" \
      -d "[$ROLE_DATA]"
    
    echo "  ✅ Assigned role '$ROLE' to admin user"
done

echo ""
echo "✅ Role assignment complete!"
echo ""
echo "📝 Summary:"
echo "  - Created ${#ROLES[@]} realm roles in Keycloak"
echo "  - Assigned all roles to admin user"
echo "  - Roles are now managed entirely by Keycloak"
echo "  - Duende only handles authentication"
echo ""
echo "🔄 Please log out and log back in for changes to take effect"
