# TEMS Infrastructure Setup - Complete ✅

## Overview
Docker Compose configuration successfully created and tested for TEMS infrastructure services. All services are running and healthy.

## Running Services

### 1. MongoDB (Database)
- **Container**: `tems-mongodb`
- **Image**: `mongo:7.0`
- **Port**: `27017`
- **Connection String**: `mongodb://localhost:27017`
- **Database**: `TemsDb`
- **Status**: ✅ HEALTHY
- **Health Check**: Ping test every 10s

### 2. Keycloak (Identity Provider)
- **Container**: `tems-keycloak`
- **Image**: `quay.io/keycloak/keycloak:23.0`
- **Port**: `8080`
- **URL**: http://localhost:8080
- **Admin Credentials**: admin/admin
- **Status**: ✅ HEALTHY
- **Mode**: Development (start-dev)

### 3. Duende Identity Server
- **Container**: `tems-identity-server`
- **Image**: Built from `Tems.IdentityServer/Dockerfile`
- **Port**: `5001` (external) → `8080` (internal)
- **URL**: http://localhost:5001
- **OpenID Config**: http://localhost:5001/.well-known/openid-configuration
- **Status**: ✅ HEALTHY
- **License**: Community Edition (valid until 2027)

## File Structure

```
Backend/Tems/
├── compose.yaml                    # Docker Compose configuration
└── Tems.IdentityServer/
    └── Dockerfile                  # Multi-stage build for Identity Server
```

Root directory:
```
start-infrastructure.sh             # Automated startup script
```

## Quick Start Commands

### Start Infrastructure
```bash
# Option 1: Using convenience script
./start-infrastructure.sh

# Option 2: Manual start
cd Backend/Tems
docker compose up -d
```

### Check Status
```bash
cd Backend/Tems
docker compose ps
```

### View Logs
```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f mongodb
docker compose logs -f keycloak
docker compose logs -f identity-server
```

### Stop Services
```bash
docker compose down

# With volume cleanup
docker compose down -v
```

## Configuration Details

### Docker Compose Features
- **Health Checks**: All services have health checks configured
- **Dependencies**: Identity Server waits for MongoDB to be healthy
- **Networking**: All services on `tems-network` bridge
- **Volumes**: 
  - `mongodb_data`: Persistent MongoDB storage
  - `identity-keys`: Identity Server signing keys
- **Restart Policy**: Services restart on failure

### Environment Variables

**Identity Server:**
- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://+:8080`
- `MongoDb__ConnectionString=mongodb://mongodb:27017`
- `MongoDb__DatabaseName=TemsDb`
- Duende license key injected from appsettings

**Keycloak:**
- `KEYCLOAK_ADMIN=admin`
- `KEYCLOAK_ADMIN_PASSWORD=admin`
- HTTP enabled (no HTTPS in dev)
- Health endpoint enabled

## Application Configuration

### Backend (Tems.Host)
Update `appsettings.Development.json`:
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017/",
    "DatabaseName": "TemsDb"
  },
  "IdentityServer": {
    "Authority": "http://localhost:5001",
    "ApiName": "tems-api"
  }
}
```

### Frontend (Angular)
Already configured to use:
- Identity Server: `http://localhost:5001`
- Backend API: `http://localhost:5000` (when backend runs)

## Development Workflow

### 1. Start Infrastructure
```bash
./start-infrastructure.sh
# Wait ~30 seconds for all services to be healthy
```

### 2. Start Backend
```bash
cd Backend/Tems/Tems.Host
dotnet run
# Backend runs on http://localhost:5000
```

### 3. Start Frontend
```bash
cd Frontend/Tems
npm start
# Frontend runs on http://localhost:4200
```

### 4. Access Services
- **Frontend**: http://localhost:4200
- **Backend API**: http://localhost:5000
- **Identity Server**: http://localhost:5001
- **Keycloak Admin**: http://localhost:8080 (admin/admin)
- **MongoDB**: mongodb://localhost:27017

## Troubleshooting

### Docker Not Running
```bash
# Start Docker Desktop
open -a Docker
# Wait 30 seconds for daemon to start
```

### Port Conflicts
```bash
# Check what's using ports
lsof -ti:27017  # MongoDB
lsof -ti:8080   # Keycloak
lsof -ti:5001   # Identity Server

# Kill process using port
lsof -ti:PORT | xargs kill -9
```

### Container Name Conflicts
```bash
# Remove old containers
docker rm -f tems-mongodb tems-keycloak tems-identity-server

# Or use compose down
docker compose down
```

### Service Not Healthy
```bash
# Check logs
docker compose logs [service-name]

# Restart specific service
docker compose restart [service-name]

# Rebuild and restart
docker compose up -d --build [service-name]
```

### MongoDB Connection Issues
```bash
# Test MongoDB connection
docker exec tems-mongodb mongosh --eval "db.adminCommand('ping')"

# Should return: { ok: 1 }
```

### Identity Server Not Ready
```bash
# Check configuration endpoint
curl http://localhost:5001/.well-known/openid-configuration

# Should return JSON with issuer, endpoints, etc.
```

## Next Steps for Login Implementation

### Infrastructure Status: ✅ READY
- MongoDB: Running and accepting connections
- Keycloak: Running and accessible
- Identity Server: Running with valid OpenID configuration

### Required for Login:
1. **Backend Configuration**
   - Verify JWT token validation settings
   - Configure authentication middleware
   - Set up user claims mapping

2. **Frontend Configuration**
   - Configure OAuth/OIDC client settings
   - Implement login/logout flows
   - Set up token refresh mechanism

3. **Identity Server Setup**
   - Configure clients (Frontend SPA, Backend API)
   - Set up API resources and scopes
   - Create test users or integrate with Keycloak

4. **Keycloak Configuration** (if using)
   - Create realm
   - Configure clients
   - Set up user federation
   - Define roles and permissions

## Service Dependencies

```
┌─────────────────────┐
│   Frontend (4200)   │
│    Angular App      │
└──────────┬──────────┘
           │
           ├──────────────────┐
           │                  │
           ▼                  ▼
┌──────────────────┐  ┌───────────────────┐
│ Backend (5000)   │  │ Identity (5001)   │
│  Tems.Host API   │  │ Duende Identity   │
└────────┬─────────┘  └─────────┬─────────┘
         │                      │
         └──────────┬───────────┘
                    ▼
        ┌─────────────────────┐
        │  MongoDB (27017)    │
        │      TemsDb         │
        └─────────────────────┘
                    
        ┌─────────────────────┐
        │  Keycloak (8080)    │
        │   (Alternative)     │
        └─────────────────────┘
```

## Build Information

### Identity Server Image
- **Build Time**: ~25 seconds
- **Base Image**: mcr.microsoft.com/dotnet/aspnet:9.0
- **SDK Image**: mcr.microsoft.com/dotnet/sdk:9.0
- **Build Type**: Multi-stage (build → publish → final)
- **Size**: Optimized with layer caching

### Volumes
- **mongodb_data**: Persists database across container restarts
- **identity-keys**: Persists signing keys for token validation

## Security Notes

### Development Only
- ⚠️ Keycloak admin credentials hardcoded (admin/admin)
- ⚠️ HTTP used (no HTTPS certificates)
- ⚠️ MongoDB has no authentication enabled
- ⚠️ All services exposed on host network

### Production Checklist
- [ ] Change Keycloak admin password
- [ ] Enable MongoDB authentication
- [ ] Use HTTPS with proper certificates
- [ ] Implement network isolation
- [ ] Add rate limiting
- [ ] Configure firewall rules
- [ ] Use secrets management (not environment variables)
- [ ] Enable audit logging

## Monitoring

### Health Check Endpoints
- MongoDB: `mongosh --eval "db.adminCommand('ping')"`
- Keycloak: http://localhost:8080/health/ready
- Identity Server: http://localhost:5001/.well-known/openid-configuration

### Container Stats
```bash
docker stats tems-mongodb tems-keycloak tems-identity-server
```

## Cleanup

### Stop and Remove Everything
```bash
cd Backend/Tems
docker compose down -v  # -v removes volumes too
```

### Remove Only Containers (Keep Data)
```bash
docker compose down
```

### Prune System (Caution)
```bash
# Remove all stopped containers, networks, dangling images
docker system prune -a
```

---

**Status**: ✅ Infrastructure fully operational and ready for authentication implementation
**Date**: January 6, 2026
**Frontend**: Compiling successfully on http://localhost:4200
**Next Task**: Fix login/authentication flows
