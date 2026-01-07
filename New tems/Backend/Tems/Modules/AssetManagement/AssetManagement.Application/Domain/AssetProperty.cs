namespace AssetManagement.Application.Domain;

public class AssetProperty
{
    public string PropertyId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public PropertyValidation? DefaultValidation { get; set; }
    public List<string> EnumValues { get; set; } = [];
    public string? Unit { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}
