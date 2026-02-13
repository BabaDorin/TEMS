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

# =============================================================================
# STEP A: Create custom "auto-link-first-login" flow
# This flow auto-creates a Keycloak user on first IDP login, or auto-links to
# an existing user — without ever showing the user any Keycloak pages.
# =============================================================================
echo "Creating custom auto-link-first-login flow..."
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "alias": "auto-link-first-login",
        "description": "Auto-creates or auto-links federated users without any prompts",
        "providerId": "basic-flow",
        "topLevel": true,
        "builtIn": false
    }'

# Add "Create User If Unique" execution (creates new KC user if none matches)
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/auto-link-first-login/executions/execution" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{ "provider": "idp-create-user-if-unique" }'

# Add "Automatically Set Existing User" execution (links to existing KC user)
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/auto-link-first-login/executions/execution" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{ "provider": "idp-auto-link" }'

# Get execution IDs so we can set their requirements to ALTERNATIVE
FIRST_LOGIN_EXECUTIONS=$(curl -s -X GET \
    "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/auto-link-first-login/executions" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}")

CREATE_USER_EXEC_ID=$(echo "$FIRST_LOGIN_EXECUTIONS" | jq -r '.[] | select(.providerId=="idp-create-user-if-unique") | .id')
AUTO_LINK_EXEC_ID=$(echo "$FIRST_LOGIN_EXECUTIONS" | jq -r '.[] | select(.providerId=="idp-auto-link") | .id')

# Set "Create User If Unique" to ALTERNATIVE
curl -s -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/auto-link-first-login/executions" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d "{\"id\": \"${CREATE_USER_EXEC_ID}\", \"requirement\": \"ALTERNATIVE\"}"

# Set "Automatically Set Existing User" to ALTERNATIVE
curl -s -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/auto-link-first-login/executions" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d "{\"id\": \"${AUTO_LINK_EXEC_ID}\", \"requirement\": \"ALTERNATIVE\"}"

echo "  ✅ auto-link-first-login flow created (no prompts, no password verification)"

# =============================================================================
# STEP B: Create Duende Identity Provider
# Uses the auto-link flow for first login, NO post-broker-login flow.
# =============================================================================
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
        "firstBrokerLoginFlowAlias": "auto-link-first-login",
        "config": {
            "authorizationUrl": "http://localhost:5001/connect/authorize",
            "tokenUrl": "http://host.docker.internal:5001/connect/token",
            "userInfoUrl": "http://host.docker.internal:5001/connect/userinfo",
            "logoutUrl": "http://localhost:5001/connect/endsession",
            "clientId": "keycloak-broker",
            "clientSecret": "keycloak-secret",
            "clientAuthMethod": "client_secret_post",
            "defaultScope": "openid profile email",
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

echo "  ✅ Duende identity provider created (firstBrokerLoginFlow=auto-link, no postBrokerLogin)"

# =============================================================================
# STEP C: Create custom browser flow that auto-redirects to Duende
# The user never sees Keycloak's login page.
# =============================================================================
echo "Creating custom browser flow with automatic IdP redirect..."

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

echo "  Custom browser flow created"

# Add Identity Provider Redirector execution
curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/duende-browser-flow/executions/execution" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "provider": "identity-provider-redirector"
    }'

# Get the execution ID
EXECUTIONS=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/authentication/flows/duende-browser-flow/executions" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}")

IDP_REDIRECTOR_ID=$(echo "$EXECUTIONS" | jq -r '.[] | select(.providerId=="identity-provider-redirector") | .id')

# Configure the redirector to use Duende as default
if [ ! -z "$IDP_REDIRECTOR_ID" ] && [ "$IDP_REDIRECTOR_ID" != "null" ]; then
    echo "  Configuring IdP redirector to automatically redirect to Duende..."
    
    # Update the execution to be REQUIRED
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

echo "  ✅ IdP redirector configured"

# Set the custom flow as the default browser flow for the realm
echo "Setting custom flow as default browser flow..."
curl -s -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "browserFlow": "duende-browser-flow"
    }'

echo "  ✅ Browser flow configured — user will always be redirected to Duende automatically"

# Add protocol mappers for roles and claims
# Create realm roles for TEMS permissions (correct roles matching backend)
echo "Creating realm roles for TEMS permissions..."

TEMS_ROLES=(
    "can_manage_assets"
    "can_manage_tickets"
    "can_open_tickets"
    "can_manage_users"
)

for role in "${TEMS_ROLES[@]}"; do
    # Check if role already exists
    EXISTING_ROLE=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/roles/${role}" \
        -H "Authorization: Bearer ${ADMIN_TOKEN}" 2>/dev/null | jq -r '.name')
    
    if [ "$EXISTING_ROLE" = "$role" ]; then
        echo "  ✅ Role '${role}' already exists"
    else
        echo "  Creating role: ${role}"
        curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/roles" \
            -H "Authorization: Bearer ${ADMIN_TOKEN}" \
            -H "Content-Type: application/json" \
            -d "{
                \"name\": \"${role}\",
                \"description\": \"Permission for ${role}\",
                \"composite\": false,
                \"clientRole\": false
            }"
    fi
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
echo "Assigning all TEMS roles to admin user..."
for role in "${TEMS_ROLES[@]}"; do
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

# Assign can_open_tickets role to regular user
echo "Assigning can_open_tickets role to regular user..."
TICKET_ROLE_DATA=$(curl -s -X GET "${KEYCLOAK_URL}/admin/realms/${REALM}/roles/can_open_tickets" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}")

curl -s -X POST "${KEYCLOAK_URL}/admin/realms/${REALM}/users/${REGULAR_USER_ID}/role-mappings/realm" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d "[${TICKET_ROLE_DATA}]"

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
echo "    Permissions: can_manage_assets, can_manage_tickets, can_open_tickets, can_manage_users"
echo ""
echo "  Regular User:"
echo "    Username: user"
echo "    Password: User123!"
echo "    Permissions: can_open_tickets only"
echo "=========================================="
