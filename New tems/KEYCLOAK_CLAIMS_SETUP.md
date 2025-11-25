# Keycloak Claims Configuration Guide

## Overview

TEMS uses Keycloak for authentication with custom user attributes that are mapped to JWT claims for authorization. This document explains how claims are configured and used throughout the system.

## Permission Claims

The system uses 6 permission claims:

- `can_view_entities` - View entity data
- `can_manage_entities` - Create, update, delete entities
- `can_allocate_keys` - Manage key allocations
- `can_send_emails` - Send system emails
- `can_manage_announcements` - Manage announcements
- `can_manage_system_configuration` - System configuration access

## How Claims Work

### 1. User Attributes in Keycloak

Claims are stored as **user attributes** in Keycloak. Each user has these attributes with values of `"true"` or `"false"`.

To view/edit in Keycloak Admin Console:
1. Navigate to: Realm `tems` → Users → Select User
2. Go to "Attributes" tab
3. Add/edit attributes as key-value pairs

### 2. Protocol Mappers

The `configure-keycloak.sh` script automatically creates **protocol mappers** that copy user attributes into JWT tokens. These mappers:
- Read from user attributes (e.g., `can_view_entities`)
- Add them to access tokens, ID tokens, and userinfo endpoint
- Use the same claim name in the JWT

### 3. JWT Token Structure

After authentication, your JWT will look like this:

```json
{
  "exp": 1763887365,
  "sub": "94939724-b92a-41b9-9eae-68c1702375e6",
  "email": "dorin.baba@gmail.com",
  "can_view_entities": "true",
  "can_manage_entities": "true",
  "can_allocate_keys": "true",
  "can_send_emails": "true",
  "can_manage_announcements": "true",
  "can_manage_system_configuration": "true",
  ...other claims
}
```

**Note:** Claims are at the **root level** of the JWT, not nested in `realm_access` or `resource_access`.

### 4. Frontend (Angular)

The `token.service.ts` reads claims directly from the JWT:

```typescript
hasClaim(claimType: string): boolean {
  const claims = this.oauthService.getIdentityClaims() as any;
  if (!claims) return false;
  
  const mappedClaim = claimMap[claimType] || claimType;
  
  // Claims are "true" or "false" as strings
  return claims[mappedClaim] === 'true' || claims[mappedClaim] === true;
}
```

### 5. Backend (.NET)

The `Program.cs` defines authorization policies:

```csharp
options.AddPolicy("CanViewEntities", policy =>
    policy.RequireClaim("can_view_entities", "true"));
```

Endpoints are protected with:

```csharp
[Authorize(Policy = "CanManageEntities")]
public class MyEndpoint : Endpoint<MyRequest, MyResponse>
{
    // ...
}
```

## Managing User Claims

### Option 1: Keycloak Admin Console (Manual)

1. Log in to Keycloak Admin Console: http://localhost:8080/admin
2. Select realm: `tems`
3. Navigate to: Users → Select User → Attributes tab
4. Add/edit attributes:
   - Key: `can_view_entities`
   - Value: `true` or `false`
5. Click "Save"
6. User must log out and log back in for changes to take effect

### Option 2: Using the Script (Automated)

Use the provided script to update user claims programmatically:

```bash
cd Infrastructure/Keycloak
./update-user-claims.sh "username@example.com" true true false false false false
```

**Parameters:**
1. Username (email)
2. can_view_entities (true/false)
3. can_manage_entities (true/false)
4. can_allocate_keys (true/false)
5. can_send_emails (true/false)
6. can_manage_announcements (true/false)
7. can_manage_system_configuration (true/false)

**Example - Grant all permissions:**
```bash
./update-user-claims.sh "admin@tems.local" true true true true true true
```

**Example - Grant only view access:**
```bash
./update-user-claims.sh "viewer@tems.local" true false false false false false
```

### Option 3: During User Creation

When creating users via API or script, include attributes:

```json
{
  "username": "newuser",
  "email": "newuser@tems.local",
  "enabled": true,
  "attributes": {
    "can_view_entities": ["true"],
    "can_manage_entities": ["false"],
    "can_allocate_keys": ["false"],
    "can_send_emails": ["false"],
    "can_manage_announcements": ["false"],
    "can_manage_system_configuration": ["false"]
  }
}
```

## Initial Setup

### 1. Run Keycloak Configuration

This creates the realm, client, protocol mappers, and test users:

```bash
cd Infrastructure/Keycloak
./configure-keycloak.sh
```

**Test users created:**
- Username: `admin` / Password: `Admin123!` (all permissions)
- Username: `user` / Password: `User123!` (view only)

### 2. Update Existing Users

If you have existing users in Keycloak, update their claims:

```bash
./update-user-claims.sh "dorin.baba@gmail.com" true true true true true true
```

### 3. Test the Flow

1. Log out from TEMS if currently logged in
2. Navigate to http://localhost:4200
3. Click "Login"
4. Authenticate with updated user
5. Check browser DevTools → Application → Session Storage → see JWT with claims

## Troubleshooting

### Claims not appearing in JWT

**Check:**
1. Protocol mappers are configured in Keycloak client `tems-angular-spa`
2. User attributes are set (not empty)
3. User has logged out and back in after attribute changes
4. Token is fresh (not cached)

**Verify protocol mappers:**
```bash
# In Keycloak Admin Console
Realm: tems → Clients → tems-angular-spa → Client scopes tab → 
Select dedicated scope → Mappers tab
```

You should see mappers for each claim:
- `can_view_entities-mapper`
- `can_manage_entities-mapper`
- etc.

### Authorization failing on backend

**Check:**
1. JWT contains the claim (decode at jwt.io)
2. Claim value is `"true"` as a **string**, not boolean
3. Backend policy matches claim name exactly
4. CORS is configured correctly
5. Token is being sent in Authorization header

**Test backend token validation:**
```bash
# Get token from browser session storage
# Then test an endpoint
curl -X GET http://localhost:14721/api/equipment \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Frontend not recognizing claims

**Check:**
1. `token.service.ts` is checking both string and boolean: `=== 'true' || === true`
2. Claim names match constants in `models/claims.ts`
3. Token is valid and not expired
4. No console errors in browser DevTools

## Security Best Practices

1. **Principle of Least Privilege:** Grant only necessary permissions
2. **Regular Audits:** Review user permissions periodically
3. **Attribute Values:** Always use lowercase `"true"` / `"false"` strings
4. **Token Refresh:** Claims update only after re-authentication
5. **Backend Validation:** Never trust frontend - always validate on backend

## Integration Points

### When Adding New Claims

1. **Backend (`Program.cs`):**
   ```csharp
   options.AddPolicy("CanNewFeature", policy =>
       policy.RequireClaim("can_new_feature", "true"));
   ```

2. **Frontend (`models/claims.ts`):**
   ```typescript
   export const CAN_NEW_FEATURE = 'can_new_feature';
   ```

3. **Frontend (`token.service.ts`):**
   ```typescript
   const claimMap: Record<string, string> = {
     // ...existing claims
     [CAN_NEW_FEATURE]: 'can_new_feature'
   };
   
   canNewFeature() {
     return this.hasClaim(CAN_NEW_FEATURE);
   }
   ```

4. **Keycloak:**
   - Add protocol mapper for `can_new_feature`
   - Add user attribute `can_new_feature` to users

5. **Update Scripts:**
   - Modify `configure-keycloak.sh` to include new claim in protocol mappers
   - Modify `update-user-claims.sh` to accept new claim parameter

## Files Modified

- `Infrastructure/Keycloak/configure-keycloak.sh` - Creates protocol mappers and test users
- `Infrastructure/Keycloak/update-user-claims.sh` - Updates existing user claims
- `Frontend/Tems/src/app/services/token.service.ts` - Reads claims from JWT
- `Backend/Tems/Tems.Host/Program.cs` - Authorization policies

## References

- [Keycloak User Attributes](https://www.keycloak.org/docs/latest/server_admin/#_user-attributes)
- [Keycloak Protocol Mappers](https://www.keycloak.org/docs/latest/server_admin/#_protocol-mappers)
- [ASP.NET Core Claims-Based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims)

---

**Last Updated:** November 23, 2025
