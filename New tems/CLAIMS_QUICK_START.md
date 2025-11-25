# Quick Start: Testing Claims After Setup

## What Was Done

1. ✅ **Keycloak Configuration Updated**
   - Protocol mappers already existed for all 6 permission claims
   - Added test user creation (admin + regular user) to `configure-keycloak.sh`

2. ✅ **Update Script Created**
   - New script: `update-user-claims.sh` for updating existing users
   - Your user `dorin.baba@gmail.com` has been updated with all permissions

3. ✅ **Angular Token Service Updated**
   - Modified `token.service.ts` to read claims from JWT root level
   - Now handles both string `"true"` and boolean `true` values

4. ✅ **Backend Already Configured**
   - `Program.cs` already has authorization policies for all claims
   - Expects claims as `"true"` strings, which matches Keycloak output

## Test Right Now

### Step 1: Log Out and Back In

Your user now has the claims, but you need a fresh token:

1. Open http://localhost:4200
2. Log out completely
3. Log back in with your credentials

### Step 2: Verify Claims in JWT

1. Open Browser DevTools (F12)
2. Go to Application tab → Session Storage → `http://localhost:4200`
3. Find the key that contains your ID token or access token
4. Copy the token value
5. Go to https://jwt.io and paste the token
6. You should now see in the decoded payload:
   ```json
   {
     "can_view_entities": "true",
     "can_manage_entities": "true",
     "can_allocate_keys": "true",
     "can_send_emails": "true",
     "can_manage_announcements": "true",
     "can_manage_system_configuration": "true"
   }
   ```

### Step 3: Test Authorization

Try accessing features that require permissions. The Angular app should:
- Show/hide UI elements based on `tokenService.canManageEntities()`, etc.
- Backend API calls should succeed for authorized operations

## Common Issues

### "Claims not in my JWT"

**Solution:** The protocol mappers are configured for client `tems-angular-spa`. Make sure you're logging in through the Angular app, not directly to Keycloak.

### "Still getting 403 Forbidden"

**Check:**
1. Token is fresh (logged out and back in)
2. Backend is running and using Keycloak as authority
3. CORS is configured in backend
4. Authorization header is being sent (check Network tab)

### "Frontend not recognizing my permissions"

**Check:**
1. Clear browser session storage and log in again
2. Check for console errors in DevTools
3. Verify `token.service.ts` changes were saved
4. Token is not expired

## Files Changed

```
Infrastructure/Keycloak/
├── configure-keycloak.sh          (updated - adds test users with claims)
└── update-user-claims.sh          (new - update existing user claims)

Frontend/Tems/src/app/services/
└── token.service.ts               (updated - reads claims from JWT root)

Documentation/
├── KEYCLOAK_CLAIMS_SETUP.md       (new - comprehensive guide)
└── CLAIMS_QUICK_START.md          (this file)
```

## Update Other Users

To give permissions to other users:

```bash
cd Infrastructure/Keycloak

# Grant all permissions
./update-user-claims.sh "user@example.com" true true true true true true

# Grant only view access
./update-user-claims.sh "viewer@example.com" true false false false false false
```

**Important:** Users must log out and log back in after changes!

## Next Steps

1. Test the application with the updated claims
2. Create additional users with different permission sets
3. Verify that protected routes/components work as expected
4. Test backend API authorization with different permission levels

---

**Your user is ready!** Log out and log back in to get your new token with all permissions.
