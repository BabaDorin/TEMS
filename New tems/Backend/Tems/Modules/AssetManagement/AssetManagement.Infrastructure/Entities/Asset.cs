using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetManagement.Infrastructure.Entities;

public class Asset
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("serial_number")]
    public string SerialNumber { get; set; } = string.Empty;

    [BsonElement("asset_tag")]
    public string AssetTag { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "active";

    [BsonElement("definition")]
    public AssetDefinitionSnapshot Definition { get; set; } = new();

    [BsonElement("purchase_info")]
    public PurchaseInfo? PurchaseInfo { get; set; }

    [BsonElement("location_id")]
    public string? LocationId { get; set; }

    [BsonElement("location")]
    public AssetLocation? Location { get; set; }

    [BsonElement("assignment")]
    public AssetAssignment? Assignment { get; set; }

    [BsonElement("parent_asset_id")]
    [BsonRepresentation(BsonType.String)]
    public string? ParentAssetId { get; set; }

    [BsonElement("child_asset_ids")]
    public List<string> ChildAssetIds { get; set; } = [];

    [BsonElement("notes")]
    public string Notes { get; set; } = string.Empty;

    [BsonElement("maintenance_history")]
    public List<MaintenanceRecord> MaintenanceHistory { get; set; } = [];

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [BsonElement("is_archived")]
    public bool IsArchived { get; set; }

    [BsonElement("archived_at")]
    public DateTime? ArchivedAt { get; set; }

    [BsonElement("archived_by")]
    public string? ArchivedBy { get; set; }
}

public class AssetDefinitionSnapshot
{
    [BsonElement("definition_id")]
    [BsonRepresentation(BsonType.String)]
    public string DefinitionId { get; set; } = string.Empty;

    [BsonElement("is_customized")]
    public bool IsCustomized { get; set; }

    [BsonElement("snapshot_at")]
    public DateTime SnapshotAt { get; set; } = DateTime.UtcNow;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("asset_type_id")]
    [BsonRepresentation(BsonType.String)]
    public string AssetTypeId { get; set; } = string.Empty;

    [BsonElement("asset_type_name")]
    public string AssetTypeName { get; set; } = string.Empty;

    [BsonElement("manufacturer")]
    public string Manufacturer { get; set; } = string.Empty;

    [BsonElement("model")]
    public string Model { get; set; } = string.Empty;

    [BsonElement("specifications")]
    public List<AssetSpecification> Specifications { get; set; } = [];
}

public class PurchaseInfo
{
    [BsonElement("purchase_date")]
    public DateTime? PurchaseDate { get; set; }

    [BsonElement("purchase_price")]
    public decimal? PurchasePrice { get; set; }

    [BsonElement("currency")]
    public string Currency { get; set; } = "USD";

    [BsonElement("vendor")]
    public string Vendor { get; set; } = string.Empty;

    [BsonElement("warranty_expiry")]
    public DateTime? WarrantyExpiry { get; set; }
}

public class AssetLocation
{
    [BsonElement("building")]
    public string Building { get; set; } = string.Empty;

    [BsonElement("room")]
    public string Room { get; set; } = string.Empty;

    [BsonElement("desk")]
    public string Desk { get; set; } = string.Empty;
}

public class AssetAssignment
{
    [BsonElement("assigned_to_user_id")]
    [BsonRepresentation(BsonType.String)]
    public string? AssignedToUserId { get; set; }

    [BsonElement("assigned_to_name")]
    public string AssignedToName { get; set; } = string.Empty;

    [BsonElement("assigned_at")]
    public DateTime? AssignedAt { get; set; }

    [BsonElement("assignment_type")]
    public string AssignmentType { get; set; } = "permanent";
}

public class MaintenanceRecord
{
    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("performed_by")]
    public string PerformedBy { get; set; } = string.Empty;

    [BsonElement("cost")]
    public decimal? Cost { get; set; }
}
