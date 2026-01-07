#!/bin/bash
KEYCLOAK_URL="http://localhost:8080"
REALM="tems"
ADMIN_USER="admin"
ADMIN_PASSWORD="admin"

echo "Getting admin token..."
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

echo "Updating Duende Identity Provider settings..."
curl -s -X PUT "${KEYCLOAK_URL}/admin/realms/${REALM}/identity-provider/instances/duende-idp" \
    -H "Authorization: Bearer ${ADMIN_TOKEN}" \
    -H "Content-Type: application/json" \
    -d '{
        "alias": "duende-idp",
        "displayName": "Duende IdentityServer",
        "providerId": "oidc",
        "enabled": true,
        "updateProfileFirstLoginMode": "on",
        "trustEmail": true,
        "storeToken": false,
        "addReadTokenRoleOnCreate": false,
        "authenticateByDefault": false,
        "linkOnly": false,
        "firstBrokerLoginFlowAlias": "first broker login",
        "config": {
            "hideOnLoginPage": "false",
            "userInfoUrl": "http://host.docker.internal:5001/connect/userinfo",
            "validateSignature": "false",
            "tokenUrl": "http://host.docker.internal:5001/connect/token",
            "uiLocales": "false",
            "backchannelSupported": "false",
            "issuer": "http://localhost:5001",
            "useJwksUrl": "true",
            "loginHint": "false",
            "pkceEnabled": "false",
            "authorizationUrl": "http://localhost:5001/connect/authorize",
            "disableUserInfo": "false",
            "logoutUrl": "http://localhost:5001/connect/endsession",
            "clientId": "keycloak-broker",
            "acceptsPromptNoneForwardFromClient": "false",
            "jwksUrl": "http://host.docker.internal:5001/.well-known/openid-configuration/jwks",
            "syncMode": "IMPORT",
            "clientSecret": "keycloak-secret",
            "defaultScope": "openid profile email roles",
            "clientAuthMethod": "client_secret_post"
        }
    }'

echo ""
echo "Identity Provider settings updated!"
echo "Key changes:"
echo "  - trustEmail: true (trust email from Duende)"
echo "  - syncMode: IMPORT (create users on first login)"
echo "  - updateProfileFirstLoginMode: on"
echo ""
echo "Try logging in again. The user should be created automatically in Keycloak."
