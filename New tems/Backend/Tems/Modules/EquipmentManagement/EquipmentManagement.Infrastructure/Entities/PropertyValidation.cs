namespace EquipmentManagement.Infrastructure.Entities;

public class PropertyValidation
{
    public string Type { get; set; } = string.Empty;
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public List<string>? AllowedValues { get; set; }
}
