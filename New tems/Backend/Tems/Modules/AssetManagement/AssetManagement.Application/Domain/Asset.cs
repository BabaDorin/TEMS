namespace AssetManagement.Application.Domain;

public class Asset
{
    public string Id { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public AssetDefinitionSnapshot Definition { get; set; } = new();
    public PurchaseInfo? PurchaseInfo { get; set; }    public string? LocationId { get; set; }    public AssetLocation? Location { get; set; }
    public AssetAssignment? Assignment { get; set; }
    public string? ParentAssetId { get; set; }
    public List<string> ChildAssetIds { get; set; } = [];
    public string Notes { get; set; } = string.Empty;
    public List<MaintenanceRecord> MaintenanceHistory { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string? ArchivedBy { get; set; }
}

public class AssetDefinitionSnapshot
{
    public string DefinitionId { get; set; } = string.Empty;
    public bool IsCustomized { get; set; }
    public DateTime SnapshotAt { get; set; } = DateTime.UtcNow;
    public string Name { get; set; } = string.Empty;
    public string AssetTypeId { get; set; } = string.Empty;
    public string AssetTypeName { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public List<AssetSpecification> Specifications { get; set; } = [];
}

public class PurchaseInfo
{
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string Currency { get; set; } = "USD";
    public string Vendor { get; set; } = string.Empty;
    public DateTime? WarrantyExpiry { get; set; }
}

public class AssetLocation
{
    public string Building { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Desk { get; set; } = string.Empty;
}

public class AssetAssignment
{
    public string? AssignedToUserId { get; set; }
    public string AssignedToName { get; set; } = string.Empty;
    public DateTime? AssignedAt { get; set; }
    public string AssignmentType { get; set; } = "permanent";
}

public class MaintenanceRecord
{
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public decimal? Cost { get; set; }
}
