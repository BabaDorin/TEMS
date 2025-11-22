# TEMS Authentication & Authorization Setup

## Architecture Overview

TEMS uses a multi-layered authentication and authorization architecture with Keycloak as the central authentication gateway, Duende IdentityServer for user management, and JWT tokens for API authorization.

```
┌─────────────────────────────────────────────────────────────────────┐
│                         TEMS Architecture                             │
└─────────────────────────────────────────────────────────────────────┘

    ┌──────────────┐
    │   Angular    │
    │   Frontend   │ (http://localhost:4200)
    │              │
    └──────┬───────┘
           │
           │ 1. User clicks "Login"
           │ 2. Redirects to Keycloak
           │
    ┌──────▼───────────────────────────────────────────────────┐
    │                                                            │
    │                      Keycloak                              │
    │              (Identity Broker)                             │
    │          http://localhost:8080                             │
    │                                                            │
    │  • Realm: tems                                            │
    │  • Client: tems-angular-spa                               │
    │  • Protocol: OpenID Connect (OIDC)                        │
    │  • Grant Type: Authorization Code + PKCE                  │
    │                                                            │
    └──────┬───────────────────────────────────────────────────┘
           │
           │ 3. Keycloak federates to Duende
           │    for user authentication
           │
    ┌──────▼─────────────────────────────────────────────┐
    │                                                      │
    │            Duende IdentityServer                     │
    │           (User Store & Authentication)              │
    │           http://localhost:5001                      │
    │                                                      │
    │  • User Store: MongoDB                              │
    │  • Users Collection: users                          │
    │  • Client: keycloak-broker                          │
    │  • Registration API: /api/Account/register          │
    │                                                      │
    └──────┬───────────────────────────────────────────────┘
           │
           │ 4. Returns user claims and tokens
           │
    ┌──────▼─────────────────────────────────────────────┐
    │                                                      │
    │              Keycloak Token                          │
    │            (ID Token + Access Token)                 │
    │                                                      │
    │  • Includes user roles                              │
    │  • Includes custom claims                           │
    │  • Signed by Keycloak                               │
    │                                                      │
    └──────┬───────────────────────────────────────────────┘
           │
           │ 5. Frontend stores tokens
           │ 6. Sends Access Token in API requests
           │
    ┌──────▼─────────────────────────────────────────────┐
    │                                                      │
    │               TEMS Backend API                       │
    │           http://localhost:14721                     │
    │                                                      │
    │  • Validates JWT from Keycloak                      │
    │  • Checks authorization policies                    │
    │  • Enforces claim-based permissions                 │
    │                                                      │
    └──────┬───────────────────────────────────────────────┘
           │
           │ 7. Queries data
           │
    ┌──────▼─────────────────────────────────────────────┐
    │                                                      │
    │                  MongoDB                             │
    │           mongodb://localhost:27017                  │
    │                                                      │
    │  • Database: TemsDb                                 │
    │  • Collections: users, equipment, rooms, etc.       │
    │                                                      │
    └────────────────────────────────────────────────────┘
```

## Authentication Flow

### Registration Flow

1. **User Registration**
   ```
   User → Frontend → IdentityServer API
   POST /api/Account/register
   {
     "email": "user@example.com",
     "password": "SecurePass123"
   }
   ```

2. **User Created**
   - IdentityServer creates user in MongoDB
   - Password is hashed using ASP.NET Identity PasswordHasher
   - Default role "User" is assigned
   - User can now login

### Login Flow (OIDC Authorization Code Flow with PKCE)

1. **Initiate Login**
   - User clicks "Sign in" button
   - Frontend calls `authService.logIn()`
   - Redirects to Keycloak authorization endpoint

2. **Keycloak Authentication**
   ```
   https://localhost:8080/realms/tems/protocol/openid-connect/auth
   ?client_id=tems-angular-spa
   &redirect_uri=http://localhost:4200/callback
   &response_type=code
   &scope=openid profile email roles offline_access
   &code_challenge=...
   &code_challenge_method=S256
   ```

3. **Duende Federation**
   - If user not in Keycloak session, Keycloak federates to Duende
   - User enters credentials on Duende login page
   - Duende validates against MongoDB
   - Duende returns user profile and claims to Keycloak

4. **Token Exchange**
   - Keycloak issues authorization code
   - Frontend exchanges code for tokens at `/token` endpoint
   - Receives:
     - `id_token`: User identity claims
     - `access_token`: For API authorization
     - `refresh_token`: For token renewal

5. **Token Storage**
   - Tokens stored by `angular-oauth2-oidc` library
   - Automatically attached to API requests via `auth.interceptor.ts`

### API Request Flow

1. **Request with Token**
   ```http
   GET /api/equipment
   Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

2. **Token Validation**
   - Backend validates JWT signature using Keycloak's public key (JWKS)
   - Checks token expiration
   - Verifies issuer is Keycloak
   - Extracts claims (user ID, roles, custom claims)

3. **Authorization Check**
   - Checks authorization policies (e.g., `CanViewEntities`)
   - Verifies required claims exist
   - Examples:
     ```csharp
     [Authorize(Policy = "CanViewEntities")]
     [Authorize(Policy = "CanManageEntities")]
     ```

4. **Response**
   - If authorized: Returns data
   - If unauthorized: Returns 403 Forbidden
   - If unauthenticated: Returns 401 Unauthorized

### Token Refresh Flow

1. **Token Expiration**
   - Access tokens expire after 15 minutes
   - Frontend detects expiration via `angular-oauth2-oidc`

2. **Silent Refresh**
   - Library automatically uses refresh token
   - Calls Keycloak `/token` endpoint with `grant_type=refresh_token`
   - Receives new access token

3. **Refresh Token Rotation**
   - Refresh tokens valid for 30 days (sliding window)
   - Updated on each refresh

### Logout Flow

1. **Initiate Logout**
   - User clicks "Logout"
   - Frontend calls `authService.signOut()`

2. **Token Revocation**
   - Revokes tokens at Keycloak
   - Clears local storage

3. **Redirect**
   - Redirects to `/login`

## Components

### Frontend (Angular)

**Technology**: Angular 20 (Standalone Components)

**Authentication Library**: `angular-oauth2-oidc`

**Key Files**:
- `app.config.ts`: OAuth configuration
- `auth.service.ts`: Authentication service
- `auth.interceptor.ts`: Adds Bearer token to requests
- `login.component.ts`: Login UI
- `register.component.ts`: Registration UI
- `callback.component.ts`: OAuth callback handler

**Configuration** (`app.config.ts`):
```typescript
export const authCodeFlowConfig: AuthConfig = {
  issuer: 'http://localhost:8080/realms/tems',
  redirectUri: window.location.origin + '/callback',
  postLogoutRedirectUri: window.location.origin + '/login',
  clientId: 'tems-angular-spa',
  responseType: 'code',
  scope: 'openid profile email roles offline_access',
  showDebugInformation: true,
  useSilentRefresh: true,
  requireHttps: false
};
```

**Auth Guards**:
- `can-view-entities.guard.ts`: Checks `can_view_entities` claim
- `can-manage-entities.guard.ts`: Checks `can_manage_entities` claim
- Similar guards for other permissions

### Keycloak (Identity Broker)

**Technology**: Keycloak 23.0

**Deployment**: Docker container

**Port**: 8080

**Admin Credentials**: admin/admin

**Realm Configuration**:
- **Realm**: tems
- **Client ID**: tems-angular-spa
- **Client Type**: Public (no client secret)
- **Valid Redirect URIs**: http://localhost:4200/*
- **Web Origins**: http://localhost:4200
- **Grant Type**: Authorization Code
- **PKCE**: Required (S256)

**Identity Provider Federation**:
- **Provider**: Duende IdentityServer
- **Alias**: duende-idp
- **Protocol**: OIDC
- **Authorization URL**: http://localhost:5001/connect/authorize
- **Token URL**: http://localhost:5001/connect/token
- **User Info URL**: http://localhost:5001/connect/userinfo
- **Client ID**: keycloak-broker
- **Client Secret**: keycloak-secret
- **Scopes**: openid profile email roles tems-api

**Protocol Mappers**:
- Role mapper: Maps `role` attribute to token
- Custom claim mappers for TEMS permissions

**Management**:
- Configuration via REST API (see `configure-keycloak.sh`)
- Manual configuration at http://localhost:8080/admin

### Duende IdentityServer

**Technology**: Duende IdentityServer 7.0 (with Community License)

**Framework**: ASP.NET Core 8

**Port**: 5001

**User Store**: MongoDB (users collection)

**Key Components**:
- `Config/IdentityConfig.cs`: Clients, scopes, resources configuration
- `UserStore/MongoDbProfileService.cs`: User profile retrieval
- `UserStore/MongoDbResourceOwnerPasswordValidator.cs`: Password validation
- `Controllers/AccountController.cs`: Registration API
- `Data/SeedData.cs`: Seed admin user on startup

**Clients**:

1. **tems-angular-spa** (Legacy - not used with Keycloak)
   - Direct Angular client
   - Authorization Code + PKCE

2. **keycloak-broker** (Active)
   - Keycloak federation client
   - Authorization Code + PKCE
   - Client Secret: keycloak-secret

**API Scopes**:
- `openid`: OpenID Connect
- `profile`: User profile
- `email`: Email claim
- `roles`: User roles
- `tems-api`: TEMS API access

**Custom Claims**:
- `can_view_entities`
- `can_manage_entities`
- `can_allocate_keys`
- `can_send_emails`
- `can_manage_announcements`
- `can_manage_system_configuration`

**Registration API**:
```http
POST /api/Account/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password",
  "username": "optional",
  "fullName": "optional"
}
```

### Backend API

**Technology**: ASP.NET Core 8 + FastEndpoints

**Port**: 14721

**Authentication**: JWT Bearer tokens from Keycloak

**Authorization**: Claim-based policies

**Configuration** (`Program.cs`):
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:8080/realms/tems";
        options.Audience = "account";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "role"
        };
        options.RequireHttpsMetadata = false;
    });
```

**Authorization Policies**:
```csharp
options.AddPolicy("CanViewEntities", policy =>
    policy.RequireClaim("can_view_entities", "true"));

options.AddPolicy("CanManageEntities", policy =>
    policy.RequireClaim("can_manage_entities", "true"));

// ... more policies
```

**CORS Configuration**:
- Allows requests from http://localhost:4200
- Allows credentials (cookies, authorization headers)
- Allows all methods and headers

### MongoDB

**Technology**: MongoDB 7.0

**Port**: 27017

**Database**: TemsDb

**Collections**:
- `users`: User accounts and credentials
- `equipment`: Equipment records
- `rooms`: Room information
- `personnel`: Personnel records
- Other application data

**User Document Structure**:
```json
{
  "_id": "ObjectId",
  "username": "string",
  "passwordHash": "string",
  "email": "string",
  "fullName": "string",
  "phoneNumber": "string",
  "roles": ["User", "Admin"],
  "claims": {
    "can_view_entities": "true",
    "can_manage_entities": "true"
  },
  "isActive": true,
  "createdAt": "ISODate"
}
```

## Security Features

### Password Security
- Passwords hashed using ASP.NET Identity PasswordHasher
- Uses PBKDF2 with HMAC-SHA256
- Salt automatically generated per user
- Minimum length: 6 characters

### Token Security
- **Access Token**: Short-lived (15 minutes)
- **Refresh Token**: Long-lived (30 days, sliding window)
- **ID Token**: Contains user identity claims
- All tokens signed with RS256 algorithm
- JWKS endpoint for public key verification

### PKCE (Proof Key for Code Exchange)
- Protects authorization code flow
- Code challenge method: S256 (SHA-256)
- Prevents authorization code interception attacks

### HTTPS (Production)
- Keycloak should use HTTPS in production
- `requireHttps: false` only for local development
- Update configuration for production deployment

### CORS
- Strict origin checking
- Credentials allowed only from trusted origins
- Pre-flight requests supported

### Session Management
- Session timeout: 30 minutes idle
- Maximum session: 10 hours
- Silent token refresh via iframe
- Session checks enabled

## Configuration Files

### Frontend

**`src/environments/environment.ts`**:
```typescript
export const environment = {
  production: false,
  apiUrl: "http://localhost:14721",
  clientUrl: "http://localhost:4200",
  identityServerUrl: "http://localhost:5001",
  keycloakUrl: "http://localhost:8080",
  keycloakRealm: "tems"
};
```

### Backend API

**`Tems.Host/appsettings.Development.json`**:
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "TemsDb"
  },
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/tems",
    "Realm": "tems"
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:4200"
  }
}
```

### IdentityServer

**`Tems.IdentityServer/appsettings.json`**:
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "TemsDb"
  },
  "Duende": {
    "LicenseKey": ""
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:4200;http://localhost:8080"
  }
}
```

### Docker Compose

**`Backend/Tems/compose.yaml`**:
```yaml
services:
  mongodb:
    image: mongo:7.0
    ports:
      - "27017:27017"

  keycloak:
    image: quay.io/keycloak/keycloak:23.0
    command: start-dev
    ports:
      - "8080:8080"
    environment:
      - KEYCLOAK_ADMIN=admin
      - KEYCLOAK_ADMIN_PASSWORD=admin
```

## Troubleshooting

### Common Issues

#### 1. "CORS policy error"
**Cause**: Frontend making requests to backend without proper CORS headers

**Solution**:
- Verify backend CORS configuration includes frontend URL
- Check `AllowedOrigins` in appsettings.json
- Ensure backend is using `app.UseCors()` middleware

#### 2. "401 Unauthorized" on API requests
**Cause**: Token invalid, expired, or not sent

**Solution**:
- Check token in browser developer tools (Application → Local Storage)
- Verify `auth.interceptor.ts` is adding Authorization header
- Check token expiration
- Try logging out and logging back in

#### 3. "403 Forbidden" on API requests
**Cause**: User lacks required claims for the operation

**Solution**:
- Check user's claims in MongoDB
- Verify authorization policy in backend
- Update user claims if needed:
  ```javascript
  db.users.updateOne(
    { email: "user@example.com" },
    { $set: { "claims.can_manage_entities": "true" } }
  )
  ```

#### 4. Keycloak not starting
**Cause**: Port conflict or configuration error

**Solution**:
```bash
# Check if port 8080 is in use
lsof -i :8080

# View Keycloak logs
docker logs tems-keycloak

# Restart Keycloak
docker restart tems-keycloak
```

#### 5. "Issuer validation failed"
**Cause**: Token issuer doesn't match configured authority

**Solution**:
- Check Keycloak URL in environment.ts
- Verify backend Authority configuration
- Ensure Keycloak realm name is correct ("tems")

#### 6. Silent refresh fails
**Cause**: Refresh token expired or iframe blocked

**Solution**:
- Check if refresh token is present
- Verify `silent-refresh.html` exists in frontend
- Check browser console for errors
- May need to login again if refresh token expired

### Debugging Tips

#### View Token Contents
Use https://jwt.io to decode tokens and inspect claims

#### Check User Claims
```bash
mongosh
use TemsDb
db.users.findOne({ email: "user@example.com" })
```

#### Monitor Authentication Events
Check logs:
```bash
# Backend
tail -f /tmp/tems-backend.log

# IdentityServer
tail -f /tmp/tems-identityserver.log

# Keycloak
docker logs -f tems-keycloak
```

#### Test Token Validation
```bash
# Get access token from browser storage
# Test API endpoint
curl -H "Authorization: Bearer YOUR_TOKEN" \
  http://localhost:14721/api/equipment
```

## Production Deployment

### Security Checklist

- [ ] Enable HTTPS for all services
- [ ] Use strong passwords for Keycloak admin
- [ ] Rotate client secrets regularly
- [ ] Set `requireHttps: true` in OAuth config
- [ ] Configure production MongoDB with authentication
- [ ] Use environment variables for sensitive configuration
- [ ] Enable logging and monitoring
- [ ] Configure proper CORS origins
- [ ] Review and update authorization policies
- [ ] Implement rate limiting
- [ ] Enable Keycloak's security features (brute force detection, etc.)
- [ ] Use production-grade SSL certificates
- [ ] Configure proper session timeouts
- [ ] Enable database backups
- [ ] Set up intrusion detection

### Configuration Updates

1. **Frontend**:
   - Update `environment.prod.ts` with production URLs
   - Enable `production: true`
   - Set `requireHttps: true` in auth config

2. **Backend**:
   - Update `appsettings.Production.json`
   - Use secure connection strings
   - Enable detailed logging

3. **Keycloak**:
   - Use `start` command instead of `start-dev`
   - Configure SSL certificate
   - Enable all security features
   - Set up realm-specific branding

4. **IdentityServer**:
   - Add production Duende license key
   - Configure SSL
   - Use production MongoDB connection

5. **MongoDB**:
   - Enable authentication
   - Configure replica set for high availability
   - Enable encryption at rest

## Additional Resources

- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [Duende IdentityServer Documentation](https://docs.duendesoftware.com/identityserver/v7)
- [angular-oauth2-oidc Documentation](https://github.com/manfredsteyer/angular-oauth2-oidc)
- [OpenID Connect Specification](https://openid.net/connect/)
- [OAuth 2.0 PKCE](https://oauth.net/2/pkce/)

## Support

For issues or questions:
1. Check logs in `/tmp/tems-*.log`
2. Review this documentation
3. Check Docker container status: `docker ps`
4. Verify all services are running on correct ports
5. Review Keycloak admin console for configuration

---

*Last Updated: November 22, 2025*
