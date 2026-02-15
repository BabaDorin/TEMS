# User Management, Authentication & Authorization

> **Source of truth** for how user identity, authentication, and authorization work across every layer of the TEMS application.

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [Component Responsibilities](#2-component-responsibilities)
3. [Authentication Flow (Step-by-Step)](#3-authentication-flow-step-by-step)
4. [Authorization System](#4-authorization-system)
5. [User Data Models](#5-user-data-models)
6. [User CRUD Lifecycle](#6-user-crud-lifecycle)
7. [Token Structure & Claims](#7-token-structure--claims)
8. [Infrastructure Setup](#8-infrastructure-setup)
9. [Frontend Auth Integration](#9-frontend-auth-integration)
10. [API Endpoints Reference](#10-api-endpoints-reference)
11. [Common Pitfalls & Debugging](#11-common-pitfalls--debugging)

---

## 1. Architecture Overview

```
┌──────────────────┐     OIDC/PKCE      ┌──────────────────┐     OIDC Broker      ┌──────────────────────┐
│   Angular SPA    │ ──────────────────► │     Keycloak      │ ──────────────────► │  Duende IdentityServer │
│  (localhost:4200)│ ◄────────────────── │  (localhost:8080) │ ◄────────────────── │   (localhost:5001)     │
│                  │    JWT (access +    │   Realm: tems     │   keycloak-broker    │                        │
│                  │    id + refresh)    │                   │   client              │                        │
└──────┬───────────┘                    └────────┬──────────┘                      └──────────┬─────────────┘
       │ Bearer token                            │ Admin API                                  │
       │                                         │ (role CRUD)                                │ Password validation
       ▼                                         │                                            │ + profile claims
┌──────────────────┐                             │                                            │
│   .NET Backend   │ ◄───────────────────────────┘                                            │
│  (localhost:5158)│                                                                          │
│  FastEndpoints   │                                                                          │
└──────┬───────────┘                                                                          │
       │                                                                                      │
       ▼                                                                                      ▼
┌──────────────────────────────────────────────────────────────────────────────────────────────────┐
│                              MongoDB  (localhost:27017 / TemsDb)                                  │
│                                                                                                  │
│  Collection: users (backend profiles)          Collection: users (Duende identity store)         │
│  { _id: GUID, Name, Email, KeycloakId,         { _id: ObjectId, Username, PasswordHash,          │
│    IdentityProviderId, TenantIds, ... }           Email, FullName, IsActive, ... }                │
└──────────────────────────────────────────────────────────────────────────────────────────────────┘
```

### Port Map

| Service               | Port  | Purpose                              |
|------------------------|-------|--------------------------------------|
| Angular SPA            | 4200  | Frontend client                      |
| .NET Backend (Host)    | 5158  | REST API (FastEndpoints + MediatR)   |
| Duende IdentityServer  | 5001  | Identity Provider (password auth)    |
| Keycloak               | 8080  | Authorization server / token issuer  |
| MongoDB                | 27017 | Database (TemsDb)                    |

---

## 2. Component Responsibilities

### 2.1 MongoDB (Backend Database)

**Role: User profile storage only. Knows nothing about roles.**

- Stores two separate user collections within `TemsDb`:
  - **Backend users** — application-level profiles used by the TEMS API (`UserManagement` module)
  - **Duende users** — identity records used by Duende IdentityServer for password authentication
- Does **not** store roles, permissions, or authorization data.
- Backend user records reference external systems via `KeycloakId` and `IdentityProviderId` fields.

### 2.2 Keycloak

**Role: Authorization server, token issuer, and identity provider broker.**

Keycloak is the **front door** for all client authentication. It:

- Hosts the **`tems` realm** with the public OIDC client `tems-angular-spa`.
- Issues **JWT access tokens, ID tokens, and refresh tokens** to the Angular SPA via Authorization Code + PKCE.
- Manages all **realm roles** (`can_manage_assets`, `can_manage_tickets`, `can_open_tickets`, `can_manage_users`). Roles are embedded in tokens via a custom protocol mapper.
- Brokers authentication to **Duende IdentityServer** as an external identity provider (`duende-idp`). When a user starts login, Keycloak auto-redirects to Duende, which performs the actual password check.
- **Auto-links** brokered users on first login via a custom `auto-link-first-login` authentication flow (no manual account linking required).
- Synchronizes user attributes from Duende on every login (`syncMode: FORCE`).
- Provides an **Admin REST API** used by the .NET backend for role CRUD, user CRUD, and role assignment queries.

### 2.3 Duende IdentityServer

**Role: Identity provider (authentication layer). Handles username/password validation.**

Duende IdentityServer:

- Runs as a standalone service connected to MongoDB.
- Validates **username + password** against the `users` collection in MongoDB using ASP.NET `PasswordHasher`.
- Issues claims (`sub`, `username`, `email`, `name`) back to Keycloak during the brokered login flow.
- Does **not** manage or issue role claims — roles are entirely Keycloak's responsibility.
- Exposes two OIDC clients:
  - **`tems-angular-spa`** — (unused directly by the SPA; exists for legacy/compatibility)
  - **`keycloak-broker`** — confidential client used by Keycloak's `duende-idp` identity provider to authenticate users (client secret: `keycloak-secret`).
- Seeds an initial admin user (`admin` / `Admin123!`, `admin@tems.local`) on startup via `SeedData.cs`.

### 2.4 .NET Backend (Tems.Host)

**Role: API server. Validates JWT tokens issued by Keycloak. Enforces authorization policies.**

The backend:

- Validates every incoming request's `Authorization: Bearer <token>` against Keycloak's realm (`http://localhost:8080/realms/tems`).
- Expands the `roles` JSON array claim into individual `ClaimsIdentity` role claims (workaround for .NET 9's `JsonWebTokenHandler` which doesn't auto-expand JSON arrays).
- Enforces four named **authorization policies** on endpoints.
- Orchestrates user CRUD across all three systems (MongoDB + Keycloak + Duende) via the `UserManagement` module.
- Fetches roles from Keycloak at runtime when returning user data (since roles are not stored in MongoDB).

### 2.5 Angular SPA (Frontend)

**Role: Client application. Initiates OIDC login flow, attaches Bearer tokens, checks roles for UI visibility.**

The frontend:

- Uses `angular-oauth2-oidc` library to connect to Keycloak via Authorization Code + PKCE.
- Passes `kc_idp_hint: 'duende-idp'` during login to bypass the Keycloak login screen and go directly to Duende.
- Attaches the Keycloak-issued access token to every API request via `authInterceptor`.
- Checks role claims from the ID token to conditionally show/hide sidebar menu items and UI elements.
- Provides route guards for certain pages (e.g., `canManageAssetsGuard` for locations).

---

## 3. Authentication Flow (Step-by-Step)

```
User clicks "Login"
        │
        ▼
[1] Angular calls oauthService.initCodeFlow('', { kc_idp_hint: 'duende-idp' })
        │
        ▼
[2] Browser redirects to Keycloak:
    GET http://localhost:8080/realms/tems/protocol/openid-connect/auth
      ?client_id=tems-angular-spa
      &response_type=code
      &scope=openid profile email roles offline_access
      &redirect_uri=http://localhost:4200/callback
      &code_challenge=<PKCE_CHALLENGE>
      &code_challenge_method=S256
      &kc_idp_hint=duende-idp
        │
        ▼
[3] Keycloak sees kc_idp_hint=duende-idp → auto-redirects to Duende:
    GET http://host.docker.internal:5001/connect/authorize
      ?client_id=keycloak-broker
      &redirect_uri=http://localhost:8080/realms/tems/broker/duende-idp/endpoint
      &response_type=code
      &scope=openid profile email
        │
        ▼
[4] Duende shows its login page. User enters username + password.
        │
        ▼
[5] Duende validates credentials against MongoDB users collection:
    MongoDbResourceOwnerPasswordValidator → PasswordHasher.VerifyHashedPassword()
        │
        ▼
[6] Duende issues authorization code back to Keycloak broker endpoint:
    → Keycloak exchanges code for tokens from Duende (using keycloak-broker client secret).
    → Duende's MongoDbProfileService returns claims: sub, username, email, name.
        │
        ▼
[7] Keycloak performs first-broker-login flow:
    a) auto-link-first-login flow runs:
       - idp-create-user-if-unique: creates Keycloak user if not exists
       - idp-auto-link: auto-links to existing Keycloak user if found
    b) On subsequent logins (syncMode: FORCE): syncs email/name from Duende.
        │
        ▼
[8] Keycloak issues authorization code back to Angular SPA:
    302 → http://localhost:4200/callback?code=<AUTH_CODE>&state=<STATE>
        │
        ▼
[9] Angular CallbackComponent handles the redirect:
    oauthService.loadDiscoveryDocumentAndTryLogin()
    → Exchanges auth code for tokens at Keycloak token endpoint (with PKCE verifier).
        │
        ▼
[10] Keycloak returns three tokens:
     - Access Token (JWT): contains roles, sub, preferred_username, aud, etc.
     - ID Token (JWT): contains roles, sub, email, name, etc.
     - Refresh Token (opaque): used for silent token renewal.
        │
        ▼
[11] Angular stores tokens in sessionStorage, navigates to /home.
     AuthService emits isAuthenticated$ = true, starts token refresh timer.
        │
        ▼
[12] On every API call, authInterceptor attaches:
     Authorization: Bearer <access_token>
        │
        ▼
[13] Backend validates the JWT:
     - Issuer: http://localhost:8080/realms/tems
     - Audience: "account" (also accepts "tems-api")
     - Signature: verified against Keycloak's JWKS endpoint
     - OnTokenValidated: expands "roles" JSON array into individual claims
        │
        ▼
[14] Backend checks authorization policy (e.g., CanManageAssets requires "can_manage_assets" role claim).
```

### Token Refresh

- `AuthService` runs a timer every **30 seconds** checking token expiry.
- If the token expires within **60 seconds**, it proactively refreshes using the refresh token.
- The `authInterceptor` also handles **401 responses** by attempting a refresh before redirecting to login.
- On refresh failure, the user is logged out and redirected to `/login`.

---

## 4. Authorization System

### 4.1 Available Roles

Four realm-level roles exist in Keycloak:

| Keycloak Role Name     | Frontend Constant        | Backend Policy Name  | Purpose                                          |
|-------------------------|--------------------------|----------------------|--------------------------------------------------|
| `can_manage_assets`     | `CAN_MANAGE_ASSETS`      | `CanManageAssets`    | Full CRUD on assets, types, definitions, locations |
| `can_manage_tickets`    | `CAN_MANAGE_TICKETS`     | `CanManageTickets`   | Full CRUD on tickets and ticket types             |
| `can_open_tickets`      | `CAN_OPEN_TICKETS`       | `CanOpenTickets`     | Create and view own tickets                       |
| `can_manage_users`      | `CAN_MANAGE_USERS`       | `CanManageUsers`     | Full CRUD on users and role assignments           |

### 4.2 Default User Role Assignments

Provisioned by `configure-keycloak.sh`:

| User     | Password    | Roles                                                                        |
|----------|-------------|------------------------------------------------------------------------------|
| `admin`  | `Admin123!` | `can_manage_assets`, `can_manage_tickets`, `can_open_tickets`, `can_manage_users` |
| `user`   | `User123!`  | `can_open_tickets`                                                            |

### 4.3 Backend Authorization Policies

Defined in `Tems.Host/Program.cs`:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageAssets", p => p.RequireClaim("roles", "can_manage_assets"));
    options.AddPolicy("CanManageTickets", p => p.RequireClaim("roles", "can_manage_tickets"));
    options.AddPolicy("CanOpenTickets", p => p.RequireClaim("roles", "can_open_tickets"));
    options.AddPolicy("CanManageUsers", p => p.RequireClaim("roles", "can_manage_users"));
});
```

JWT Bearer configuration:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/tems";
        options.Audience = "account";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RoleClaimType = "roles",
            NameClaimType = "preferred_username",
            ValidAudiences = new[] { "account", "tems-api" },
        };
    });
```

**Critical: Role Claim Expansion**

.NET 9's `JsonWebTokenHandler` does not automatically expand JSON array claims. The `OnTokenValidated` event manually expands the `roles` claim:

```csharp
options.Events = new JwtBearerEvents
{
    OnTokenValidated = context =>
    {
        // Reads the "roles" claim (a JSON array like ["can_manage_assets","can_manage_tickets"])
        // and adds each value as an individual Claim of type "roles"
        // so that policy.RequireClaim("roles", "can_manage_assets") works correctly.
    }
};
```

### 4.4 Frontend Authorization

**TokenService** (`token.service.ts`) checks the **ID token** claims:

```
1. claims.roles[]              ← top-level array (added by realm-roles-mapper)
2. claims.realm_access.roles[] ← standard Keycloak location (fallback)
3. claims[roleName]            ← direct boolean claim (last fallback)
```

The mapping from frontend constants to Keycloak role names:

```typescript
const claimMap: Record<string, string> = {
    'Can manage assets':  'can_manage_assets',
    'Can manage tickets': 'can_manage_tickets',
    'Can open tickets':   'can_open_tickets',
    'Can manage users':   'can_manage_users',
};
```

**Note:** `canOpenTickets()` returns `true` if the user has **either** `can_open_tickets` OR `can_manage_tickets`.

### 4.5 Route Guards

| Guard                    | Type            | Protects                  | Behavior on Fail            |
|--------------------------|-----------------|---------------------------|-----------------------------|
| `AuthGuard`              | Class-based     | `/profile/view`           | Redirects to Keycloak login |
| `GuestGuard`             | Class-based     | `/login`                  | Redirects to `/home` if already logged in |
| `canManageAssetsGuard`   | Functional      | `/locations/view`, `/locations/:id` | Redirects to `/unauthorized` |
| `canManageTicketsGuard`  | Functional      | *(exists but not wired)*  | Redirects to `/unauthorized` |
| `canOpenTicketsGuard`    | Functional      | *(exists but not wired)*  | Redirects to `/unauthorized` |

**Important:** The `/administration/users` route has **no frontend guard**. Access control relies on:
1. Sidebar menu visibility (hidden if `canManageUsers()` is `false`).
2. Backend 403 response if the user lacks the `can_manage_users` role.

---

## 5. User Data Models

Three distinct user representations exist across the system:

### 5.1 Duende IdentityServer User (MongoDB — identity store)

**Collection:** `users` (accessed by Duende IdentityServer only)  
**Entity:** `Tems.Common.Identity.User`

```csharp
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }                    // MongoDB ObjectId

    public string Username { get; set; }
    public string PasswordHash { get; set; }           // BCrypt via PasswordHasher
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = new();   // ALWAYS EMPTY — roles in Keycloak
    public Dictionary<string, string> Claims { get; set; } = new(); // ALWAYS EMPTY
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

**Purpose:** Password hash storage and basic profile attributes for authentication. This is the credential store that Duende validates against. The `Roles` and `Claims` fields are intentionally kept empty — all role management happens in Keycloak.

### 5.2 Backend API User (MongoDB — application store)

**Collection:** `users` (accessed by `UserManagement` module)  
**Entity:** `UserManagement.Infrastructure.Entities.User`

```csharp
public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString(); // GUID string

    public string Name { get; set; }
    public string Email { get; set; }
    public string? AvatarUrl { get; set; }
    public string? IdentityProviderId { get; set; }   // Duende user's ObjectId
    public List<string> TenantIds { get; set; } = new();
    public string? KeycloakId { get; set; }           // Keycloak user UUID
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

**Purpose:** Application-level user profile for the TEMS business logic. Contains cross-references to both Keycloak (`KeycloakId`) and Duende (`IdentityProviderId`). Does **not** store roles.

### 5.3 Keycloak User (Keycloak internal database)

Managed entirely through Keycloak Admin REST API. Key fields:

| Field              | Description                                |
|--------------------|--------------------------------------------|
| `id`               | Keycloak UUID (stored as `KeycloakId` in backend user) |
| `username`         | Login name                                 |
| `email`            | Email address                              |
| `firstName`        | First name                                 |
| `lastName`         | Last name                                  |
| `enabled`          | Whether the user can log in                |
| `federatedIdentities` | Links to Duende IdP (auto-created on first login) |
| `realmRoleMappings` | Assigned realm roles                      |

### 5.4 Relationship Between the Three User Records

```
                        ┌─────────────────────────────┐
                        │     Keycloak User            │
                        │   id: "kc-uuid-1234"         │
                        │   username: "admin"           │
                        │   roles: [can_manage_assets,  │
                        │           can_manage_tickets,  │
                        │           can_open_tickets,    │
                        │           can_manage_users]    │
                        └──────┬──────────┬────────────┘
                               │          │
                    KeycloakId │          │ federatedIdentity
                               │          │ (idp: duende-idp)
                               ▼          ▼
┌──────────────────────────┐     ┌──────────────────────────┐
│  Backend API User        │     │  Duende User              │
│  id: "guid-5678"         │     │  id: "objectid-abcd"      │
│  KeycloakId: "kc-uuid-1234" │  │  Username: "admin"        │
│  IdentityProviderId:     │     │  PasswordHash: "..."       │
│    "objectid-abcd"       │     │  Email: "admin@tems.local" │
│  Name: "admin"           │     │  Roles: [] (empty)         │
│  Email: "admin@tems.local" │   └──────────────────────────┘
└──────────────────────────┘
```

When creating a user, all three records are created. When deleting, all three are removed.

---

## 6. User CRUD Lifecycle

### 6.1 Create User

**Endpoint:** `POST /users` (requires `CanManageUsers` policy)

**Handler:** `CreateUserCommandHandler`

The creation process touches all three systems in sequence:

```
1. Create user in Duende IdentityServer
   → POST http://localhost:5001/api/users
   → Duende hashes the password and stores a User record in MongoDB
   → Returns the new Duende user ID (ObjectId string)

2. Create user in Keycloak
   → POST http://localhost:8080/admin/realms/tems/users
   → Body: { username, email, firstName, lastName, enabled: true,
             credentials: [{ type: password, value: "...", temporary: false }] }
   → Returns the new Keycloak user UUID (from Location header)

3. Assign roles in Keycloak
   → POST http://localhost:8080/admin/realms/tems/users/{kcUserId}/role-mappings/realm
   → Body: array of role representation objects

4. Create user in MongoDB (backend)
   → Stores: { Id: new GUID, Name, Email, KeycloakId: kcUserId,
               IdentityProviderId: duendeUserId, TenantIds: [...] }
```

### 6.2 Delete User

**Endpoint:** `DELETE /users/{Id}` (requires `CanManageUsers` policy)

**Handler:** `DeleteUserCommandHandler`

```
1. Look up backend user by Id → get KeycloakId and IdentityProviderId
2. Delete from Keycloak → DELETE /admin/realms/tems/users/{keycloakId}
3. Delete from Duende → DELETE http://localhost:5001/api/users/{identityProviderId}
4. Unallocate all assets assigned to this user
5. Delete from MongoDB (backend users collection)
```

### 6.3 Update User Roles

**Endpoint:** `PUT /users/{Id}/roles` (requires `CanManageUsers` policy)

**Handler:** `UpdateUserRolesCommandHandler`

Roles are managed exclusively in Keycloak. The handler performs a diff:

```
1. Look up backend user → get KeycloakId
2. Fetch current roles from Keycloak for this user
3. Compute diff:
   - rolesToAdd    = requested roles − current roles
   - rolesToRemove = current roles − requested roles
4. If rolesToAdd is not empty:
   → POST /admin/realms/tems/users/{kcId}/role-mappings/realm (add new roles)
5. If rolesToRemove is not empty:
   → DELETE /admin/realms/tems/users/{kcId}/role-mappings/realm (remove old roles)
```

The user must **log out and log back in** (or wait for token refresh) to get updated roles in their JWT.

### 6.4 Get All Users

**Endpoint:** `GET /users` (requires `CanManageUsers` policy)

**Handler:** `GetAllUsersQueryHandler`

```
1. Fetch all users from MongoDB (backend collection)
2. For each user with a KeycloakId, fetch roles from Keycloak in parallel batches
   → GET /admin/realms/tems/users/{kcId}/role-mappings/realm
3. Filter out system roles: offline_access, uma_authorization, default-roles-tems
4. Return users with their roles attached
```

### 6.5 Get Single User

**Endpoint:** `GET /users/{id}` (requires `CanManageUsers` policy)

**Handler:** `GetUserByIdQueryHandler`

Same as above but for a single user — fetches from MongoDB then enriches with Keycloak roles.

### 6.6 Get Or Create Profile (Login Sync)

**Endpoint:** `GET /profile` (AllowAnonymous — but requires a valid JWT)

**Handler:** `GetOrCreateProfileQueryHandler`

This is called on every app load when a user is authenticated:

```
1. Extract sub, name, email, picture from JWT claims
2. Look up backend user by IdentityProviderId = sub
3. If not found → create a new backend user with:
   - IdentityProviderId = sub (from JWT)
   - Lookup Keycloak user by username to get KeycloakId
   - Name, Email from JWT claims
4. If found → update Name/Email if changed, sync KeycloakId if missing
5. Return the user profile
```

This ensures that even users created outside the admin UI (e.g., direct Keycloak registration) get a backend profile.

### 6.7 Get Available Roles

**Endpoint:** `GET /roles` (requires `CanManageUsers` policy)

**Handler:** `GetRolesQueryHandler`

```
1. Fetch all realm roles from Keycloak
   → GET /admin/realms/tems/roles
2. Filter out system roles: offline_access, uma_authorization, default-roles-tems
3. Return role names
```

---

## 7. Token Structure & Claims

### 7.1 Access Token (Keycloak-issued JWT)

Key claims present in the access token:

| Claim                | Example Value                          | Source      | Used By         |
|----------------------|----------------------------------------|-------------|-----------------|
| `iss`                | `http://localhost:8080/realms/tems`     | Keycloak    | Backend (issuer validation) |
| `sub`                | `kc-uuid-1234`                          | Keycloak    | Profile sync    |
| `aud`                | `["account", "tems-api"]`              | Keycloak    | Backend (audience validation) |
| `preferred_username` | `admin`                                 | Keycloak    | Backend (NameClaimType) |
| `email`              | `admin@tems.local`                      | Keycloak    | Profile sync    |
| `name`               | `admin`                                 | Keycloak    | Profile sync    |
| `roles`              | `["can_manage_assets", "can_manage_tickets", ...]` | Keycloak (realm-roles-mapper) | Backend (authorization policies) |
| `realm_access`       | `{ "roles": ["can_manage_assets", ...] }` | Keycloak    | Frontend (fallback) |
| `azp`                | `tems-angular-spa`                      | Keycloak    | —               |
| `scope`              | `openid profile email roles offline_access` | Keycloak | —               |
| `picture`            | URL string                              | Keycloak    | Profile avatar  |

### 7.2 ID Token (Keycloak-issued JWT)

Contains the same role claims. The frontend reads roles from the ID token (via `oauthService.getIdentityClaims()`), while the backend reads from the access token.

### 7.3 Protocol Mappers (Keycloak)

Two custom mappers on the `tems-angular-spa` client:

1. **realm-roles-mapper** (`oidc-usermodel-realm-role-mapper`):
   - Claim name: `roles` (top-level, not nested under `realm_access`)
   - Multivalued: `true`
   - Added to: ID token, access token, userinfo
   - This is why `claims.roles` exists as a top-level array

2. **tems-api-audience** (`oidc-audience-mapper`):
   - Adds `tems-api` to the `aud` claim of the access token
   - Required because the backend validates `ValidAudiences = ["account", "tems-api"]`

---

## 8. Infrastructure Setup

### 8.1 Docker Compose Services

Defined in `Backend/Tems/compose.yaml`:

| Service           | Image                          | Port     | Notes                                           |
|--------------------|--------------------------------|----------|------------------------------------------------|
| `mongodb`          | `mongo:7.0`                    | 27017    | Database `TemsDb`, health-checked via `mongosh` |
| `keycloak`         | `quay.io/keycloak/keycloak:23.0` | 8080   | `start-dev` mode, admin/admin                   |
| `identity-server`  | Custom Dockerfile               | 5001→8080 | Depends on `mongodb` health                    |

All services share the `tems-network` bridge network.

### 8.2 Keycloak Provisioning (`configure-keycloak.sh`)

The script runs 12 steps:

| Step | Action |
|------|--------|
| 1 | Wait for Keycloak readiness (up to 120s) |
| 2 | Obtain admin token via `admin-cli` password grant on `master` realm |
| 3 | Create `tems` realm |
| 4 | Create `tems-angular-spa` public client (PKCE, S256, redirects to `localhost:4200`) |
| 5 | Create `auto-link-first-login` authentication flow with `idp-create-user-if-unique` + `idp-auto-link` (both `ALTERNATIVE`) |
| 6 | Create `duende-idp` identity provider (OIDC, pointing to `host.docker.internal:5001`, `authenticateByDefault: true`, `syncMode: FORCE`, `firstBrokerLoginFlowAlias: auto-link-first-login`) |
| 7 | Create `duende-browser-flow` custom browser flow (only `identity-provider-redirector` execution, `REQUIRED`, `defaultProvider: duende-idp`) |
| 8 | Set `duende-browser-flow` as the realm's default browser flow |
| 9 | Create four realm roles (`can_manage_assets`, `can_manage_tickets`, `can_open_tickets`, `can_manage_users`) |
| 10 | Create admin user with all 4 roles (batch assignment with retry + verification) |
| 11 | Create regular user (`user`) with `can_open_tickets` only |
| 12 | Configure protocol mappers (realm-roles-mapper + audience-mapper) |

### 8.3 `kc_idp_hint` Flow

The Angular SPA passes `kc_idp_hint: 'duende-idp'` when initiating login. Combined with the custom browser flow (step 7–8), this ensures:

- The user **never sees Keycloak's login page**.
- Keycloak immediately redirects to Duende IdentityServer.
- From the user's perspective, they only interact with one login screen (Duende's).

### 8.4 Backend Configuration (`appsettings.Development.json`)

```json
{
    "MongoDb": {
        "ConnectionString": "mongodb://localhost:27017/",
        "DatabaseName": "TemsDb"
    },
    "Keycloak": {
        "BaseUrl": "http://localhost:8080",
        "Realm": "tems",
        "AdminUsername": "admin",
        "AdminPassword": "admin"
    },
    "Cors": {
        "AllowedOrigins": "http://localhost:4200;http://localhost:4201"
    }
}
```

### 8.5 Frontend Configuration (`environment.ts`)

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

---

## 9. Frontend Auth Integration

### 9.1 OAuth Configuration

Defined in `app.config.ts`:

```typescript
export const authCodeFlowConfig: AuthConfig = {
    issuer: 'http://localhost:8080/realms/tems',    // Keycloak realm
    redirectUri: window.location.origin + '/callback',
    postLogoutRedirectUri: window.location.origin + '/home',
    clientId: 'tems-angular-spa',                   // Public client
    responseType: 'code',                           // Authorization Code flow
    scope: 'openid profile email roles offline_access',
    disablePKCE: false,                             // PKCE enabled
    requireHttps: false,                            // Dev mode
};
```

### 9.2 AuthService

`auth.service.ts` — manages the OAuth lifecycle:

| Method              | Purpose                                                    |
|---------------------|------------------------------------------------------------|
| `configure()`       | Loads OIDC discovery document, attempts auto-login          |
| `logIn()`           | Initiates code flow with `kc_idp_hint: 'duende-idp'`      |
| `signOut()`         | Logs out from Keycloak, clears storage, redirects to `/home` |
| `isLoggedIn()`      | Checks if access token is valid                            |
| `getAccessToken()`  | Returns current access token string                        |
| `forceTokenRefresh()` | Manually triggers token refresh                          |

The service monitors OAuth events and proactively refreshes tokens before expiry.

### 9.3 Auth Interceptor

`auth.interceptor.ts` — HTTP interceptor that:

1. Attaches `Authorization: Bearer <token>` to every outgoing request.
2. On **401** response: attempts token refresh, retries the request. On failure: clears storage, redirects to `/login`.
3. On **403** response: shows "Insufficient permissions" snackbar.

### 9.4 TokenService

`token.service.ts` — role-checking service:

| Method              | Returns `true` when                          |
|---------------------|----------------------------------------------|
| `canManageAssets()`  | User has `can_manage_assets` role             |
| `canManageTickets()` | User has `can_manage_tickets` role            |
| `canOpenTickets()`   | User has `can_open_tickets` OR `can_manage_tickets` |
| `canManageUsers()`   | User has `can_manage_users` role              |

### 9.5 ClaimService (Legacy)

`claim.service.ts` — reads claims once at construction. Used by many components via dependency injection. Properties: `canManageAssets`, `canManageTickets`, `canOpenTickets`.

**Caveat:** Since it reads claims at construction time, changes to roles require a full page reload (or re-injection) to take effect.

### 9.6 Menu Visibility

`menu.service.ts` builds the sidebar dynamically based on roles:

| Menu Section       | Visibility Condition            |
|--------------------|---------------------------------|
| Assets             | Always shown                    |
| Locations          | `canManageAssets()` required    |
| Technical Support  | Always shown                    |
| User Management    | `canManageUsers()` required     |

The menu rebuilds when `isAuthenticated$` changes (e.g., after login or token refresh).

### 9.7 Callback Component

After Keycloak redirects back to `/callback`:
1. `oauthService.loadDiscoveryDocumentAndTryLogin()` exchanges the auth code for tokens.
2. Calls `userService.getProfile()` which hits `GET /profile` on the backend to sync/create the user profile.
3. Navigates to `/home`.

---

## 10. API Endpoints Reference

### UserManagement Module

| Method | Path                       | Policy          | Handler                        | Description                        |
|--------|----------------------------|-----------------|--------------------------------|------------------------------------|
| GET    | `/profile`                  | AllowAnonymous  | `GetOrCreateProfileQueryHandler` | Get/create user profile from JWT claims |
| POST   | `/users`                    | CanManageUsers  | `CreateUserCommandHandler`       | Create user in all 3 systems       |
| DELETE | `/users/{Id}`               | CanManageUsers  | `DeleteUserCommandHandler`       | Delete user from all 3 systems     |
| GET    | `/users`                    | CanManageUsers  | `GetAllUsersQueryHandler`        | List all users with Keycloak roles |
| GET    | `/users/{id}`               | CanManageUsers  | `GetUserByIdQueryHandler`        | Get single user with roles         |
| PUT    | `/users/{Id}/roles`         | CanManageUsers  | `UpdateUserRolesCommandHandler`  | Update user's Keycloak roles       |
| GET    | `/roles`                    | CanManageUsers  | `GetRolesQueryHandler`           | List all available realm roles     |
| GET    | `/users/{userId}/assets`    | CanManageAssets | `GetUserAssetsQueryHandler`      | Get assets assigned to user        |
| GET    | `/users/{userId}/assets/count` | CanManageUsers | `GetUserAssetCountQueryHandler` | Count assets assigned to user      |

### Keycloak Admin API (used internally by backend)

| Purpose                | Endpoint                                                          |
|------------------------|------------------------------------------------------------------|
| Authenticate           | `POST /realms/master/protocol/openid-connect/token`               |
| Create user            | `POST /admin/realms/tems/users`                                   |
| Delete user            | `DELETE /admin/realms/tems/users/{id}`                             |
| Get users by username  | `GET /admin/realms/tems/users?username={name}&exact=true`         |
| Get user roles         | `GET /admin/realms/tems/users/{id}/role-mappings/realm`           |
| Assign roles           | `POST /admin/realms/tems/users/{id}/role-mappings/realm`          |
| Remove roles           | `DELETE /admin/realms/tems/users/{id}/role-mappings/realm`        |
| Get all realm roles    | `GET /admin/realms/tems/roles`                                    |
| Get role by name       | `GET /admin/realms/tems/roles/{roleName}`                         |

The `KeycloakClient` authenticates via the `admin-cli` client using `admin`/`admin` credentials on the `master` realm. It caches the access token and refreshes on expiry.

### Duende IdentityServer API (used internally by backend)

| Purpose        | Endpoint                                    |
|----------------|---------------------------------------------|
| Create user    | `POST http://localhost:5001/api/users`       |
| Delete user    | `DELETE http://localhost:5001/api/users/{id}` |

---

## 11. Common Pitfalls & Debugging

### 11.1 Stale Keycloak IDs After Container Recreation

**Problem:** When the Keycloak container is recreated (`docker compose down && up`), all Keycloak user UUIDs change. Backend MongoDB records still hold old `KeycloakId` values, causing role fetches to fail (404 from Keycloak).

**Fix:** The `GetOrCreateProfileQueryHandler` syncs `KeycloakId` on every profile fetch by looking up the Keycloak user by username. For bulk fixes, manually update MongoDB:

```javascript
// In mongosh
db.users.updateMany({}, { $set: { KeycloakId: null } });
// Then have each user log in again to trigger profile sync
```

### 11.2 Missing Roles After Clean Restart

**Problem:** `configure-keycloak.sh` creates users and assigns roles, but the role assignment can silently fail if the Keycloak API returns errors (e.g., race conditions during startup).

**Fix:** The script now includes batch role assignment with retry logic and post-assignment verification. If roles are still missing, assign manually:

```bash
# Get admin token
TOKEN=$(curl -s -X POST "http://localhost:8080/realms/master/protocol/openid-connect/token" \
  -d "client_id=admin-cli&grant_type=password&username=admin&password=admin" | jq -r '.access_token')

# Get user ID
USER_ID=$(curl -s -H "Authorization: Bearer $TOKEN" \
  "http://localhost:8080/admin/realms/tems/users?username=admin&exact=true" | jq -r '.[0].id')

# Get role representation
ROLE=$(curl -s -H "Authorization: Bearer $TOKEN" \
  "http://localhost:8080/admin/realms/tems/roles/can_manage_assets")

# Assign role
curl -X POST -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  "http://localhost:8080/admin/realms/tems/users/$USER_ID/role-mappings/realm" \
  -d "[$ROLE]"
```

### 11.3 403 Forbidden on API Calls

**Diagnosis checklist:**

1. **Is the role in the JWT?** Decode the access token at jwt.io and check the `roles` array.
2. **Is the role assigned in Keycloak?** Check via Admin UI or API.
3. **Is the token fresh?** The user must log out and back in after role changes.
4. **Is the roles claim expanding correctly?** Check backend logs for the `OnTokenValidated` event. If the `roles` claim is a JSON array string but not being expanded, the workaround in `Program.cs` may have been bypassed.

### 11.4 User Not Found After Login

**Problem:** Frontend shows no profile or "user not found" after authentication.

**Cause:** The `GET /profile` endpoint extracts `sub` from the JWT and looks up `IdentityProviderId`. If the user was created before the profile-sync mechanism was added, the backend record might not exist.

**Fix:** `GetOrCreateProfileQueryHandler` auto-creates the backend user on first profile request. If it still fails, check:
- Is the JWT `sub` claim present?
- Is MongoDB reachable from the backend?
- Is the Keycloak user lookup by username succeeding?

### 11.5 Token Refresh Failures

**Symptoms:** User gets logged out unexpectedly, 401 errors.

**Causes:**
- Keycloak session expired (default: 30 min idle, 10 hr max).
- Refresh token was invalidated (Keycloak restart, session revocation).
- CORS blocking the token refresh request.

**Fix:** Check browser console for `[AuthService] Token refresh failed` errors. Ensure Keycloak's session timeouts are appropriate for the use case.

### 11.6 Dual User Collections in MongoDB

The `TemsDb.users` collection contains documents from **both** Duende IdentityServer and the backend API, but they have different schemas:

| Field          | Duende User         | Backend User          |
|----------------|---------------------|-----------------------|
| `_id`          | ObjectId            | GUID string           |
| `Username`     | ✅                  | ❌ (uses `Name`)      |
| `PasswordHash` | ✅                  | ❌                     |
| `KeycloakId`   | ❌                  | ✅                     |
| `TenantIds`    | ❌                  | ✅                     |

Both coexist in the same collection. MongoDB's schema-less nature allows this, but queries must be aware of which type they're reading.

---

## Appendix A: File Locations Quick Reference

| Concern                      | File Path                                                              |
|------------------------------|------------------------------------------------------------------------|
| Keycloak provisioning        | `Infrastructure/Keycloak/configure-keycloak.sh`                        |
| Role mapper config           | `Infrastructure/Keycloak/configure-role-mapper.sh`                     |
| Docker Compose               | `Backend/Tems/compose.yaml`                                            |
| Duende Program.cs            | `Backend/Tems/Tems.IdentityServer/Program.cs`                          |
| Duende clients config        | `Backend/Tems/Tems.IdentityServer/Config/IdentityConfig.cs`            |
| Duende profile service       | `Backend/Tems/Tems.IdentityServer/UserStore/MongoDbProfileService.cs`  |
| Duende password validator    | `Backend/Tems/Tems.IdentityServer/UserStore/MongoDbResourceOwnerPasswordValidator.cs` |
| Duende seed data             | `Backend/Tems/Tems.IdentityServer/Data/SeedData.cs`                    |
| Duende User entity           | `Backend/Tems/Tems.Common/Identity/User.cs`                            |
| Backend JWT + policies       | `Backend/Tems/Tems.Host/Program.cs`                                    |
| Backend appsettings (dev)    | `Backend/Tems/Tems.Host/appsettings.Development.json`                  |
| Tenant middleware             | `Backend/Tems/Tems.Host/Middleware/TenantMiddleware.cs`                |
| Backend User entity          | `Backend/Tems/Modules/UserManagement/.../Entities/User.cs`             |
| User repository              | `Backend/Tems/Modules/UserManagement/.../Repositories/UserRepository.cs` |
| Keycloak client              | `Backend/Tems/Modules/UserManagement/.../Keycloak/KeycloakClient.cs`   |
| IdentityServer client        | `Backend/Tems/Modules/UserManagement/.../IdentityServer/IdentityServerClient.cs` |
| All user endpoints           | `Backend/Tems/Modules/UserManagement/.../Endpoints/*.cs`               |
| All user handlers            | `Backend/Tems/Modules/UserManagement/.../Handlers/*.cs`                |
| Frontend OAuth config        | `Frontend/Tems/src/app/app.config.ts`                                  |
| Frontend auth service        | `Frontend/Tems/src/app/services/auth.service.ts`                       |
| Frontend token service       | `Frontend/Tems/src/app/services/token.service.ts`                      |
| Frontend claim constants     | `Frontend/Tems/src/app/models/claims.ts`                               |
| Frontend auth interceptor    | `Frontend/Tems/src/app/auth.interceptor.ts`                            |
| Frontend menu service        | `Frontend/Tems/src/app/services/menu.service.ts`                       |
| Frontend route guards        | `Frontend/Tems/src/app/guards/*.ts`                                    |
| Frontend routing             | `Frontend/Tems/src/app/app-routing.module.ts`                          |
| Frontend environment         | `Frontend/Tems/src/environments/environment.ts`                        |

---

*Last updated: January 2026*
