namespace AssetManagement.Application.Domain;

public class AssetDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string AssetTypeId { get; set; } = string.Empty;
    public string AssetTypeName { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public List<AssetSpecification> Specifications { get; set; } = [];
    public int UsageCount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public string? ArchivedBy { get; set; }
}

public class AssetSpecification
{
    public string PropertyId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public object Value { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string? Unit { get; set; }
}
