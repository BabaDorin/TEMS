using AssetManagement.Infrastructure.Entities;
using LocationManagement.Infrastructure.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using UserManagement.Infrastructure.Entities;

namespace Tems.Host.Seeding;

public class BulkEquipmentSeeder(IMongoDatabase database, ILogger<BulkEquipmentSeeder> logger)
{
    private readonly IMongoCollection<AssetType> _assetTypes = database.GetCollection<AssetType>("asset_types");
    private readonly IMongoCollection<AssetDefinition> _assetDefinitions = database.GetCollection<AssetDefinition>("asset_definitions");
    private readonly IMongoCollection<Asset> _assets = database.GetCollection<Asset>("assets");
    private readonly IMongoCollection<Room> _rooms = database.GetCollection<Room>("rooms");
    private readonly IMongoCollection<Building> _buildings = database.GetCollection<Building>("buildings");
    private readonly IMongoCollection<User> _users = database.GetCollection<User>("users");

    public async Task SeedAsync(int targetCount = 500, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Seeding bulk equipment data...");

        var definitions = await _assetDefinitions.Find(FilterDefinition<AssetDefinition>.Empty)
            .SortBy(d => d.Name)
            .ToListAsync(cancellationToken);

        if (definitions.Count == 0)
        {
            logger.LogWarning("No asset definitions found. Skipping bulk equipment seeding.");
            return;
        }

        var rooms = await _rooms.Find(FilterDefinition<Room>.Empty).ToListAsync(cancellationToken);
        var buildings = await _buildings.Find(FilterDefinition<Building>.Empty).ToListAsync(cancellationToken);
        var users = await _users.Find(FilterDefinition<User>.Empty).ToListAsync(cancellationToken);

        var bulkTagFilter = Builders<Asset>.Filter.Regex(x => x.AssetTag, new BsonRegularExpression("^BULK-EQ-"));
        var existingBulkCount = (int)await _assets.CountDocumentsAsync(bulkTagFilter, cancellationToken: cancellationToken);
        if (existingBulkCount >= targetCount)
        {
            logger.LogInformation("Bulk equipment already seeded with {Count} records. Skipping.", existingBulkCount);
            return;
        }

        var missingCount = targetCount - existingBulkCount;
        var startIndex = existingBulkCount + 1;
        var now = DateTime.UtcNow;
        var statuses = new[] { "active", "in_use", "under_maintenance" };
        var vendorsByManufacturer = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Dell"] = "Dell Direct",
            ["Apple"] = "Apple Business",
            ["Lenovo"] = "Lenovo Pro",
            ["HP"] = "HP Enterprise",
            ["Canon"] = "Canon Business",
            ["Cisco"] = "Cisco Partner",
            ["Yealink"] = "VoIP Supplier",
            ["Steelcase"] = "Office Furniture Inc",
            ["Fully"] = "Office Furniture Inc"
        };

        var buildingLookup = buildings.ToDictionary(b => b.Id, b => b.Name);
        var roomLookup = rooms.ToDictionary(
            room => room.Id,
            room => new
            {
                Room = room,
                BuildingName = buildingLookup.TryGetValue(room.BuildingId, out var buildingName) ? buildingName : room.BuildingId
            });

        var assets = new List<Asset>(missingCount);
        for (var i = 0; i < missingCount; i++)
        {
            var definition = definitions[i % definitions.Count];
            var status = statuses[i % statuses.Length];
            var assetNumber = startIndex + i;
            var serialNumber = $"{definition.ShortName}-{assetNumber:0000}";
            var assetTag = $"BULK-EQ-{assetNumber:0000}";
            var purchaseDate = now.AddMonths(-((assetNumber % 36) + 1));

            var asset = new Asset
            {
                SerialNumber = serialNumber,
                AssetTag = assetTag,
                Status = status,
                Definition = new AssetDefinitionSnapshot
                {
                    DefinitionId = definition.Id,
                    IsCustomized = false,
                    SnapshotAt = now,
                    Name = definition.Name,
                    AssetTypeId = definition.AssetTypeId,
                    AssetTypeName = definition.AssetTypeName,
                    Manufacturer = definition.Manufacturer,
                    Model = definition.Model,
                    Specifications = definition.Specifications
                        .Select(spec => new AssetSpecification
                        {
                            PropertyId = spec.PropertyId,
                            Name = spec.Name,
                            Value = spec.Value,
                            DataType = spec.DataType,
                            Unit = spec.Unit
                        })
                        .ToList()
                },
                PurchaseInfo = new PurchaseInfo
                {
                    PurchaseDate = purchaseDate,
                    PurchasePrice = 100m + (assetNumber % 25) * 37.5m,
                    Currency = "USD",
                    Vendor = vendorsByManufacturer.TryGetValue(definition.Manufacturer, out var vendor)
                        ? vendor
                        : "Equipment Supplier",
                    WarrantyExpiry = purchaseDate.AddYears(3)
                },
                Notes = $"Bulk seeded equipment #{assetNumber}",
                CreatedBy = "system",
                CreatedAt = now,
                UpdatedAt = now
            };

            if (roomLookup.Count > 0)
            {
                var roomEntry = roomLookup.ElementAt(i % roomLookup.Count).Value;
                asset.LocationId = roomEntry.Room.Id;
                asset.Location = new AssetLocation
                {
                    Building = roomEntry.BuildingName,
                    Room = roomEntry.Room.Name,
                    Desk = roomEntry.Room.RoomNumber ?? $"Desk {((assetNumber - 1) % 20) + 1}"
                };
            }

            if (users.Count > 0 && i % 2 == 0)
            {
                var user = users[i % users.Count];
                asset.Assignment = new AssetAssignment
                {
                    AssignedToUserId = user.Id,
                    AssignedToName = user.GetDisplayName(),
                    AssignedAt = now.AddMonths(-(assetNumber % 12)),
                    AssignmentType = "permanent"
                };
            }

            assets.Add(asset);
        }

        await _assets.InsertManyAsync(assets, cancellationToken: cancellationToken);
        logger.LogInformation("Seeded {Count} bulk equipment records.", assets.Count);
    }
}
