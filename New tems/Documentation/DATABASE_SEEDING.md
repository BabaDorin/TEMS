# TEMS Database Seeding Guide

**Last Updated:** January 18, 2026

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

#### Asset Types (10 types)
| Name | Description |
|------|-------------|
| Computer | Root for desktops and laptops |
| Laptop | Portable computer (child of Computer) |
| Desktop | Workstation (child of Computer) |
| Monitor | Display monitors and screens |
| Printer | Printing device |
| Network Switch | Routers, switches, access points |
| IP Phone | VoIP telephone |
| Furniture | Parent for desks and chairs |
| Desk | Office desk (child of Furniture) |
| Office Chair | Seating (child of Furniture) |

#### Asset Properties (25 properties)
Data types covered: string, number, boolean, enum, and date (Warranty Expiration).
| Name | Data Type | Category |
|------|-----------|----------|
| CPU | string | Hardware |
| RAM | number | Hardware |
| Storage | number | Hardware |
| Screen Size | number | Display |
| Resolution | string | Display |
| Operating System | enum | Software |
| IP Address | string | Network |
| MAC Address | string | Network |
| Hostname | string | Network |
| Warranty Expiration | date | Lifecycle |
| Print Speed | number | Performance |
| Print Technology | enum | Hardware |
| Color Support | boolean | Features |
| Duplex Printing | boolean | Features |
| Panel Type | enum | Display |
| Refresh Rate | number | Display |
| Port Count | number | Network |
| PoE Support | boolean | Features |
| Speed | enum | Performance |
| VoIP Support | boolean | Features |
| PoE Class | enum | Hardware |
| Dimensions | string | Physical |
| Material | enum | Physical |
| Color | string | Appearance |
| Adjustable | boolean | Features |

#### Asset Definitions (12 definitions)
| Name | Type |
|------|------|
| Dell Latitude 5430 | Laptop |
| Apple MacBook Pro 14-inch | Laptop |
| Lenovo ThinkPad X1 Carbon Gen 11 | Laptop |
| Dell OptiPlex 7090 | Desktop |
| Dell P2722H | Monitor |
| LG 27UP850 | Monitor |
| HP LaserJet Pro M404dn | Printer |
| Canon PIXMA TR8620 | Printer |
| Cisco Catalyst 2960-X | Network Switch |
| Yealink SIP-T46S | IP Phone |
| Steelcase Series 1 | Office Chair |
| Fully Jarvis Standing Desk | Desk |

#### Assets (9 sample assets)
| Asset Tag | Definition | Location |
|-----------|------------|----------|
| LAP-001 | Dell Latitude 5430 | Room 101, Desk A |
| LAP-002 | Apple MacBook Pro 14-inch | Room 102, Desk B |
| MON-001 | Dell P2722H | Room 101, Desk A |
| MON-002 | Dell P2722H | Room 101, Desk A |
| PRN-001 | HP LaserJet Pro M404dn | Print Room |
| NET-001 | Cisco Catalyst 2960-X | Server Room |
| PHN-001 | Yealink SIP-T46S | Room 101, Desk A |
| CHR-001 | Steelcase Series 1 | Room 101, Desk A |
| DSK-001 | Fully Jarvis Standing Desk | Room 101, Desk A |

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
| AssetTypes | 10 |
| AssetProperties | 25 |
| AssetDefinitions | 12 |
| Assets | 9 |
| TicketTypes | 5 |
| Tickets | 5 |

## Verification

### Check Seeding Logs

On startup, you'll see logs like:

```
[DatabaseSeederService] Starting database seeding...
[AssetManagementSeeder] Seeding asset types...
[AssetManagementSeeder] Created 10 asset types
[AssetManagementSeeder] Seeding asset properties...
[AssetManagementSeeder] Created 25 asset properties
[AssetManagementSeeder] Seeding asset definitions...
[AssetManagementSeeder] Created 12 asset definitions
[AssetManagementSeeder] Seeding assets...
[AssetManagementSeeder] Created 9 assets
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
db.AssetTypes.countDocuments()      # Should return 10
db.AssetProperties.countDocuments() # Should return 25
db.AssetDefinitions.countDocuments() # Should return 12
db.Assets.countDocuments()          # Should return 9
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
