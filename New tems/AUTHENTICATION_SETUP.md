# Authentication & Authorization Setup

This document describes the authentication and authorization implementation added to the TEMS project using Duende IdentityServer.

## Architecture Overview

The system now uses a three-tier architecture:

1. **Duende IdentityServer** (port 5001) - OAuth2/OIDC provider for authentication
2. **TEMS API** (port 14721) - ASP.NET Core backend with JWT authentication
3. **Angular Frontend** (port 4200) - SPA using Authorization Code + PKCE flow

## Components Added

### Backend

#### 1. Tems.IdentityServer
A new ASP.NET Core 9.0 project providing OAuth2/OIDC authentication services.

**Key Files:**
- `Program.cs` - Main configuration with MongoDB integration
- `Config/IdentityConfig.cs` - OIDC clients, scopes, and resources
- `UserStore/MongoDbProfileService.cs` - Provides user profile data to tokens
- `UserStore/MongoDbResourceOwnerPasswordValidator.cs` - Password validation
- `Data/SeedData.cs` - Seeds default admin user
- `Views/Account/` - Login/logout UI

**Features:**
- MongoDB-backed user storage
- Custom claims support (6 permission claims)
- MVC-based login UI
- Development seed data

#### 2. Tems.Common/Identity
Shared user model between IdentityServer and API.

**Files:**
- `User.cs` - MongoDB user document with roles and claims

#### 3. Tems.Host Updates
API configured for JWT Bearer authentication.

**Changes:**
- Added `Microsoft.AspNetCore.Authentication.JwtBearer` 9.0.0
- Configured JWT authentication with IdentityServer as authority
- Added 6 authorization policies for claim-based access control
- Enabled CORS for frontend
- Updated Equipment Management endpoints to require `CanManageEntities` policy

### Frontend

#### Dependencies Added
- `angular-oauth2-oidc` 17.0.2 - OIDC client library

#### Updated Files
- `app.config.ts` - OIDC configuration and providers
- `services/auth.service.ts` - Refactored to use OAuthService
- `auth.interceptor.ts` - Converted to functional interceptor, adds Bearer token
- `services/token.service.ts` - Reads claims from OAuthService
- `public/login/login.component.ts` - Simplified to redirect to IdentityServer
- `app-routing.module.ts` - Added callback route
- `environments/*.ts` - Added identityServerUrl configuration

#### New Files
- `public/callback/callback.component.ts` - Handles OIDC redirect
- `silent-refresh.html` - Background token refresh

## Configuration

### 1. Duende IdentityServer (Backend/Tems/Tems.IdentityServer/appsettings.json)

```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017/tems_identity"
  },
  "Duende": {
    "LicenseKey": ""  // ⚠️ REQUIRED: Get free license from duendesoftware.com
  },
  "AllowedCorsOrigins": [
    "http://localhost:4200"
  ]
}
```

### 2. TEMS API (Backend/Tems/Tems.Host/appsettings.json)

```json
{
  "IdentityServer": {
    "Authority": "http://localhost:5001",
    "Audience": "tems-api"
  },
  "AllowedCorsOrigins": [
    "http://localhost:4200"
  ]
}
```

### 3. Frontend (Frontend/Tems/src/environments/environment.ts)

```typescript
export const environment = {
  production: false,
  identityServerUrl: 'http://localhost:5001'
};
```

### 4. Docker Compose (Backend/Tems/compose.yaml)

Three services configured:
- `mongodb` - Port 27017
- `identityserver` - Port 5001
- `tems.host` - Port 14721

## Default User

A default admin user is seeded in development:

- **Username:** `admin`
- **Password:** `Admin123!`
- **Permissions:** All 6 claims enabled
  - `can_view_entities`
  - `can_manage_entities`
  - `can_allocate_keys`
  - `can_send_emails`
  - `can_manage_announcements`
  - `can_manage_system_configuration`

## Authorization Policies

The API enforces these policies on endpoints:

| Policy | Claim Required | Description |
|--------|----------------|-------------|
| CanViewEntities | can_view_entities | View entity data |
| CanManageEntities | can_manage_entities | CRUD operations on entities |
| CanAllocateKeys | can_allocate_keys | Key allocation operations |
| CanSendEmails | can_send_emails | Email sending operations |
| CanManageAnnouncements | can_manage_announcements | Announcement management |
| CanManageSystemConfiguration | can_manage_system_configuration | System settings |

## Setup Instructions

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ (⚠️ Currently using 14.21.3 - **upgrade required**)
- MongoDB instance

### Step 1: Get Duende License
1. Visit https://duendesoftware.com/products/communityedition
2. Register for free Community Edition license (free for <$1M revenue)
3. Add license key to `Backend/Tems/Tems.IdentityServer/appsettings.json`

### Step 2: Update Node.js
```bash
# Using nvm (recommended)
nvm install 18
nvm use 18

# Or download from nodejs.org
```

### Step 3: Install Dependencies

#### Backend
```bash
cd "Backend/Tems"
dotnet restore
dotnet build
```

#### Frontend
```bash
cd "Frontend/Tems"
npm install
```

### Step 4: Start MongoDB
```bash
# Using Docker
docker run -d -p 27017:27017 --name mongodb mongo:latest

# Or using local installation
mongod
```

### Step 5: Run Services

#### Option A: Individual Terminals
```bash
# Terminal 1 - IdentityServer
cd "Backend/Tems/Tems.IdentityServer"
dotnet run

# Terminal 2 - API
cd "Backend/Tems/Tems.Host"
dotnet run

# Terminal 3 - Frontend
cd "Frontend/Tems"
ng serve
```

#### Option B: Docker Compose
```bash
cd "Backend/Tems"
docker-compose up
```

### Step 6: Test the Flow

1. Navigate to http://localhost:4200
2. Click "Login"
3. You'll be redirected to http://localhost:5001/Account/Login
4. Enter credentials: `admin` / `Admin123!`
5. After successful login, you'll be redirected back to the Angular app
6. Open browser DevTools > Application > Local Storage to see tokens

## Token Structure

### Access Token Claims
- `sub` - User ID
- `name` - Display name
- `email` - Email address
- `role` - User roles (array)
- `can_view_entities` - Permission claim
- `can_manage_entities` - Permission claim
- (etc.)

### Token Lifetimes
- **Access Token:** 15 minutes
- **Refresh Token:** 30 days (sliding)

## Security Features

- ✅ Authorization Code + PKCE flow (secure for SPAs)
- ✅ No client secrets in frontend
- ✅ Automatic token refresh using silent refresh iframe
- ✅ HTTPS required in production (configured for HTTP in dev)
- ✅ CORS properly configured
- ✅ Claim-based authorization on API endpoints
- ✅ Password hashing using ASP.NET Core Identity hasher

## Production Considerations

1. **HTTPS:** Update all URLs to use HTTPS
2. **Duende License:** Ensure license is in place
3. **CORS:** Update allowed origins for production domains
4. **MongoDB:** Use replica sets for high availability
5. **Secrets:** Store connection strings and license in Azure Key Vault or similar
6. **Token Storage:** Frontend uses sessionStorage (cleared on tab close)
7. **Rate Limiting:** Consider adding rate limiting to login endpoints
8. **Monitoring:** Add logging for authentication events

## Troubleshooting

### Issue: "Invalid license" error
**Solution:** Obtain and add Duende license key to appsettings.json

### Issue: CORS errors in browser
**Solution:** Verify frontend URL is in AllowedCorsOrigins in both IdentityServer and API

### Issue: 401 Unauthorized on API calls
**Solution:** Check that IdentityServer is running and API can reach it on port 5001

### Issue: Login page doesn't load
**Solution:** Ensure IdentityServer is running on port 5001 and MVC views are properly configured

### Issue: Token refresh fails
**Solution:** Check that `silent-refresh.html` exists and is accessible

## Files Modified/Created

### Created
- `Backend/Tems/Tems.IdentityServer/` (entire project)
- `Backend/Tems/Tems.Common/Identity/User.cs`
- `Frontend/Tems/src/app/public/callback/callback.component.ts`
- `Frontend/Tems/src/silent-refresh.html`

### Modified
- `Backend/Tems/Tems.Host/Program.cs`
- `Backend/Tems/Tems.Host/appsettings.json`
- `Backend/Tems/Tems.Host/Tems.Host.csproj`
- `Backend/Tems/compose.yaml`
- `Backend/Tems/Tems.sln`
- `Backend/Tems/Modules/EquipmentManagement/EquipmentManagement.API/` (endpoints)
- `Frontend/Tems/package.json`
- `Frontend/Tems/src/app/app.config.ts`
- `Frontend/Tems/src/app/app-routing.module.ts`
- `Frontend/Tems/src/app/services/auth.service.ts`
- `Frontend/Tems/src/app/auth.interceptor.ts`
- `Frontend/Tems/src/app/services/token.service.ts`
- `Frontend/Tems/src/app/public/login/login.component.ts`
- `Frontend/Tems/src/environments/environment.ts`
- `Frontend/Tems/src/environments/environment.prod.ts`

## Next Steps

1. ✅ Obtain Duende license key
2. ✅ Upgrade Node.js to version 18+
3. ⏸️ Test authentication flow end-to-end
4. ⏸️ Add user management UI (register, profile, password reset)
5. ⏸️ Implement role-based UI component visibility
6. ⏸️ Add unit tests for authentication services
7. ⏸️ Configure production environment
8. ⏸️ Set up CI/CD pipeline

---

**Implementation Date:** November 18, 2025  
**Duende IdentityServer Version:** 7.0.8 (Community Edition)  
**OAuth2 Flow:** Authorization Code + PKCE  
**Token Type:** JWT Bearer
