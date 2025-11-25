# TEMS Quick Start Guide

## Prerequisites

- Docker and Docker Compose
- .NET 8 SDK
- Node.js v20.19.5 (use nvm)
- MongoDB client (optional)

## Starting TEMS

### Quick Start (Recommended)

```bash
# Make scripts executable
chmod +x start-tems.sh stop-tems.sh

# Start all services
./start-tems.sh
```

The startup script will:
1. Start MongoDB and Keycloak in Docker
2. Configure Keycloak (first time only)
3. Start Duende IdentityServer
4. Start Backend API
5. Start Angular Frontend

### Manual Start

If you prefer to start services manually:

```bash
# 1. Start Docker services
cd "Backend/Tems"
docker compose up -d

# 2. Wait for Keycloak to be ready (about 60 seconds)
# Then configure Keycloak (first time only)
cd "../../Infrastructure/Keycloak"
chmod +x configure-keycloak.sh
./configure-keycloak.sh

# 3. Start IdentityServer
cd "../../Backend/Tems/Tems.IdentityServer"
dotnet run --urls "http://localhost:5001"

# 4. Start Backend API (in new terminal)
cd "../Tems.Host"
dotnet run --urls "http://localhost:14721"

# 5. Start Frontend (in new terminal)
cd "../../../Frontend/Tems"
nvm use 20.19.5
npm start
```

## Stopping TEMS

```bash
./stop-tems.sh
```

## Service URLs

| Service | URL | Credentials |
|---------|-----|-------------|
| Frontend | http://localhost:4200 | Register new user |
| Keycloak Admin | http://localhost:8080/admin | admin/admin |
| IdentityServer | http://localhost:5001 | N/A |
| Backend API | http://localhost:14721 | N/A |
| MongoDB | mongodb://localhost:27017 | N/A |

## First Time Setup

### 1. Register a New User

1. Navigate to http://localhost:4200
2. Click "Sign up"
3. Enter your email and password
4. Click "Create Account"
5. You'll be redirected to login

### 2. Login

1. Click "Sign in"
2. You'll be redirected to Keycloak
3. Login with your registered credentials
4. You'll be redirected back to the TEMS application

### 3. (Optional) Create Admin User

To create a user with admin privileges:

```bash
# Connect to MongoDB
mongosh

# Switch to TemsDb
use TemsDb

# Find your user and update roles
db.users.updateOne(
  { email: "your@email.com" },
  {
    $set: {
      roles: ["Admin"],
      claims: {
        can_view_entities: "true",
        can_manage_entities: "true",
        can_allocate_keys: "true",
        can_send_emails: "true",
        can_manage_announcements: "true",
        can_manage_system_configuration: "true"
      }
    }
  }
)
```

## Troubleshooting

### Frontend won't start
```bash
cd "Frontend/Tems"
nvm use 20.19.5
rm -rf node_modules package-lock.json
npm install
npm start
```

### Keycloak not responding
```bash
# Check Keycloak logs
docker logs tems-keycloak

# Restart Keycloak
docker restart tems-keycloak

# Wait 60 seconds for startup
```

### Authentication not working
```bash
# Reconfigure Keycloak
cd "Infrastructure/Keycloak"
rm .configured
./configure-keycloak.sh
```

### Backend API errors
```bash
# Check logs
tail -f /tmp/tems-backend.log

# Verify MongoDB is running
docker ps | grep mongodb

# Restart backend
cd "Backend/Tems/Tems.Host"
dotnet run --urls "http://localhost:14721"
```

### IdentityServer errors
```bash
# Check logs
tail -f /tmp/tems-identityserver.log

# Restart IdentityServer
cd "Backend/Tems/Tems.IdentityServer"
dotnet run --urls "http://localhost:5001"
```

## Development

### View Logs

```bash
# Frontend
tail -f /tmp/tems-frontend.log

# Backend
tail -f /tmp/tems-backend.log

# IdentityServer
tail -f /tmp/tems-identityserver.log

# Keycloak
docker logs -f tems-keycloak

# MongoDB
docker logs -f tems-mongodb
```

### Rebuild Backend

```bash
cd "Backend/Tems/Tems.Host"
dotnet build
dotnet run --urls "http://localhost:14721"
```

### Rebuild Frontend

```bash
cd "Frontend/Tems"
npm run build
npm start
```

## Architecture

See [AUTH_SETUP.md](./AUTH_SETUP.md) for detailed architecture documentation.
