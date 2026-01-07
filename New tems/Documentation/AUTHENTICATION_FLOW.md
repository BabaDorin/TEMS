# TEMS Authentication & Authorization Flow - COMPLETE WORKING GUIDE

**Last Updated:** January 7, 2026  
**Status:** ✅ AUTHENTICATION WORKING - ⚠️ AUTHORIZATION REQUIRES RE-LOGIN

## Important: After Configuration Changes
**You must log out and log back in** for Keycloak role mapper changes to take effect. Old tokens will not have the updated role claims.

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [Backend Authorization System](#backend-authorization-system)
3. [Component Configuration](#component-configuration)
4. [Role Management](#role-management)
5. [Troubleshooting](#troubleshooting)

## Architecture Overview

TEMS uses a **multi-layered authentication architecture** with Keycloak as the authorization layer and Duende IdentityServer as the identity provider.

```
┌─────────────────┐
│  Angular SPA    │
│  (Port 4200)    │
└────────┬────────┘
         │ 1. User clicks "Login"
         │ initCodeFlow({ kc_idp_hint: 'duende-idp' })
         ▼
┌─────────────────────────┐
│      Keycloak           │
│   (Port 8080)           │
│   Realm: tems           │
│   Client: tems-angular-spa │
└────────┬────────────────┘
         │ 2. Keycloak sees kc_idp_hint='duende-idp'
         │    Redirects to Duende via Identity Broker
         ▼
┌─────────────────────────┐
│  Duende IdentityServer  │
│   (Port 5001)           │
│   Client: keycloak-broker │
└────────┬────────────────┘
         │ 3. User authenticates with Duende
         │    (MongoDB user store)
         │    Username: admin / Password: Admin123!
         ▼
┌─────────────────────────┐
│  Duende IdentityServer  │
│  Returns auth code      │
└────────┬────────────────┘
         │ 4. Auth code sent to Keycloak
         ▼
┌─────────────────────────┐
│      Keycloak           │
│  Exchanges code for     │
│  tokens & user info     │
│  (syncMode: IMPORT)     │
└────────┬────────────────┘
         │ 5. Keycloak redirects back to Angular
         │    http://localhost:4200/callback?code=xxx
         ▼
┌─────────────────┐
│  Angular SPA    │
│  /callback      │
│  OAuthService   │
│  processes code │
└────────┬────────┘
         │ 6. Exchange code for tokens
         │    Store access_token & id_token
         │    Wait 100ms for processing
         ▼
┌─────────────────┐
│  Navigate to    │
│     /home       │
│  Authenticated! │
└─────────────────┘
```

## Component Configuration

### 1. Frontend (Angular SPA)

**File:** `Frontend/Tems/src/app/app.config.ts`

```typescript
export const authCodeFlowConfig: AuthConfig = {
  issuer: `${environment.keycloakUrl}/realms/${environment.keycloakRealm}`,
  // Points to Keycloak, NOT directly to Duende
  redirectUri: window.location.origin + '/callback',  // ✅ OAuth callback endpoint
  postLogoutRedirectUri: window.location.origin + '/home',
  clientId: 'tems-angular-spa',
  responseType: 'code',
  scope: 'openid profile email roles offline_access',
  showDebugInformation: !environment.production,
  useSilentRefresh: true,
  silentRefreshRedirectUri: window.location.origin + '/silent-refresh.html',
  sessionChecksEnabled: false,
  requireHttps: false,
};
```

**Key Points:**
- **Issuer:** Points to Keycloak realm (http://localhost:8080/realms/tems)
- **Redirect URI:** `/callback` - **CRITICAL** - where Keycloak returns after authentication
- **Post-Logout Redirect:** `/home` - where user goes after logout
- **Client ID:** `tems-angular-spa` - registered in Keycloak, NOT in Duende directly

### 2. Login Trigger

**File:** `Frontend/Tems/src/app/services/auth.service.ts`

```typescript
logIn() {
  this.oauthService.initCodeFlow('', { kc_idp_hint: 'duende-idp' });
}
```

**Key Points:**
- **kc_idp_hint: 'duende-idp'** - Critical parameter that tells Keycloak to use Duende as the identity provider
- Without this hint, Keycloak would show its own login form
- The hint name must match the Keycloak Identity Provider configuration

### 3. Keycloak Configuration

**Realm:** tems  
**Port:** 8080  
**Admin Credentials:** admin/admin

**Required Configuration in Keycloak:**

#### Client Configuration (`tems-angular-spa`)
- **Client Protocol:** openid-connect
- **Access Type:** public (SPA cannot keep secrets)
- **Valid Redirect URIs:**
  - `http://localhost:4200/callback` - **PRIMARY OAUTH CALLBACK**
  - `http://localhost:4200/home` - Post-logout redirect
  - `http://localhost:4200/silent-refresh.html` - Token refresh
  - `http://localhost:4200/*` - Wildcard for development
- **Web Origins:** `http://localhost:4200`
- **Enabled:** true

#### Identity Provider Configuration (`duende-idp`)
- **Provider Type:** OpenID Connect v1.0
- **Alias:** `duende-idp` (must match kc_idp_hint)
- **Authorization URL:** `http://localhost:5001/connect/authorize`
- **Token URL:** `http://localhost:5001/connect/token`
- **Logout URL:** `http://localhost:5001/connect/endsession`
- **User Info URL:** `http://localhost:5001/connect/userinfo`
- **Client ID:** `keycloak-broker`
- **Client Secret:** `keycloak-secret`
- **Client Authentication:** Client secret sent as post
- **Default Scopes:** `openid profile email roles`

### 4. Duende IdentityServer Configuration

**File:** `Backend/Tems/Tems.IdentityServer/Config/IdentityConfig.cs`

**Port:** 5001

#### Keycloak Broker Client
```csharp
new Client
{
    ClientId = "keycloak-broker",
    ClientName = "Keycloak Identity Broker",
    ClientSecrets = { new Secret("keycloak-secret".Sha256()) },
    AllowedGrantTypes = GrantTypes.Code,
    RequirePkce = false, // Keycloak doesn't send PKCE
    RequireClientSecret = true,
    
    RedirectUris = {
        "http://localhost:8080/realms/tems/broker/duende-idp/endpoint"
    },
    PostLogoutRedirectUris = { "http://localhost:8080/realms/tems" },
    AllowedCorsOrigins = { "http://localhost:8080" },
    
    AllowedScopes = {
        IdentityServerConstants.StandardScopes.OpenId,
        IdentityServerConstants.StandardScopes.Profile,
        IdentityServerConstants.StandardScopes.Email,
        "roles",
        "tems-api"
    }
}
```

#### User Store
- **Database:** MongoDB (Port 27017)
- **Collection:** users
- **Default Admin:**
  - Username: `admin`
  - Password: `Admin123!`
  - Email: `admin@tems.local`

### 5. Callback Handling

**File:** `Frontend/Tems/src/app/public/callback/callback.component.ts`

**Route:** `/callback` (registered in app-routing.module.ts)

**Implementation:**
```typescript
async ngOnInit() {
  await this.oauthService.loadDiscoveryDocumentAndTryLogin();
  
  // Wait for token processing
  setTimeout(() => {
    const hasToken = this.oauthService.hasValidAccessToken();
    const accessToken = this.oauthService.getAccessToken();
    
    if (hasToken && accessToken) {
      console.log('[Callback] Login successful, redirecting to home');
      this.router.navigate(['/home']);  // ✅ Navigate to /home, not /dashboard
    } else {
      console.error('[Callback] Login failed');
      this.router.navigate(['/login']);
    }
  }, 100);  // 100ms delay ensures tokens are processed
}
```

**Key Points:**
- Called after Keycloak redirects with authorization code
- Exchanges code for access_token and id_token
- **100ms delay** crucial for token processing
- On success: Navigate to `/home` (user's landing page)
- On failure: Navigate to `/login`

## Backend Authorization System

### Overview

The TEMS backend uses **claim-based authorization** with **role-based policies** implemented through ASP.NET Core's authorization framework and FastEndpoints.

### Architecture

```
┌─────────────────────┐
│   Frontend sends    │
│   JWT Access Token  │
│   in Authorization  │
│   Header            │
└──────────┬──────────┘
           │
           ▼
┌─────────────────────────────────────────┐
│   ASP.NET Core Middleware               │
│                                         │
│   1. JWT Bearer Authentication          │
│      - Validates token signature        │
│      - Checks issuer (Keycloak)        │
│      - Validates audience              │
│      - Extracts claims from token      │
└──────────┬──────────────────────────────┘
           │
           ▼
┌─────────────────────────────────────────┐
│   Authorization Middleware              │
│                                         │
│   2. Policy Evaluation                  │
│      - Maps endpoint to policy          │
│      - Checks user has required role    │
│      - Returns 403 if unauthorized     │
└──────────┬──────────────────────────────┘
           │
           ▼
┌─────────────────────────────────────────┐
│   FastEndpoint                          │
│                                         │
│   3. Endpoint Execution                 │
│      - Only if authorized               │
│      - Processes request                │
│      - Returns response                 │
└─────────────────────────────────────────┘
```

### Configuration (Program.cs)

#### 1. JWT Bearer Authentication

**File:** `Backend/Tems/Tems.Host/Program.cs`

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Keycloak issues tokens
        options.Authority = "http://localhost:8080/realms/tems";
        options.Audience = "account"; // Keycloak default audience
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudiences = new[] { "account", "tems-api" },
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            
            // CRITICAL: Keycloak uses 'roles' (plural) claim
            RoleClaimType = "roles"
        };
        
        options.RequireHttpsMetadata = false; // Only for dev
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();
                logger.LogError(context.Exception, "Authentication failed");
                return Task.CompletedTask;
            }
        };
    });
```

**Key Points:**
- **Authority:** Points to Keycloak realm (token issuer)
- **Audience:** Validates token intended audience
- **RoleClaimType:** Must be "roles" (plural) - Keycloak standard
- **ClockSkew:** Zero for strict token expiration validation

#### 2. Authorization Policies

```csharp
builder.Services.AddAuthorization(options =>
{
    // Asset Management - Full CRUD on assets, types, definitions, properties
    options.AddPolicy("CanManageAssets", policy =>
        policy.RequireRole("can_manage_assets"));
    
    // Ticket Management - Full CRUD on tickets, ticket types
    options.AddPolicy("CanManageTickets", policy =>
        policy.RequireRole("can_manage_tickets"));
        
    // Open Tickets - Create tickets only (basic user)
    options.AddPolicy("CanOpenTickets", policy =>
        policy.RequireRole("can_open_tickets"));
});
```

**Policy Details:**

| Policy Name | Required Role | Permissions |
|------------|---------------|-------------|
| CanManageAssets | `can_manage_assets` | Full CRUD on: Asset Types, Assets, Asset Definitions, Asset Properties |
| CanManageTickets | `can_manage_tickets` | Full CRUD on: Tickets, Ticket Types, Ticket Messages |
| CanOpenTickets | `can_open_tickets` | Create tickets using existing types |

**Important:** Users with `can_manage_tickets` role automatically have permission to open tickets.

### FastEndpoints Integration

#### Endpoint Configuration

Each endpoint declares its required policy using the `Policies()` method:

**Example:** Asset Type Endpoint

**File:** `Modules/AssetManagement/AssetManagement.API/Endpoints/AssetTypes/GetAllAssetTypeEndpoint.cs`

```csharp
public class GetAllAssetTypeEndpoint : Endpoint<GetAllAssetTypeCommand, GetAllAssetTypeResponse>
{
    public override void Configure()
    {
        Get("/asset-type");
        Policies("CanManageAssets"); // Requires CanManageAssets policy
    }

    public override async Task HandleAsync(GetAllAssetTypeCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await SendOkAsync(response, ct);
    }
}
```

#### All Protected Endpoints

**Asset Management Module (21 endpoints):**
- `/asset-type` - GET, POST, PUT, DELETE, GET by ID → `Policies("CanManageAssets")`
- `/asset` - GET, POST, PUT, DELETE, GET by ID → `Policies("CanManageAssets")`
- `/asset-definition` - GET, POST, PUT, DELETE, GET by ID → `Policies("CanManageAssets")`
- `/asset-property` - GET, POST, PUT, DELETE, GET by ID → `Policies("CanManageAssets")`

**Ticket Management Module (12 endpoints):**
- `/ticket-type` - GET, POST, PUT, DELETE, GET by ID → `Policies("CanManageTickets")`
- `/ticket` - GET, PUT, DELETE, GET by ID → `Policies("CanManageTickets")`
- `/ticket` - POST → `Policies("CanOpenTickets")` ⚠️ Note: Create uses different policy
- `/ticket/{id}/messages` - GET, POST → `Policies("CanManageTickets")`

### JWT Token Structure

When a user successfully authenticates, Keycloak issues a JWT token with the following structure:

```json
{
  "exp": 1704650400,
  "iat": 1704650100,
  "iss": "http://localhost:8080/realms/tems",
  "aud": ["account", "tems-api"],
  "sub": "f1234567-89ab-cdef-0123-456789abcdef",
  "typ": "Bearer",
  "azp": "tems-angular-spa",
  "session_state": "...",
  "acr": "1",
  "realm_access": {
    "roles": [
      "can_manage_assets",
      "can_manage_tickets",
      "can_open_tickets",
      "Admin"
    ]
  },
  "roles": [
    "can_manage_assets",
    "can_manage_tickets",
    "can_open_tickets",
    "Admin"
  ],
  "scope": "openid profile email",
  "email_verified": true,
  "name": "Administrator",
  "preferred_username": "admin",
  "email": "admin@tems.local"
}
```

**Critical Claims:**
- **`roles`** - Array of role names (configured via Role Mapper in Keycloak)
- **`realm_access.roles`** - Standard Keycloak role location (also checked)
- **`iss`** - Must match configured Authority
- **`aud`** - Must match configured ValidAudiences

### How Authorization Works

#### Request Flow

```
1. Frontend Request
   GET /asset-type
   Authorization: Bearer eyJhbGciOiJSUzI1Ni...
   
2. Authentication Middleware
   ✓ Validate token signature (using Keycloak public key)
   ✓ Check issuer: http://localhost:8080/realms/tems
   ✓ Check audience: account, tems-api
   ✓ Check expiration: not expired
   ✓ Extract claims including 'roles' claim
   
3. Set User Principal
   - ClaimsPrincipal created with all claims from token
   - Roles extracted from 'roles' claim (RoleClaimType)
   
4. Authorization Middleware
   - Endpoint requires: Policies("CanManageAssets")
   - Policy requires role: "can_manage_assets"
   - Check: Does user have role "can_manage_assets"?
   
5a. If Authorized (User has role)
   ✓ Call endpoint handler
   ✓ Return 200 OK with data
   
5b. If Not Authorized (User lacks role)
   ✗ Short-circuit request
   ✗ Return 403 Forbidden
```

#### Authorization Decision Logic

```csharp
// Simplified authorization logic
public bool IsAuthorized(ClaimsPrincipal user, string policyName)
{
    var policy = GetPolicy(policyName); // e.g., CanManageAssets
    var requiredRole = policy.RequiredRole; // e.g., can_manage_assets
    
    // Check if user has the required role in their 'roles' claim
    var userRoles = user.FindAll(RoleClaimType).Select(c => c.Value);
    
    return userRoles.Contains(requiredRole);
}
```

### Common Issues & Solutions

#### Issue 1: 403 Forbidden Despite Having Role

**Symptom:** Backend returns 403 even though user has correct role in Keycloak

**Possible Causes:**
1. **RoleClaimType mismatch** - Backend expects "roles" (plural) but token has "role" (singular)
2. **Role name mismatch** - Policy requires "can_manage_assets" but token has "CanManageAssets"
3. **Token not enriched** - Role mapper not configured in Keycloak
4. **Audience mismatch** - Token audience doesn't match ValidAudiences

**Solutions:**
```csharp
// 1. Verify RoleClaimType in Program.cs
RoleClaimType = "roles" // Must match token claim name

// 2. Verify role names match exactly (case-sensitive)
policy.RequireRole("can_manage_assets") // Underscore, lowercase

// 3. Check Keycloak Role Mapper (see Role Management section)

// 4. Verify ValidAudiences includes token audience
ValidAudiences = new[] { "account", "tems-api" }
```

#### Issue 2: Token Validation Fails

**Symptom:** 401 Unauthorized, authentication fails before authorization

**Check:**
```bash
# View backend logs for authentication errors
# Look for: "Authentication failed" or "Token validation failed"

# Common errors:
# - "IDX10214: Audience validation failed" → Audience mismatch
# - "IDX10205: Issuer validation failed" → Issuer mismatch  
# - "IDX10223: Lifetime validation failed" → Token expired
# - "IDX10503: Signature validation failed" → Invalid signature
```

**Solutions:**
- Verify `options.Authority` matches token `iss` claim
- Verify `ValidAudiences` includes token `aud` claim
- Check token not expired (ClockSkew = TimeSpan.Zero)
- Verify Keycloak public key accessible

#### Issue 3: Role Claim Not Found

**Symptom:** User authenticated but has no roles

**Diagnostic:**
```csharp
// Add logging to see what claims are in the token
options.Events = new JwtBearerEvents
{
    OnTokenValidated = context =>
    {
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILogger<Program>>();
        
        var claims = context.Principal.Claims
            .Select(c => $"{c.Type}: {c.Value}");
        
        logger.LogInformation("Token claims: {Claims}", 
            string.Join(", ", claims));
        
        return Task.CompletedTask;
    }
};
```

**Solution:** Ensure Keycloak Role Mapper configured (see Role Management section)

### Testing Authorization

#### Using Swagger

1. Start backend: `cd Backend/Tems/Tems.Host && dotnet run`
2. Navigate to: `http://localhost:5158/swagger`
3. Click "Authorize" button
4. Enter access token from browser (copy from localStorage or Network tab)
5. Try calling `/asset-type` endpoint
6. Should return 200 OK if authorized, 403 if not

#### Using cURL

```bash
# Get token (copy from browser localStorage: access_token)
TOKEN="eyJhbGciOiJSUzI1Ni..."

# Test endpoint
curl -H "Authorization: Bearer $TOKEN" \
     http://localhost:5158/asset-type

# Expected responses:
# 200 OK - Authorized, returns asset types
# 401 Unauthorized - Token invalid/expired
# 403 Forbidden - Token valid but lacks required role
```

#### Using Browser DevTools

1. Open browser DevTools (F12)
2. Go to Network tab
3. Filter: XHR
4. Click action that calls backend
5. Check request:
   - Headers → Authorization: Bearer ...
6. Check response:
   - Status: 200 OK (success) or 403 (forbidden)
7. If 403, check:
   - Token contains correct roles
   - RoleClaimType is "roles"
   - Role names match exactly

## Complete Authentication Flow

```
1. User clicks "Login with Duende IdentityServer"
   ↓
2. Frontend calls AuthService.logIn()
   ↓
3. OAuthService.initCodeFlow('', { kc_idp_hint: 'duende-idp' })
   ↓
4. Browser redirects to Keycloak:
   http://localhost:8080/realms/tems/protocol/openid-connect/auth
   ↓
5. Keycloak sees kc_idp_hint='duende-idp'
   Automatically redirects to Duende (no Keycloak login screen)
   ↓
6. Browser redirects to Duende IdentityServer:
   http://localhost:5001/connect/authorize
   ↓
7. Duende shows login form
   User enters: admin / Admin123!
   ↓
8. Duende validates credentials against MongoDB
   Returns authorization code to Keycloak
   ↓
9. Keycloak receives code at:
   http://localhost:8080/realms/tems/broker/duende-idp/endpoint
   ↓
10. Keycloak exchanges code with Duende for tokens
    Backend-to-backend call using host.docker.internal:5001
    ↓
11. Keycloak validates token issuer: http://host.docker.internal:5001
    ↓
12. Keycloak creates/updates user (syncMode: IMPORT, trustEmail: true)
    If user doesn't exist, auto-creates from Duende user info
    ↓
13. Keycloak creates session and redirects to Angular with auth code:
    http://localhost:4200/callback?code=xxx&state=yyy
    ↓
14. CallbackComponent processes the callback:
    - loadDiscoveryDocumentAndTryLogin()
    - Exchange code for access_token & id_token
    - Wait 100ms for token processing
    ↓
15. Tokens stored in browser storage
    ↓
16. Navigate to /home (user's landing page, NOT /dashboard)
    ↓
17. User is authenticated!
    AuthService.isAuthenticatedSubject.next(true)
```

## Logout Flow

```
1. User clicks "Logout"
   ↓
2. AuthService.signOut() is called
   ↓
3. oauthService.logOut() - Logs out from Keycloak/OAuth
   Redirects to Keycloak end_session_endpoint
   Keycloak terminates session
   ↓
4. localStorage.clear() - Clears all browser storage
   ↓
5. sessionStorage.clear() - Clears session data
   ↓
6. isAuthenticatedSubject.next(false) - Updates auth state
   ↓
7. window.location.href = '/home' - Hard refresh to reset UI
   ↓
8. Application reloads as unauthenticated user
   UI properly shows guest state
```

**Why Hard Refresh for Logout?**
- Ensures complete UI reset
- Clears all component state
- Re-initializes application with fresh auth state
- Prevents stale authenticated UI from showing
- More reliable than Angular soft navigation

## Token Management

### Access Token
- **Lifetime:** 15 minutes (900 seconds)
- **Issued by:** Keycloak
- **Contains:** User claims, roles, permissions
- **Used for:** API authentication

### Refresh Token
- **Lifetime:** 30 days sliding (2592000 seconds)
- **Usage:** Automatically refreshed via silent iframe
- **Endpoint:** `/silent-refresh.html`

### ID Token
- **Contains:** User identity information
- **Used by:** Frontend to get user details

## Critical Configuration Details

### 1. MongoDB Connection for Duende (Docker Container)
**Problem:** Duende container cannot connect to MongoDB using `localhost:27017`  
**Solution:** Use Docker service name `mongodb:27017` in connection string

**Files Updated:**
- `Backend/Tems/Tems.IdentityServer/appsettings.Development.json`
- `Backend/Tems/Tems.IdentityServer/appsettings.json`

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://mongodb:27017",
    "DatabaseName": "TemsDb"
  }
}
```

### 2. Keycloak Identity Provider Issuer
**Problem:** Token issuer mismatch - Keycloak expected `http://localhost:5001` but Duende returns `http://host.docker.internal:5001`  
**Solution:** Configure Keycloak IdP to use `host.docker.internal:5001` for backend calls

**Critical Settings:**
```json
{
  "issuer": "http://host.docker.internal:5001",
  "tokenUrl": "http://host.docker.internal:5001/connect/token",
  "userInfoUrl": "http://host.docker.internal:5001/connect/userinfo",
  "jwksUrl": "http://host.docker.internal:5001/.well-known/openid-configuration/jwks",
  "authorizationUrl": "http://localhost:5001/connect/authorize",
  "syncMode": "IMPORT",
  "trustEmail": true,
  "updateProfileFirstLoginMode": "on"
}
```

**Key Points:**
- **Backend URLs** (token, userinfo, jwks): Use `host.docker.internal:5001` (Keycloak container → Duende container)
- **Browser URL** (authorization): Use `localhost:5001` (User's browser → Duende via port mapping)
- **syncMode: IMPORT** - Auto-creates users in Keycloak from Duende
- **trustEmail: true** - Trusts email from Duende without verification

### 3. OAuth Redirect URIs
**Frontend Configuration:**
```typescript
redirectUri: window.location.origin + '/callback',
postLogoutRedirectUri: window.location.origin + '/home'
```

**Keycloak Client Valid Redirect URIs:**
- `http://localhost:4200/callback` (primary OAuth callback)
- `http://localhost:4200/home` (post-logout redirect)
- `http://localhost:4200/silent-refresh.html` (token refresh)
- `http://localhost:4200/*` (wildcard for development)

**Callback Flow:**
1. User logs in successfully
2. Keycloak redirects to `/callback` with authorization code
3. CallbackComponent exchanges code for tokens (100ms delay for processing)
4. On success: Navigate to `/home` (NOT `/dashboard`)
5. On failure: Navigate to `/login`

### 4. Logout Implementation
**Method:** `AuthService.signOut()`

**Process:**
1. Call `oauthService.logOut()` - Terminates OAuth session
2. Clear `localStorage` and `sessionStorage` - Removes all cached data
3. Update `isAuthenticatedSubject.next(false)` - Notifies subscribers
4. Use `window.location.href = '/home'` - Hard refresh (not Angular routing)

**Why Hard Refresh?**
- Ensures complete UI reset
- Clears all component state
- Re-initializes application with fresh auth state
- Prevents stale authenticated UI from showing

### 5. Guest Guard for Login Protection
**Purpose:** Prevent authenticated users from accessing login page

**File:** `Frontend/Tems/src/app/guards/guest.guard.ts`

**Implementation:**
- Async check with 50ms delay (ensures auth state is loaded)
- If logged in: Redirect to `/home`
- If not logged in: Allow access

**Routes Protected:**
- `/login`
- `/auth/login`

### 6. Environment Configuration
**Frontend:**
```typescript
export const environment = {
  production: false,
  apiUrl: "http://localhost:5158",
  clientUrl: "http://localhost:4200",
  identityServerUrl: "http://localhost:5001",
  keycloakUrl: "http://localhost:8080",
  keycloakRealm: "tems"
};
```

**Docker Compose:**
- MongoDB: `mongodb://mongodb:27017` (internal network)
- Duende: Port `5001:8080` (host:container)
- Keycloak: Port `8080:8080`

## Common Issues & Solutions

### Issue 1: Login Spinning Indefinitely
**Symptom:** Duende login page hangs after entering credentials  
**Root Cause:** MongoDB connection timeout  
**Error:** `Connection refused [::1]:27017` from Duende container  
**Solution:** Updated connection strings to `mongodb://mongodb:27017` (use Docker service name)  
**Status:** ✅ FIXED

### Issue 2: "Invalid username or password" Without Credentials
**Symptom:** Error shown immediately on Keycloak redirect  
**Root Cause:** Keycloak not configured to auto-create users from Duende  
**Solution:** Updated Keycloak IdP settings (trustEmail: true, syncMode: IMPORT)  
**Script:** `Infrastructure/Keycloak/fix-idp-settings.sh`  
**Status:** ✅ FIXED

### Issue 3: "Unexpected error when authenticating with identity provider"
**Symptom:** Error after successful Duende authentication  
**Root Cause:** Issuer URL mismatch  
**Error:** `Wrong issuer from token. Got: http://host.docker.internal:5001 expected: http://localhost:5001`  
**Solution:** Updated Keycloak IdP issuer to `host.docker.internal:5001`  
**Script:** `Infrastructure/Keycloak/fix-issuer.sh`  
**Status:** ✅ FIXED

### Issue 4: Authenticated Users Can Access /login
**Symptom:** Authenticated users see login page  
**Root Cause:** No guard preventing authenticated access to guest routes  
**Solution:** Created GuestGuard with async auth check  
**Applied to:** `/login` and `/auth/login` routes  
**Status:** ✅ FIXED

### Issue 5: Logout Doesn't Refresh UI
**Symptom:** After logout, authenticated menus still visible  
**Root Cause:** signOut() not properly terminating OAuth session  
**Solution:** Call oauthService.logOut(), clear all storage, use hard refresh  
**Status:** ✅ FIXED

### Issue 6: CORS Errors
**Symptom:** Browser blocks token exchange  
**Cause:** Missing CORS configuration  
**Fix:** Ensure AllowedCorsOrigins includes frontend URL in both Keycloak and Duende

### Issue 7: Invalid Redirect URI
**Symptom:** Error after Keycloak redirect  
**Cause:** Redirect URI mismatch  
**Fix:** Ensure Keycloak client has exact redirect URI (`http://localhost:4200/callback`)

## Current Status - WORKING ✅

**Last Updated:** January 7, 2026

### Working Features:
- ✅ Login flow: Frontend → Keycloak → Duende → MongoDB
- ✅ User authentication with admin/Admin123!
- ✅ Automatic user creation in Keycloak from Duende
- ✅ Token exchange and storage
- ✅ Redirect to /home after successful login (NOT /dashboard)
- ✅ Logout with full session cleanup and UI refresh
- ✅ Guest guard preventing authenticated users from accessing login page
- ✅ Proper OAuth callback handling with token validation
- ✅ Docker networking (host.docker.internal for inter-container communication)

### Key Fixes Applied:
1. **MongoDB Connection:** Changed from `localhost:27017` to `mongodb:27017` in Duende container
2. **Token Issuer:** Updated Keycloak IdP to accept `http://host.docker.internal:5001`
3. **User Sync:** Enabled `syncMode: IMPORT` and `trustEmail: true` in Keycloak
4. **Callback Route:** Added `/callback` route and updated redirect URI
5. **Logout:** Implemented proper OAuth logout with hard refresh
6. **Guest Guard:** Added async guard to protect login page from authenticated users
7. **Post-Login Redirect:** Changed from `/dashboard` to `/home`

### Configuration Scripts:
All fixes documented in `Infrastructure/Keycloak/`:
- `configure-keycloak.sh` - Initial realm and client setup
- `fix-idp-settings.sh` - Trust email and user sync settings
- `fix-issuer.sh` - Correct token issuer configuration
- `update-callback-uri.sh` - OAuth redirect URI updates

## How to Test

1. **Start Infrastructure:**
   ```bash
   cd Backend/Tems
   docker compose up -d
   ```

2. **Start Backend:**
   ```bash
   cd Backend/Tems/Tems.Host
   dotnet run
   ```

3. **Start Frontend:**
   ```bash
   cd Frontend/Tems
   npm start
   ```

4. **Test Login Flow:**
   - Navigate to http://localhost:4200
   - Click "Login with Duende IdentityServer"
   - You will be redirected through: Frontend → Keycloak → Duende
   - Duende login page will appear
   - Login with: `admin` / `Admin123!`
   - After successful login: Duende → Keycloak → Frontend `/callback`
   - You should be redirected to `/home` as an authenticated user

5. **Test Logout:**
   - Click "Logout" button
   - Session is terminated, storage cleared
   - Page refreshes to `/home` showing guest state
   - UI properly shows unauthenticated menus

6. **Test Guest Guard:**
   - While authenticated, try to access `/login`
   - Should be automatically redirected to `/home`

## Test Credentials

**Duende IdentityServer:**
- Admin: `admin` / `Admin123!` (admin@tems.local)
- User: `user` / `User123!` (user@tems.local)

**Keycloak Admin Console:**
- http://localhost:8080/admin
- Username: `admin`
- Password: `admin`

## Service Startup Order

1. **MongoDB** (Port 27017) - Database for user storage
2. **Duende IdentityServer** (Port 5001) - Identity Provider
3. **Keycloak** (Port 8080) - Authorization Layer
4. **Backend API** (Port 5158) - TEMS API
5. **Frontend** (Port 4200) - Angular SPA

All infrastructure services can be started with:
```bash
cd Backend/Tems
docker compose up -d
```

## Quick Reference

### URLs
- **Frontend:** http://localhost:4200
- **Backend API:** http://localhost:5158
- **Duende IdentityServer:** http://localhost:5001
- **Keycloak:** http://localhost:8080
- **Keycloak Admin:** http://localhost:8080/admin

### Key OAuth Endpoints
- **Authorization:** http://localhost:8080/realms/tems/protocol/openid-connect/auth
- **Token:** http://localhost:8080/realms/tems/protocol/openid-connect/token
- **UserInfo:** http://localhost:8080/realms/tems/protocol/openid-connect/userinfo
- **Logout:** http://localhost:8080/realms/tems/protocol/openid-connect/logout

### Key Routes
- `/login` - Login page (protected by GuestGuard)
- `/callback` - OAuth callback endpoint (handles token exchange)
- `/home` - Landing page after login/logout
- `/dashboard` - Main application dashboard (requires authentication)

### Important Files
- **OAuth Config:** `Frontend/Tems/src/app/app.config.ts`
- **Auth Service:** `Frontend/Tems/src/app/services/auth.service.ts`
- **Callback Component:** `Frontend/Tems/src/app/public/callback/callback.component.ts`
- **Guest Guard:** `Frontend/Tems/src/app/guards/guest.guard.ts`
- **Duende Config:** `Backend/Tems/Tems.IdentityServer/Config/IdentityConfig.cs`
- **MongoDB Settings:** `Backend/Tems/Tems.IdentityServer/appsettings.json`

## Role Management

**IMPORTANT:** Roles and permissions are managed entirely in Keycloak, NOT in Duende.

### Architecture
- **Duende:** Authentication only (username/password verification)
- **Keycloak:** Authorization (roles, permissions, claims)
- **MongoDB:** User credentials storage

### Available Roles
The following roles are defined in Keycloak:
- `Admin` - System administrator
- `can_manage_assets` - **Asset Management** - Create asset types, manage definitions, view/edit all assets
- `can_manage_tickets` - **Ticket Management** - Create ticket types, view all tickets, full ticket administration
- `can_open_tickets` - **Open Tickets** - Create tickets using existing types (basic user permission)

**Note:** Users with `can_manage_tickets` automatically have permission to open tickets as well.

### Managing User Roles

**Via Keycloak Admin Console:**
1. Navigate to http://localhost:8080/admin
2. Login with admin/admin
3. Select `tems` realm
4. Go to Users → Select user
5. Go to "Role mapping" tab
6. Assign/remove realm roles

**Via Script:**
```bash
# Migrate to new role system and assign roles to admin
cd Infrastructure/Keycloak
./migrate-to-new-roles.sh
```

### Creating New Roles

**Via Keycloak Admin Console:**
1. Navigate to Realm → Realm roles
2. Click "Create role"
3. Set role name (use snake_case, e.g., `can_manage_reports`)
4. Save

**Backend Integration:**
After creating a role in Keycloak, add it to the backend:

1. Update `Backend/Tems/Tems.Host/Program.cs`:
```csharp
options.AddPolicy("CanDoSomething", policy =>
    policy.RequireRole("can_do_something"));
```

2. Update frontend `Frontend/Tems/src/app/models/claims.ts`:
```typescript
export const CAN_DO_SOMETHING = "Can do something";
```

3. Update token service claim map in `Frontend/Tems/src/app/services/token.service.ts`:
```typescript
const claimMap: Record<string, string> = {
  [CAN_DO_SOMETHING]: 'can_do_something',
  // ... other claims
};
```

4. Add helper method in `Frontend/Tems/src/app/services/token.service.ts`:
```typescript
canDoSomething() {
  return this.hasClaim(CAN_DO_SOMETHING);
}
```

### What Duende Sends to Keycloak
Duende only sends these claims (NO ROLES):
- `sub` - User ID (from MongoDB)
- `username` - Username
- `email` - Email address
- `name` - Full name
- `phone_number` - Phone number (if present)

### What Keycloak Adds
Keycloak enriches the token with:
- `roles` - Array of role names assigned to the user
- `realm_access.roles` - Standard Keycloak role location
- Any custom claims/attributes configured in Keycloak

## Security Notes

- Never commit client secrets to source control
- Use environment variables for production
- Enable HTTPS for production deployments
- Rotate secrets regularly
- Monitor failed authentication attempts
- Implement rate limiting on auth endpoints

## Troubleshooting

### Checking Service Health

```bash
# Check all Docker containers
docker ps

# Check Keycloak logs
docker logs tems-keycloak

# Check Duende logs
docker logs tems-identity-server

# Check MongoDB logs
docker logs tems-mongodb

# Check MongoDB users
docker exec -it tems-mongodb mongosh TemsDb --eval "db.users.find({}, {username: 1, email: 1}).pretty()"

# Test MongoDB connection from Duende container
docker exec -it tems-identity-server curl mongodb:27017
```

### Common Problems

**Problem:** Login button does nothing  
**Check:** Browser console for errors  
**Solution:** Ensure Keycloak is running and realm is configured

**Problem:** Redirect to Keycloak but shows error  
**Check:** Keycloak Identity Provider configuration  
**Solution:** Run `Infrastructure/Keycloak/fix-idp-settings.sh`

**Problem:** Token issuer mismatch  
**Check:** Keycloak logs for issuer errors  
**Solution:** Run `Infrastructure/Keycloak/fix-issuer.sh`

**Problem:** Callback fails with 404  
**Check:** Angular routing configuration  
**Solution:** Ensure `/callback` route is registered in app-routing.module.ts

**Problem:** Logout doesn't clear UI  
**Check:** auth.service.ts signOut() method  
**Solution:** Ensure oauthService.logOut() is called and window.location.href is used

**Problem:** Can access /login while authenticated  
**Check:** GuestGuard is applied to route  
**Solution:** Add `canActivate: [GuestGuard]` to login routes

## Architecture Decisions

### Why Keycloak + Duende?
- **Keycloak:** Authorization layer, token management, role management, multi-tenant support
- **Duende:** Identity provider, custom user authentication (username/password), MongoDB user store
- **Separation:** Duende ONLY handles authentication; Keycloak manages ALL roles and permissions
- **Benefit:** Allows different identity providers per tenant while maintaining centralized authorization

**Important:** Duende does not send roles or custom claims. It only authenticates users and provides:
- `sub` (user ID)
- `username`
- `email`
- `name` (full name)
- `phone_number` (if present)

All roles and permissions are assigned and managed in Keycloak.

### Why /callback instead of /home for OAuth?
- **Separation of Concerns:** OAuth callback handling separate from landing page
- **Cleaner Code:** CallbackComponent dedicated to token exchange
- **Flexibility:** /home can be any landing page, callback always handles OAuth
- **Debugging:** Easier to troubleshoot OAuth flow issues

### Why Hard Refresh on Logout?
- **Complete Reset:** Ensures all component state is cleared
- **UI Consistency:** Guarantees guest state is properly displayed
- **Reliability:** More predictable than Angular navigation for auth state changes
- **User Experience:** Clear indication that logout completed successfully

### Why GuestGuard with Delay?
- **Auth State Loading:** 50ms delay ensures auth state is initialized
- **Race Condition:** Prevents guard checking before OAuth library initializes
- **User Experience:** Smooth redirect without flashing login page

### Why syncMode: IMPORT?
- **Auto User Creation:** Users don't need to be pre-created in Keycloak
- **Single Source of Truth for Authentication:** Duende/MongoDB is the authoritative user store for credentials
- **Centralized Authorization:** Keycloak is the authoritative source for roles and permissions
- **Simplified Management:** User credentials in MongoDB (Duende), roles/permissions in Keycloak

### Why host.docker.internal?
- **Container-to-Container:** Keycloak container calls Duende container
- **Docker Networking:** `host.docker.internal` resolves to host machine
- **Port Mapping:** Duende runs on port 8080 in container, mapped to 5001 on host
- **Token Validation:** Issuer must match what Duende returns in tokens
docker exec tems-mongodb mongosh --eval "db.users.find().pretty()"

# Test Keycloak OIDC discovery
curl http://localhost:8080/realms/tems/.well-known/openid-configuration

# Test Duende OIDC discovery
curl http://localhost:5001/.well-known/openid-configuration
```
