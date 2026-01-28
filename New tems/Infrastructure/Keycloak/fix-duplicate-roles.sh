#!/bin/bash

# Keycloak Configuration
KEYCLOAK_URL="http://localhost:8080"
REALM="tems"
CLIENT_ID="tems-angular-spa"

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

# Get client UUID
echo "Getting client UUID..."
CLIENT_UUID=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/clients?clientId=$CLIENT_ID" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[0].id')

if [ -z "$CLIENT_UUID" ] || [ "$CLIENT_UUID" = "null" ]; then
    echo "❌ Failed to get client UUID"
    exit 1
fi

echo "✅ Client UUID: $CLIENT_UUID"

# Get all protocol mappers for the client
echo ""
echo "Checking for duplicate role mappers..."
MAPPERS=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models" \
  -H "Authorization: Bearer $ADMIN_TOKEN")

# Find duplicate role mappers
REALM_ROLES_MAPPERS=$(echo "$MAPPERS" | jq -r '.[] | select(.protocolMapper == "oidc-usermodel-realm-role-mapper") | .id')

MAPPER_COUNT=$(echo "$REALM_ROLES_MAPPERS" | wc -l | tr -d ' ')

if [ "$MAPPER_COUNT" -gt 1 ]; then
    echo "⚠️  Found $MAPPER_COUNT realm role mappers (duplicates detected)"
    echo ""
    echo "Removing duplicate mappers..."
    
    # Keep the first one, delete the rest
    FIRST_MAPPER=""
    for MAPPER_ID in $REALM_ROLES_MAPPERS; do
        if [ -z "$FIRST_MAPPER" ]; then
            FIRST_MAPPER="$MAPPER_ID"
            echo "  ✅ Keeping mapper: $MAPPER_ID"
        else
            echo "  🗑️  Deleting duplicate mapper: $MAPPER_ID"
            curl -s -X DELETE "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models/$MAPPER_ID" \
              -H "Authorization: Bearer $ADMIN_TOKEN"
        fi
    done
else
    echo "✅ No duplicate mappers found"
fi

# Ensure we have exactly one realm roles mapper with correct config
echo ""
echo "Configuring realm roles mapper..."

REMAINING_MAPPER=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[] | select(.name == "realm roles") | .id')

if [ -n "$REMAINING_MAPPER" ] && [ "$REMAINING_MAPPER" != "null" ]; then
    echo "Updating existing realm roles mapper..."
    MAPPER_CONFIG='{
      "id": "'$REMAINING_MAPPER'",
      "name": "realm roles",
      "protocol": "openid-connect",
      "protocolMapper": "oidc-usermodel-realm-role-mapper",
      "consentRequired": false,
      "config": {
        "multivalued": "true",
        "userinfo.token.claim": "true",
        "id.token.claim": "true",
        "access.token.claim": "true",
        "claim.name": "roles",
        "jsonType.label": "String"
      }
    }'
    
    curl -s -X PUT "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models/$REMAINING_MAPPER" \
      -H "Authorization: Bearer $ADMIN_TOKEN" \
      -H "Content-Type: application/json" \
      -d "$MAPPER_CONFIG"
    
    echo "✅ Realm roles mapper updated"
else
    echo "Creating realm roles mapper..."
    MAPPER_CONFIG='{
      "name": "realm roles",
      "protocol": "openid-connect",
      "protocolMapper": "oidc-usermodel-realm-role-mapper",
      "consentRequired": false,
      "config": {
        "multivalued": "true",
        "userinfo.token.claim": "true",
        "id.token.claim": "true",
        "access.token.claim": "true",
        "claim.name": "roles",
        "jsonType.label": "String"
      }
    }'
    
    curl -s -X POST "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models" \
      -H "Authorization: Bearer $ADMIN_TOKEN" \
      -H "Content-Type: application/json" \
      -d "$MAPPER_CONFIG"
    
    echo "✅ Realm roles mapper created"
fi

echo ""
echo "✅ Role mapper configuration complete!"
echo ""
echo "🔄 Please log out and log back in to get fresh tokens"
