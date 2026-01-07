# TEMS Database Seeding Guide

**Last Updated:** January 7, 2026

## Overview

TEMS includes an automatic database seeding system that populates MongoDB with realistic initial data when the application starts. This ensures developers and testers have meaningful data to work with immediately.

## How It Works

The seeding system runs automatically on application startup via a `BackgroundService`:

1. **Startup Check** - When the backend starts, it checks if seed data already exists
2. **Conditional Seeding** - Data is only created if it doesn't already exist (idempotent)
3. **Relationship Aware** - Seeds are created in the correct order to maintain relationships
4. **Tenant Aware** - All seed data is associated with the default tenant

## Seeded Data

### Asset Management Module

#### Asset Types (5 types)
| Name | Description |
|------|-------------|
| Computer | Desktop and laptop computers |
| Monitor | Display monitors and screens |
| Peripheral | Keyboards, mice, webcams, headsets |
| Network Equipment | Routers, switches, access points |
| Mobile Device | Smartphones, tablets, mobile devices |

#### Asset Properties (10 properties)
| Name | Data Type | Description |
|------|-----------|-------------|
| Serial Number | Text | Manufacturer serial number |
| Purchase Date | Date | Date of purchase |
| Warranty Expiry | Date | Warranty expiration date |
| Manufacturer | Text | Equipment manufacturer |
| Model | Text | Model name/number |
| Processor | Text | CPU type and speed |
| RAM | Text | Memory capacity |
| Storage | Text | Storage type and capacity |
| Screen Size | Text | Display diagonal size |
| MAC Address | Text | Network MAC address |

#### Asset Definitions (5 definitions)
| Name | Type | Properties |
|------|------|------------|
| Dell Laptop | Computer | Serial, Purchase Date, Warranty, Manufacturer, Model, Processor, RAM, Storage |
| HP Monitor | Monitor | Serial, Purchase Date, Warranty, Manufacturer, Model, Screen Size |
| Logitech Keyboard | Peripheral | Serial, Manufacturer, Model |
| Cisco Switch | Network Equipment | Serial, Purchase Date, Warranty, Manufacturer, Model, MAC Address |
| iPhone | Mobile Device | Serial, Purchase Date, Warranty, Manufacturer, Model, Storage |

#### Assets (10 sample assets)
| TEMSID | Definition | Location |
|--------|------------|----------|
| TEMS-001 | Dell Laptop | Room 101 |
| TEMS-002 | Dell Laptop | Room 102 |
| TEMS-003 | Dell Laptop | Room 201 |
| TEMS-004 | HP Monitor | Room 101 |
| TEMS-005 | HP Monitor | Room 102 |
| TEMS-006 | Logitech Keyboard | Room 101 |
| TEMS-007 | Logitech Keyboard | Room 102 |
| TEMS-008 | Cisco Switch | Server Room |
| TEMS-009 | iPhone | Reception |
| TEMS-010 | iPhone | IT Department |

### Ticket Management Module

#### Ticket Types (5 types)
| Name | Description | Default Priority |
|------|-------------|------------------|
| Hardware Issue | Physical equipment problems, damage, or malfunction | Medium |
| Software Issue | Application errors, crashes, or configuration problems | Medium |
| Access Request | Request for access to systems, rooms, or resources | Low |
| Maintenance Request | Scheduled or preventive maintenance tasks | Low |
| Security Incident | Security-related issues requiring immediate attention | High |

#### Sample Tickets (5 tickets)
| Summary | Type | Status | Priority |
|---------|------|--------|----------|
| Laptop screen flickering | Hardware Issue | Open | High |
| Microsoft Office not activating | Software Issue | In Progress | Medium |
| Access to server room needed | Access Request | Open | Low |
| Printer maintenance scheduled | Maintenance Request | Resolved | Low |
| Suspicious email reported | Security Incident | In Progress | Critical |

## Configuration

### Enabling/Disabling Seeding

Seeding can be controlled via `appsettings.json`:

```json
{
  "Seeding": {
    "Enabled": true,
    "DefaultTenantId": "default-tenant"
  }
}
```

### Seed Data Location

- **Asset Management Seeder**: `Tems.Host/Seeding/AssetManagementSeeder.cs`
- **Ticket Management Seeder**: `Tems.Host/Seeding/TicketManagementSeeder.cs`
- **Main Orchestrator**: `Tems.Host/Seeding/DatabaseSeederService.cs`

## Adding New Seed Data

### 1. Add to Existing Seeder

To add more assets or tickets, modify the corresponding seeder file:

```csharp
// In AssetManagementSeeder.cs
private static List<Asset> GetSampleAssets(...)
{
    return new List<Asset>
    {
        // Add new assets here
        new Asset
        {
            Id = ObjectId.GenerateNewId().ToString(),
            TemsId = "TEMS-011",
            AssetDefinitionId = laptopDefinition.Id,
            // ...
        }
    };
}
```

### 2. Create New Module Seeder

For a new module:

1. Create a new seeder class in `Tems.Host/Seeding/`
2. Implement the `IModuleSeeder` interface
3. Register it in `DatabaseSeederService.cs`

```csharp
public interface IModuleSeeder
{
    Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct);
}

public class MyModuleSeeder : IModuleSeeder
{
    public async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken ct)
    {
        // Your seeding logic here
    }
}
```

## MongoDB Collections After Seeding

| Collection | Document Count |
|------------|----------------|
| AssetTypes | 5 |
| AssetProperties | 10 |
| AssetDefinitions | 5 |
| Assets | 10 |
| TicketTypes | 5 |
| Tickets | 5 |

## Verification

### Check Seeding Logs

On startup, you'll see logs like:

```
[DatabaseSeederService] Starting database seeding...
[AssetManagementSeeder] Seeding asset types...
[AssetManagementSeeder] Created 5 asset types
[AssetManagementSeeder] Seeding asset properties...
[AssetManagementSeeder] Created 10 asset properties
[AssetManagementSeeder] Seeding asset definitions...
[AssetManagementSeeder] Created 5 asset definitions
[AssetManagementSeeder] Seeding assets...
[AssetManagementSeeder] Created 10 assets
[TicketManagementSeeder] Seeding ticket types...
[TicketManagementSeeder] Created 5 ticket types
[TicketManagementSeeder] Seeding tickets...
[TicketManagementSeeder] Created 5 tickets
[DatabaseSeederService] Database seeding completed successfully
```

### Verify via MongoDB Shell

```bash
# Connect to MongoDB
docker exec -it tems-mongodb mongosh

# Switch to TEMS database
use TemsDb

# Check collections
db.AssetTypes.countDocuments()      # Should return 5
db.AssetProperties.countDocuments() # Should return 10
db.AssetDefinitions.countDocuments() # Should return 5
db.Assets.countDocuments()          # Should return 10
db.TicketTypes.countDocuments()     # Should return 5
db.Tickets.countDocuments()         # Should return 5
```

### Verify via API

```bash
# Get all asset types
curl http://localhost:5158/asset-type \
  -H "Authorization: Bearer YOUR_TOKEN"

# Get all tickets
curl http://localhost:5158/ticket \
  -H "Authorization: Bearer YOUR_TOKEN"
```

## Resetting Seed Data

To reseed the database:

1. **Drop collections** (MongoDB shell):
```javascript
use TemsDb
db.AssetTypes.drop()
db.AssetProperties.drop()
db.AssetDefinitions.drop()
db.Assets.drop()
db.TicketTypes.drop()
db.Tickets.drop()
```

2. **Restart the backend** - Seeding will run automatically

## Data Relationships

```
┌─────────────────┐
│   AssetType     │
│  (e.g. Computer)│
└────────┬────────┘
         │ 1:N
         ▼
┌─────────────────┐      ┌──────────────────┐
│AssetDefinition  │─────▶│  AssetProperty   │
│(e.g. Dell Laptop│ M:N  │(e.g. Serial No.) │
└────────┬────────┘      └──────────────────┘
         │ 1:N
         ▼
┌─────────────────┐
│     Asset       │
│ (e.g. TEMS-001) │
└─────────────────┘

┌─────────────────┐
│   TicketType    │
│(e.g. Hardware)  │
└────────┬────────┘
         │ 1:N
         ▼
┌─────────────────┐
│     Ticket      │
│(e.g. Screen fix)│
└─────────────────┘
```

## Troubleshooting

### Seed Data Not Created

**Symptom:** No data in MongoDB after startup

**Check:**
1. Verify MongoDB is running: `docker ps | grep mongodb`
2. Check connection string in `appsettings.json`
3. Look for errors in startup logs
4. Ensure seeding is enabled in configuration

### Duplicate Key Errors

**Symptom:** Errors about duplicate keys during seeding

**Cause:** Partial seeding from previous run

**Solution:** Drop affected collections and restart

### Missing Relationships

**Symptom:** Assets showing "Unknown Definition"

**Cause:** Definition IDs not matching

**Solution:** Drop all collections and reseed (order matters)

## Best Practices

1. **Idempotent Seeding** - Always check if data exists before creating
2. **Realistic Data** - Use meaningful names and values
3. **Relationship Order** - Seed parent entities before children
4. **Tenant Isolation** - Associate all seed data with correct tenant
5. **Logging** - Log seeding progress for debugging
