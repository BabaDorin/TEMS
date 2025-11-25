# TEMS Role-Based Authorization - Quick Reference

## ✅ System Now Uses Keycloak Roles

The system has been converted from user attributes to **Keycloak realm roles** for dynamic permission management.

## Available Roles

- `can_view_entities`
- `can_manage_entities`
- `can_allocate_keys`
- `can_send_emails`
- `can_manage_announcements`
- `can_manage_system_configuration`

## Quick Commands

### Assign Roles to a User
```bash
cd Infrastructure/Keycloak
./assign-roles.sh "user@example.com" can_view_entities can_manage_entities
```

### Remove Roles from a User
```bash
cd Infrastructure/Keycloak
./remove-roles.sh "user@example.com" can_manage_entities
```

### List User's Roles
```bash
cd Infrastructure/Keycloak
./list-user-roles.sh "user@example.com"
```

### Create Roles (if needed)
```bash
cd Infrastructure/Keycloak
./create-roles.sh
```

## Your User is Ready!

Your user (`dorin.baba@gmail.com`) now has all 6 roles assigned. **Log out and log back in** to get a fresh token.

## How It Works

### In Keycloak
- Roles are **realm roles** (not client roles)
- Assigned per user dynamically via API
- Appear in JWT token under `realm_access.roles` and `roles` claim

### In JWT Token
After login, your token will include:
```json
{
  "realm_access": {
    "roles": [
      "can_view_entities",
      "can_manage_entities",
      "can_allocate_keys",
      "can_send_emails",
      "can_manage_announcements",
      "can_manage_system_configuration"
    ]
  },
  "roles": [
    "can_view_entities",
    "can_manage_entities",
    ...
  ]
}
```

### In Backend (.NET)
Authorization policies use `RequireRole()`:
```csharp
[Authorize(Policy = "CanManageEntities")]
public class MyEndpoint : Endpoint<MyRequest, MyResponse>
```

### In Frontend (Angular)
Token service checks roles array:
```typescript
tokenService.canManageEntities() // returns true/false
```

## Managing Roles via API

You can build a user management UI in your Angular app using the Keycloak Admin REST API:

### Assign Role
```typescript
POST http://localhost:8080/admin/realms/tems/users/{userId}/role-mappings/realm
Authorization: Bearer {admin-token}
Content-Type: application/json

[{
  "id": "{role-id}",
  "name": "can_view_entities"
}]
```

### Remove Role
```typescript
DELETE http://localhost:8080/admin/realms/tems/users/{userId}/role-mappings/realm
Authorization: Bearer {admin-token}
Content-Type: application/json

[{
  "id": "{role-id}",
  "name": "can_view_entities"
}]
```

### Get User's Roles
```typescript
GET http://localhost:8080/admin/realms/tems/users/{userId}/role-mappings/realm
Authorization: Bearer {admin-token}
```

## Example: Grant Admin User All Permissions
```bash
./assign-roles.sh "admin@tems.local" \
  can_view_entities \
  can_manage_entities \
  can_allocate_keys \
  can_send_emails \
  can_manage_announcements \
  can_manage_system_configuration
```

## Example: Grant Regular User View-Only
```bash
./assign-roles.sh "user@tems.local" can_view_entities
```

## Example: Promote User to Manager
```bash
./assign-roles.sh "user@tems.local" \
  can_manage_entities \
  can_allocate_keys
```

## Example: Demote User
```bash
./remove-roles.sh "user@tems.local" \
  can_manage_entities \
  can_allocate_keys
```

## Files Changed

### Updated
- `Infrastructure/Keycloak/configure-keycloak.sh` - Creates roles instead of attribute mappers
- `Backend/Tems/Tems.Host/Program.cs` - Uses `RequireRole()` instead of `RequireClaim()`
- `Frontend/Tems/src/app/services/token.service.ts` - Reads from `realm_access.roles`

### New Scripts
- `Infrastructure/Keycloak/create-roles.sh` - Create realm roles
- `Infrastructure/Keycloak/assign-roles.sh` - Assign roles to users
- `Infrastructure/Keycloak/remove-roles.sh` - Remove roles from users
- `Infrastructure/Keycloak/list-user-roles.sh` - List user's roles

## Next Steps

1. **Log out and log back in** to get fresh JWT with roles
2. **Test authorization** in the app
3. **Build user management UI** to assign/remove roles dynamically
4. **Create role groups** in Keycloak for common permission sets (optional)

## Benefits of Roles vs Attributes

✅ **Dynamic Assignment** - Change permissions without updating user profile  
✅ **REST API Support** - Easy to manage via Keycloak Admin API  
✅ **Standard Pattern** - Follows OAuth2/OIDC best practices  
✅ **Hierarchical** - Can create composite roles (role contains other roles)  
✅ **Auditable** - Keycloak logs all role assignments  

---

**Status:** ✅ Complete - Roles configured, scripts ready, your user has all permissions
