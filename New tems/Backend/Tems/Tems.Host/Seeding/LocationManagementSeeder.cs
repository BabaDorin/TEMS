using LocationManagement.Infrastructure.Entities;
using MongoDB.Driver;

namespace Tems.Host.Seeding;

public class LocationManagementSeeder(IMongoDatabase database, ILogger<LocationManagementSeeder> logger)
{
    private readonly IMongoCollection<Site> _sites = database.GetCollection<Site>("sites");
    private readonly IMongoCollection<Building> _buildings = database.GetCollection<Building>("buildings");
    private readonly IMongoCollection<Room> _rooms = database.GetCollection<Room>("rooms");

    public async Task SeedAsync()
    {
        logger.LogInformation("Seeding Location Management data...");

        await SeedSitesAsync();
        await SeedBuildingsAsync();
        await SeedRoomsAsync();

        logger.LogInformation("Location Management seeding completed.");
    }

    private async Task SeedSitesAsync()
    {
        var count = await _sites.CountDocumentsAsync(FilterDefinition<Site>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Sites already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding sites...");

        var now = DateTime.UtcNow;
        var sites = new List<Site>
        {
            new()
            {
                Id = "site_hq_sf",
                Name = "Headquarters - San Francisco",
                Code = "HQ-SF",
                Timezone = "America/Los_Angeles",
                IsActive = true,
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "site_dc_ny",
                Name = "Data Center - New York",
                Code = "DC-NY",
                Timezone = "America/New_York",
                IsActive = true,
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "site_branch_la",
                Name = "Branch Office - Los Angeles",
                Code = "BR-LA",
                Timezone = "America/Los_Angeles",
                IsActive = true,
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            }
        };

        await _sites.InsertManyAsync(sites);
        logger.LogInformation("Seeded {Count} sites.", sites.Count);
    }

    private async Task SeedBuildingsAsync()
    {
        var count = await _buildings.CountDocumentsAsync(FilterDefinition<Building>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Buildings already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding buildings...");

        var now = DateTime.UtcNow;
        var buildings = new List<Building>
        {
            new()
            {
                Id = "bldg_hq_main",
                SiteId = "site_hq_sf",
                Name = "Main Building",
                AddressLine = "123 Market Street, San Francisco, CA 94105",
                ManagerContact = "facilities@company.com",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "bldg_hq_annex",
                SiteId = "site_hq_sf",
                Name = "Annex Building",
                AddressLine = "125 Market Street, San Francisco, CA 94105",
                ManagerContact = "facilities@company.com",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "bldg_dc_primary",
                SiteId = "site_dc_ny",
                Name = "Primary Data Center",
                AddressLine = "456 Tech Avenue, New York, NY 10001",
                ManagerContact = "datacenter@company.com",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "bldg_la_office",
                SiteId = "site_branch_la",
                Name = "LA Office Building",
                AddressLine = "789 Sunset Blvd, Los Angeles, CA 90028",
                ManagerContact = "la-office@company.com",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            }
        };

        await _buildings.InsertManyAsync(buildings);
        logger.LogInformation("Seeded {Count} buildings.", buildings.Count);
    }

    private async Task SeedRoomsAsync()
    {
        var count = await _rooms.CountDocumentsAsync(FilterDefinition<Room>.Empty);
        if (count > 0)
        {
            logger.LogInformation("Rooms already seeded. Skipping.");
            return;
        }

        logger.LogInformation("Seeding rooms...");

        var now = DateTime.UtcNow;
        var rooms = new List<Room>
        {
            // HQ Main Building Rooms
            new()
            {
                Id = "a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d", // Room 101
                BuildingId = "bldg_hq_main",
                Name = "Room 101",
                FloorLabel = "1st Floor",
                Type = "Desk",
                Capacity = 10,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e", // Room 102
                BuildingId = "bldg_hq_main",
                Name = "Room 102",
                FloorLabel = "1st Floor",
                Type = "Meeting",
                Capacity = 8,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f", // Room 201
                BuildingId = "bldg_hq_main",
                Name = "Room 201",
                FloorLabel = "2nd Floor",
                Type = "Desk",
                Capacity = 12,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "d4e5f6a7-b8c9-4d5e-1f2a-3b4c5d6e7f8a", // Server Room
                BuildingId = "bldg_hq_main",
                Name = "Server Room",
                FloorLabel = "Basement",
                Type = "ServerRoom",
                Capacity = 2,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            // HQ Annex Building Rooms
            new()
            {
                Id = "e5f6a7b8-c9d0-4e5f-2a3b-4c5d6e7f8a9b", // Workshop
                BuildingId = "bldg_hq_annex",
                Name = "Workshop",
                FloorLabel = "Ground Floor",
                Type = "Workshop",
                Capacity = 15,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "f6a7b8c9-d0e1-4f5a-3b4c-5d6e7f8a9b0c", // Equipment Storage
                BuildingId = "bldg_hq_annex",
                Name = "Equipment Storage",
                FloorLabel = "Ground Floor",
                Type = "Desk",
                Capacity = 5,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            // Data Center Rooms
            new()
            {
                Id = "a7b8c9d0-e1f2-4a5b-4c5d-6e7f8a9b0c1d", // Server Room A
                BuildingId = "bldg_dc_primary",
                Name = "Server Room A",
                FloorLabel = "1st Floor",
                Type = "ServerRoom",
                Capacity = 5,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "b8c9d0e1-f2a3-4b5c-5d6e-7f8a9b0c1d2e", // Server Room B
                BuildingId = "bldg_dc_primary",
                Name = "Server Room B",
                FloorLabel = "2nd Floor",
                Type = "ServerRoom",
                Capacity = 5,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f", // Network Operations Center
                BuildingId = "bldg_dc_primary",
                Name = "Network Operations Center",
                FloorLabel = "1st Floor",
                Type = "Desk",
                Capacity = 6,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            // LA Office Rooms
            new()
            {
                Id = "d0e1f2a3-b4c5-4d5e-7f8a-9b0c1d2e3f4a", // Open Office Space
                BuildingId = "bldg_la_office",
                Name = "Open Office Space",
                FloorLabel = "3rd Floor",
                Type = "Desk",
                Capacity = 20,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            },
            new()
            {
                Id = "e1f2a3b4-c5d6-4e5f-8a9b-0c1d2e3f4a5b", // Conference Room
                BuildingId = "bldg_la_office",
                Name = "Conference Room",
                FloorLabel = "3rd Floor",
                Type = "Meeting",
                Capacity = 12,
                Status = "Available",
                TenantId = "default",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "system"
            }
        };

        await _rooms.InsertManyAsync(rooms);
        logger.LogInformation("Seeded {Count} rooms.", rooms.Count);
    }
}
