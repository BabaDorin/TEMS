# Development Environment Configuration Verification

## Overview
Both Tems.Host and Tems.IdentityServer are now properly configured to load `appsettings.Development.json` when running locally (non-production environments).

## Configuration Details

### 1. Tems.Host (API)

**File:** `Backend/Tems/Tems.Host/Program.cs`

```csharp
// Ensure appsettings.Development.json is loaded for non-production environments
if (!builder.Environment.IsProduction())
{
    builder.Environment.EnvironmentName = "Development";
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
}
```

**Configuration Files:**
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development-specific overrides (local MongoDB, localhost ports)
- `appsettings.Production.json` - Production-specific configuration

**Launch Profile:**
- Set in `Properties/launchSettings.json`
- `ASPNETCORE_ENVIRONMENT` = `Development`

### 2. Tems.IdentityServer

**File:** `Backend/Tems/Tems.IdentityServer/Program.cs`

```csharp
// Ensure appsettings.Development.json is loaded for non-production environments
if (!builder.Environment.IsProduction())
{
    builder.Environment.EnvironmentName = "Development";
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
}
```

**Configuration Files:**
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development-specific overrides (Duende license, local MongoDB)
- `appsettings.Production.json` - Production-specific configuration (if needed)

**Launch Profile:**
- Set in `Properties/launchSettings.json`
- `ASPNETCORE_ENVIRONMENT` = `Development`

## Configuration Loading Order

When running locally with `dotnet run`:

1. WebApplicationBuilder loads `appsettings.json` (base)
2. WebApplicationBuilder loads `appsettings.{ASPNETCORE_ENVIRONMENT}.json` (overrides)
3. Program.cs explicitly adds `appsettings.Development.json` if not in production
4. Environment variables override configuration files

**Result:** Development settings take precedence over base settings

## Key Configuration Values in Development

### Tems.Host (appsettings.Development.json)
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "TemsDb"
  },
  "IdentityServer": {
    "Authority": "http://localhost:5001",
    "ApiName": "tems-api"
  }
}
```

### Tems.IdentityServer (appsettings.Development.json)
```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "TemsDb"
  },
  "Duende": {
    "LicenseKey": "YOUR_DUENDE_LICENSE_KEY"
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:4200"
  }
}
```

## Running Local Services

### Option 1: Using `dotnet run`
```bash
# Tems.Host - Reads launchSettings.json (sets ASPNETCORE_ENVIRONMENT=Development)
cd "Backend/Tems/Tems.Host"
dotnet run

# Tems.IdentityServer - Same behavior
cd "Backend/Tems/Tems.IdentityServer"
dotnet run
```

### Option 2: Explicit Environment Variable
```bash
cd "Backend/Tems/Tems.Host"
ASPNETCORE_ENVIRONMENT=Development dotnet run

cd "Backend/Tems/Tems.IdentityServer"
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

### Option 3: Visual Studio / Rider
- Debug configuration automatically uses launchSettings.json
- ASPNETCORE_ENVIRONMENT=Development is set automatically

## Verification

To verify the correct configuration is being loaded:

```bash
# Start the application and check the logs
ASPNETCORE_ENVIRONMENT=Development dotnet run --project Tems.Host

# Look for these indicators in the logs:
# - "Application is starting..."
# - MongoDB connection with "mongodb://localhost:27017"
# - IdentityServer Authority: "http://localhost:5001"
```

## Important Notes

1. **ASPNETCORE_ENVIRONMENT Not Required**
   - `launchSettings.json` automatically sets it to "Development"
   - Program.cs ensures Development settings are loaded if not in production

2. **Production Deployment**
   - Set `ASPNETCORE_ENVIRONMENT=Production` before running
   - Will use `appsettings.json` and `appsettings.Production.json`
   - Development files will NOT be loaded

3. **Docker/Container Deployment**
   - Set environment variable in Dockerfile or docker-compose.yaml
   - Configure appropriate connection strings for production

## Build Status

✅ **All Code Compiles Successfully**
- No errors in Tems.Host
- No errors in Tems.IdentityServer
- All dependencies resolved
- All using directives added where needed

## Files Modified

### Backend/Tems/Tems.Host/Program.cs
- Added explicit Development configuration loading
- Line 10-14: Configuration loading logic

### Backend/Tems/Tems.IdentityServer/Program.cs
- Added explicit Development configuration loading
- Added missing `using Microsoft.Extensions.Options;`
- Line 10-15: Configuration loading logic

### Backend/Tems/Tems.IdentityServer/UserStore/MongoDbProfileService.cs
- Added missing `using Duende.IdentityServer.Extensions;`

### Backend/Tems/Tems.IdentityServer/Quickstart/Account/AccountController.cs
- Added missing using directives
- Fixed authentication handler check logic
- Replaced undefined extension method with proper implementation

## Summary

Both services are now guaranteed to load `appsettings.Development.json` when:
1. Running locally via `dotnet run`
2. Debugging in Visual Studio/Rider
3. Running with explicit `ASPNETCORE_ENVIRONMENT=Development`

Production deployments will use `appsettings.Production.json` when `ASPNETCORE_ENVIRONMENT=Production` is set.
