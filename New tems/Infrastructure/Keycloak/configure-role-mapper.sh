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

# Create or update realm roles mapper for the client
echo "Configuring realm roles mapper..."
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

# Check if mapper already exists
EXISTING_MAPPER=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[] | select(.name == "realm roles") | .id')

if [ -z "$EXISTING_MAPPER" ] || [ "$EXISTING_MAPPER" = "null" ]; then
    echo "Creating realm roles mapper..."
    curl -s -X POST "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models" \
      -H "Authorization: Bearer $ADMIN_TOKEN" \
      -H "Content-Type: application/json" \
      -d "$MAPPER_CONFIG"
    echo "✅ Realm roles mapper created"
else
    echo "Updating existing realm roles mapper..."
    curl -s -X PUT "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models/$EXISTING_MAPPER" \
      -H "Authorization: Bearer $ADMIN_TOKEN" \
      -H "Content-Type: application/json" \
      -d "$MAPPER_CONFIG"
    echo "✅ Realm roles mapper updated"
fi

# Also add roles to userinfo endpoint
echo "Ensuring roles are included in all token types..."

# Add audience mapper for the backend
AUDIENCE_MAPPER='{
  "name": "audience-mapper",
  "protocol": "openid-connect",
  "protocolMapper": "oidc-audience-mapper",
  "consentRequired": false,
  "config": {
    "included.client.audience": "tems-api",
    "id.token.claim": "false",
    "access.token.claim": "true"
  }
}'

EXISTING_AUDIENCE=$(curl -s -X GET "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models" \
  -H "Authorization: Bearer $ADMIN_TOKEN" | jq -r '.[] | select(.name == "audience-mapper") | .id')

if [ -z "$EXISTING_AUDIENCE" ] || [ "$EXISTING_AUDIENCE" = "null" ]; then
    echo "Creating audience mapper..."
    curl -s -X POST "$KEYCLOAK_URL/admin/realms/$REALM/clients/$CLIENT_UUID/protocol-mappers/models" \
      -H "Authorization: Bearer $ADMIN_TOKEN" \
      -H "Content-Type: application/json" \
      -d "$AUDIENCE_MAPPER"
    echo "✅ Audience mapper created"
else
    echo "✅ Audience mapper already exists"
fi

echo ""
echo "✅ Role mapper configuration complete!"
echo ""
echo "📝 Summary:"
echo "  - Realm roles are now mapped to 'roles' claim"
echo "  - Roles included in: ID token, Access token, UserInfo"
echo "  - Audience set to 'tems-api' for backend validation"
echo ""
echo "🔄 Please log out and log back in for changes to take effect"
