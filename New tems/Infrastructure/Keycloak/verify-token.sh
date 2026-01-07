#!/bin/bash

echo "=== TEMS Token Verification Tool ==="
echo ""
echo "This script helps verify that your JWT token contains the correct roles."
echo ""
echo "📋 Instructions:"
echo "1. Log into the TEMS frontend (http://localhost:4200)"
echo "2. Open Browser DevTools (F12)"
echo "3. Go to Console tab"
echo "4. Type: localStorage.getItem('access_token')"
echo "5. Copy the token value (without quotes)"
echo "6. Paste it when prompted below"
echo ""
read -p "Paste your access token here: " TOKEN

if [ -z "$TOKEN" ]; then
    echo "❌ No token provided"
    exit 1
fi

echo ""
echo "=== Decoding Token ==="
echo ""

# Decode JWT (split by dots, get payload, base64 decode)
PAYLOAD=$(echo "$TOKEN" | cut -d '.' -f 2)

# Add padding if needed (base64 requires proper padding)
case ${#PAYLOAD} in
    $((${#PAYLOAD} % 4 == 2)))
        PAYLOAD="${PAYLOAD}=="
        ;;
    $((${#PAYLOAD} % 4 == 3)))
        PAYLOAD="${PAYLOAD}="
        ;;
esac

# Decode and pretty print
DECODED=$(echo "$PAYLOAD" | base64 -d 2>/dev/null | jq '.')

if [ $? -ne 0 ]; then
    echo "❌ Failed to decode token. Make sure you copied the full token."
    exit 1
fi

echo "$DECODED"
echo ""
echo "=== Checking Required Claims ==="
echo ""

# Check issuer
ISS=$(echo "$DECODED" | jq -r '.iss')
echo "✓ Issuer: $ISS"
if [[ "$ISS" != *"localhost:8080/realms/tems"* ]]; then
    echo "  ⚠️  Warning: Issuer should be http://localhost:8080/realms/tems"
fi

# Check audience
AUD=$(echo "$DECODED" | jq -r '.aud')
echo "✓ Audience: $AUD"
if [[ "$AUD" != *"account"* ]] && [[ "$AUD" != *"tems-api"* ]]; then
    echo "  ⚠️  Warning: Audience should include 'account' or 'tems-api'"
fi

# Check roles
ROLES=$(echo "$DECODED" | jq -r '.roles[]?' 2>/dev/null)
if [ -z "$ROLES" ]; then
    # Try realm_access.roles
    ROLES=$(echo "$DECODED" | jq -r '.realm_access.roles[]?' 2>/dev/null)
fi

echo "✓ Roles found:"
if [ -z "$ROLES" ]; then
    echo "  ❌ NO ROLES FOUND!"
    echo ""
    echo "This is the problem! Your token doesn't contain any roles."
    echo ""
    echo "To fix:"
    echo "1. Run: cd Infrastructure/Keycloak && ./configure-role-mapper.sh"
    echo "2. Log out from the frontend"
    echo "3. Log back in"
    echo "4. Run this script again to verify"
else
    echo "$ROLES" | while read role; do
        echo "  - $role"
    done
    
    echo ""
    echo "=== Checking Required Roles ==="
    
    if echo "$ROLES" | grep -q "can_manage_assets"; then
        echo "✅ can_manage_assets - Can access Asset Management endpoints"
    else
        echo "❌ can_manage_assets - MISSING! Cannot access Asset Management endpoints"
    fi
    
    if echo "$ROLES" | grep -q "can_manage_tickets"; then
        echo "✅ can_manage_tickets - Can access Ticket Management endpoints"
    else
        echo "⚠️  can_manage_tickets - Not assigned (optional)"
    fi
    
    if echo "$ROLES" | grep -q "can_open_tickets"; then
        echo "✅ can_open_tickets - Can create tickets"
    else
        echo "⚠️  can_open_tickets - Not assigned (optional)"
    fi
fi

echo ""
echo "=== Token Expiration ==="
EXP=$(echo "$DECODED" | jq -r '.exp')
NOW=$(date +%s)
if [ "$EXP" -lt "$NOW" ]; then
    echo "❌ Token EXPIRED at $(date -r $EXP)"
    echo "   Please refresh your browser or log in again"
else
    REMAINING=$((EXP - NOW))
    echo "✅ Token valid for $((REMAINING / 60)) minutes"
fi

echo ""
echo "=== Done ==="
