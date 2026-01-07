# TEMS Documentation Index

This folder contains comprehensive documentation for the TEMS (Technical Equipment Management System) project.

## Quick Reference

### Authentication & Authorization
- **[AUTHENTICATION_FLOW.md](./AUTHENTICATION_FLOW.md)** - Complete guide to the multi-layered auth architecture
  - Keycloak → Duende → MongoDB flow diagram
  - Configuration details for all components
  - Common issues and troubleshooting
  - Testing procedures

- **[LOGIN_FIX_SUMMARY.md](./LOGIN_FIX_SUMMARY.md)** - Recent login issue fix documentation
  - What was broken and why
  - What was fixed and how
  - Step-by-step verification
  - Rollback procedures if needed

### Database
- **[DATABASE_SEEDING.md](./DATABASE_SEEDING.md)** - Database seeding guide
  - Seeded asset types, properties, definitions, and assets
  - Seeded ticket types and sample tickets
  - How to customize seed data
  - Troubleshooting and verification

## Architecture Overview

### Authentication Flow
```
Frontend (Angular) 
    ↓ Uses OIDC
Keycloak (Authorization Layer)
    ↓ Brokers to
Duende IdentityServer (Identity Provider)
    ↓ Validates against
MongoDB (User Store)
```

### Key Components

| Component | Port | Purpose | Technology |
|-----------|------|---------|------------|
| Frontend | 4200 | User Interface | Angular 18, Tailwind CSS |
| Backend API | 5158 | Business Logic | .NET 9, ASP.NET Core |
| Duende IdentityServer | 5001 | Identity Provider | Duende IdentityServer |
| Keycloak | 8080 | Authorization Layer | Keycloak 23 |
| MongoDB | 27017 | Database | MongoDB 7.0 |

### Default Credentials

**Keycloak Admin:**
- URL: http://localhost:8080/admin
- Username: `admin`
- Password: `admin`

**Test Users (both Duende and Keycloak):**
- Admin: `admin` / `Admin123!` (all permissions)
- Regular: `user` / `User123!` (view only)

## Service Management

### Start All Infrastructure
```bash
cd Backend/Tems
docker compose up -d
```

### Start Backend API
```bash
cd Backend/Tems/Tems.Host
dotnet run
```

### Start Frontend
```bash
cd Frontend/Tems
npm start
```

### Check Service Status
```bash
cd /path/to/tems
./check-services.sh
```

### Configure Keycloak (First Time Setup)
```bash
cd Infrastructure/Keycloak
./configure-keycloak.sh
```

## Common Tasks

### Testing Login Flow
1. Navigate to http://localhost:4200
2. Click "Login with Duende IdentityServer"
3. Enter credentials: `admin` / `Admin123!`
4. Verify redirect to `/dashboard`

### Viewing Logs
```bash
# Keycloak logs
docker logs -f tems-keycloak

# Duende IdentityServer logs
docker logs -f tems-identity-server

# MongoDB logs
docker logs -f tems-mongodb

# Backend API logs (in terminal where dotnet run is active)

# Frontend logs (browser console)
```

### Database Access
```bash
# Connect to MongoDB
docker exec -it tems-mongodb mongosh

# List databases
show dbs

# Use tems database
use tems

# List users
db.users.find().pretty()
```

## Project Structure

```
TEMS/
├── Backend/
│   └── Tems/
│       ├── Modules/                    # Feature modules
│       │   ├── AssetManagement/
│       │   ├── EquipmentManagement/
│       │   ├── TicketManagement/
│       │   └── Example/
│       ├── Tems.Common/                # Shared code
│       ├── Tems.Host/                  # API host
│       ├── Tems.IdentityServer/        # Duende IdentityServer
│       └── compose.yaml                # Infrastructure
│
├── Frontend/
│   └── Tems/
│       └── src/
│           ├── app/
│           │   ├── public/             # Public pages (login, etc.)
│           │   ├── tems-components/    # Feature components
│           │   ├── services/           # Services
│           │   ├── guards/             # Route guards
│           │   └── models/             # TypeScript interfaces
│           └── assets/                 # Static assets
│
├── Infrastructure/
│   └── Keycloak/                       # Keycloak config scripts
│
└── Documentation/                      # This folder
    ├── AUTHENTICATION_FLOW.md
    ├── LOGIN_FIX_SUMMARY.md
    └── README.md (this file)
```

## Development Workflow

### Making Changes

1. **Frontend Changes:**
   - Edit files in `Frontend/Tems/src/`
   - Changes auto-reload (hot module replacement)
   - Check browser console for errors

2. **Backend Changes:**
   - Edit files in `Backend/Tems/`
   - Stop and restart `dotnet run`
   - Check terminal for compilation errors

3. **Database Changes:**
   - Connect via mongosh or MongoDB Compass
   - Duende auto-seeds admin user on startup

### Testing Authentication Changes

1. Clear browser storage (important!)
   - Chrome DevTools → Application → Storage → Clear site data
2. Test login flow from scratch
3. Check browser console for OAuth logs
4. Verify tokens in Local Storage

### Troubleshooting

**Issue: Login redirects but no token**
- Clear browser storage
- Check Keycloak realm exists: `curl http://localhost:8080/realms/tems`
- Verify redirect URI in Keycloak matches frontend

**Issue: Can't access Keycloak admin**
- Verify Keycloak is running: `docker ps | grep keycloak`
- Check logs: `docker logs tems-keycloak`

**Issue: Backend 401 errors**
- Verify token is being sent (Network tab → Headers)
- Check token is valid (decode at jwt.io)
- Verify backend is validating against correct issuer

## Further Documentation

### Infrastructure Setup
See project root:
- `INFRASTRUCTURE_SETUP.md` - Docker Compose infrastructure guide
- `START_HERE.md` - Project quickstart guide
- `STATUS.md` - Project status and features

### API Documentation
- Backend Swagger: http://localhost:5158/swagger (when API is running)
- Duende Discovery: http://localhost:5001/.well-known/openid-configuration
- Keycloak Discovery: http://localhost:8080/realms/tems/.well-known/openid-configuration

## Security Notes

⚠️ **Development Only Configuration**
- All credentials are development defaults
- HTTP is allowed (requireHttps: false)
- CORS is open for localhost
- Debug logging is enabled

🔒 **For Production:**
- Change all default passwords
- Enable HTTPS everywhere
- Restrict CORS origins
- Disable debug logging
- Use environment variables for secrets
- Enable rate limiting
- Implement monitoring

## Contributing

When making changes:
1. Document authentication changes in `AUTHENTICATION_FLOW.md`
2. Update this README if adding new components
3. Test the full login flow after auth changes
4. Check that all services still start correctly

## Support

If login is broken again:
1. Read `LOGIN_FIX_SUMMARY.md` first
2. Check `AUTHENTICATION_FLOW.md` for architecture
3. Run service status check: `./check-services.sh`
4. Review logs in each component
5. Try rollback steps in LOGIN_FIX_SUMMARY.md

## Last Updated
- Initial documentation: January 7, 2026
- Authentication fix applied: January 7, 2026
