#!/bin/bash

# Keycloak Configuration Script for TEMS
# This script configures Keycloak realm, clients, and identity provider

KEYCLOAK_URL="http://localhost:8080"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"
REALM="tems"

echo "Waiting for Keycloak to be ready..."
TIMEOUT=120
ELAPSED=0
until curl -sf "${KEYCLOAK_URL}/realms/master" > /dev/null 2>&1; do
    if [ $ELAPSED -ge $TIMEOUT ]; then
        echo "Timeout waiting for Keycloak to be ready after ${TIMEOUT} seconds"
        exit 1
    fi
    echo "Keycloak is not ready yet. Waiting... (${ELAPSED}s/${TIMEOUT}s)"
    sleep 5
    ELAPSED=$((ELAPSED + 5))
done
echo "Keycloak is ready!"

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

# Create TEMS realm
echo "Creating TEMS realm..."
curl -s -X POST "${KEYCLOAK_URL}/admin/realms" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "realm": "tems",
        "enabled": true,
        "displayName": "TEMS",
        "displayNameHtml": "<b>TEMS Authentication</b>",
        "loginTheme": "keycloak",
        "accessTokenLifespan": 900,
        "ssoSessionIdleTimeout": 1800,
        "ssoSessionMaxLifespan": 36000,
        "offlineSessionIdleTimeout": 2592000,
        "accessCodeLifespan": 60,
        "accessCodeLifespanUserAction": 300,
        "accessCodeLifespanLogin": 1800
    }'

echo "TEMS realm created"

# Create Angular SPA client
echo "Creating Angular SPA client..."
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/clients" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "clientId": "tems-angular-spa",
        "name": "TEMS Angular SPA",
        "description": "TEMS Angular Single Page Application",
        "enabled": true,
        "publicClient": true,
        "protocol": "openid-connect",
        "standardFlowEnabled": true,
        "implicitFlowEnabled": false,
        "directAccessGrantsEnabled": false,
        "serviceAccountsEnabled": false,
        "redirectUris": [
            "http://localhost:4200/*",
            "http://localhost:4200/home"
        ],
        "webOrigins": [
            "http://localhost:4200"
        ],
        "attributes": {
            "pkce.code.challenge.method": "S256"
        },
        "fullScopeAllowed": true
    }'

echo "Angular SPA client created"

# Create Duende Identity Provider
echo "Creating Duende IdentityServer identity provider..."
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/identity-provider/instances" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "alias": "duende-idp",
        "displayName": "Duende IdentityServer",
        "providerId": "oidc",
        "enabled": true,
        "trustEmail": true,
        "storeToken": true,
        "addReadTokenRoleOnCreate": false,
        "authenticateByDefault": true,
        "linkOnly": false,
        "firstBrokerLoginFlowAlias": "first broker login",
        "postBrokerLoginFlowAlias": "first broker login",
        "config": {
            "authorizationUrl": "http://localhost:5001/connect/authorize",
            "tokenUrl": "http://host.docker.internal:5001/connect/token",
            "userInfoUrl": "http://host.docker.internal:5001/connect/userinfo",
            "logoutUrl": "http://localhost:5001/connect/endsession",
            "clientId": "keycloak-broker",
            "clientSecret": "keycloak-secret",
            "clientAuthMethod": "client_secret_post",
            "defaultScope": "openid profile email roles tems-api",
            "syncMode": "FORCE",
            "useJwksUrl": "true",
            "jwksUrl": "http://host.docker.internal:5001/.well-known/openid-configuration/jwks",
            "validateSignature": "true",
            "backchannelSupported": "false",
            "disableUserInfo": "false",
            "hideOnLoginPage": "false",
            "loginHint": "false",
            "uiLocales": "false",
            "acceptsPromptNoneForwardFromClient": "false"
        }
    }'

echo "Duende identity provider created"

# Set Duende as default IdP for the realm
echo "Setting Duende as default identity provider..."
curl -s -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "defaultDefaultClientScopes": ["web-origins","acr","roles","profile","email"],
        "identityProviders": [],
        "identityProviderMappers": []
    }'

# Configure authentication flow to redirect directly to Duende
echo "Creating custom browser flow with automatic IdP redirect..."

# Create a new authentication flow
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "alias": "duende-browser-flow",
        "description": "Browser flow that redirects to Duende IdentityServer",
        "providerId": "basic-flow",
        "topLevel": true,
        "builtIn": false
    }'

echo "Custom browser flow created"

# Get the flow ID
CUSTOM_FLOW_ID=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" | jq -r '.[] | select(.alias=="duende-browser-flow") | .id')

# Add Identity Provider Redirector execution
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/duende-browser-flow/executions/execution" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "provider": "identity-provider-redirector"
    }'

echo "IdP redirector execution added"

# Get the execution ID
EXECUTIONS=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/duende-browser-flow/executions" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}")

IDP_REDIRECTOR_ID=$(echo "$EXECUTIONS" | jq -r '.[] | select(.providerId=="identity-provider-redirector") | .id')

# Configure the redirector to use Duende as default
if [ ! -z "$IDP_REDIRECTOR_ID" ] && [ "$IDP_REDIRECTOR_ID" != "null" ]; then
    echo "Configuring IdP redirector to automatically redirect to Duende..."
    
    # First, update the execution to be REQUIRED
    curl -s -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/duende-browser-flow/executions" \
        -H "Authorization: Bearer ${ADMIN_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"id\": \"${IDP_REDIRECTOR_ID}\",
            \"requirement\": \"REQUIRED\"
        }"
    
    # Configure the redirector with Duende as default
    curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/executions/${IDP_REDIRECTOR_ID}/config" \
        -H "Authorization: Bearer ${ADMIN_TOKEN}" \
        -H "Content-Type: application/json" \
        -d '{
            "alias": "duende-auto-redirect",
            "config": {
                "defaultProvider": "duende-idp"
            }
        }'
fi

# Set the custom flow as the default browser flow for the realm
echo "Setting custom flow as default browser flow..."
curl -s -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "browserFlow": "duende-browser-flow"
    }'

echo "Browser flow configured - Keycloak will now redirect to Duende IdentityServer"

# Add protocol mappers for roles and claims
# Create realm roles for permissions
echo "Creating realm roles for permissions..."

for role in "can_view_entities" "can_manage_entities" "can_allocate_keys" "can_send_emails" "can_manage_announcements" "can_manage_system_configuration"; do
    echo "Creating role: ${role}"
    curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/roles" \
        -H "Authorization: Bearer ${ADMIN_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "{
            \"name\": \"${role}\",
            \"description\": \"Permission to ${role}\",
            \"composite\": false,
            \"clientRole\": false
        }"
done

echo "Realm roles created"

# Get client ID
echo "Configuring client protocol mappers..."
CLIENT_ID=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/clients?clientId=tems-angular-spa" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" | jq -r '.[0].id')

# Add realm roles mapper to include roles in token
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/clients/${CLIENT_ID}/protocol-mappers/models" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "name": "realm-roles-mapper",
        "protocol": "openid-connect",
        "protocolMapper": "oidc-usermodel-realm-role-mapper",
        "config": {
            "claim.name": "roles",
            "jsonType.label": "String",
            "id.token.claim": "true",
            "access.token.claim": "true",
            "userinfo.token.claim": "true",
            "multivalued": "true"
        }
    }'

echo "Protocol mappers configured"

# Create a test admin user with all roles
echo "Creating test admin user with all roles..."
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/users" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "username": "admin",
        "email": "admin@tems.local",
        "firstName": "Admin",
        "lastName": "User",
        "enabled": true,
        "emailVerified": true,
        "credentials": [{
            "type": "password",
            "value": "Admin123!",
            "temporary": false
        }]
    }'

# Get admin user ID
ADMIN_USER_ID=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/users?username=admin" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" | jq -r '.[0].id')

# Assign all roles to admin user
echo "Assigning all roles to admin user..."
for role in "can_view_entities" "can_manage_entities" "can_allocate_keys" "can_send_emails" "can_manage_announcements" "can_manage_system_configuration"; do
    ROLE_DATA=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/roles/${role}" \
        -H "Authorization: Bearer ${ADMIN_TOKEN}")
    
    curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/users/${ADMIN_USER_ID}/role-mappings/realm" \
        -H "Authorization: Bearer ${ADMIN_TOKEN}" \
        -H "Content-Type: application/json" \
        -d "[${ROLE_DATA}]"
done

echo "Admin user created with username: admin, password: Admin123!"

# Create a test regular user with limited roles
echo "Creating test regular user with limited roles..."
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/users" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "username": "user",
        "email": "user@tems.local",
        "firstName": "Regular",
        "lastName": "User",
        "enabled": true,
        "emailVerified": true,
        "credentials": [{
            "type": "password",
            "value": "User123!",
            "temporary": false
        }]
    }'

# Get regular user ID
REGULAR_USER_ID=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/users?username=user" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" | jq -r '.[0].id')

# Assign only view role to regular user
echo "Assigning view role to regular user..."
VIEW_ROLE_DATA=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/roles/can_view_entities" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}")

curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/users/${REGULAR_USER_ID}/role-mappings/realm" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d "[${VIEW_ROLE_DATA}]"

echo "Regular user created with username: user, password: User123!"

echo "=========================================="
echo "Keycloak configuration completed!"
echo "=========================================="
echo "Keycloak URL: ${KEYCLOAK_URL}"
echo "Admin Console: ${KEYCLOAK_URL}/admin"
echo "TEMS Realm: ${REALM}"
echo "Admin Username: ${ADMIN_USER}"
echo "Admin Password: ${ADMIN_PASSWORD}"
echo ""
echo "Test Users Created:"
echo "  Admin User:"
echo "    Username: admin"
echo "    Password: Admin123!"
echo "    Permissions: ALL"
echo ""
echo "  Regular User:"
echo "    Username: user"
echo "    Password: User123!"
echo "    Permissions: can_view_entities only"
echo "=========================================="
