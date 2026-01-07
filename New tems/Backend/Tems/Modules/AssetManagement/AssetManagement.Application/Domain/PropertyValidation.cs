namespace AssetManagement.Application.Domain;

public class PropertyValidation
{
    public string Type { get; set; } = string.Empty;
    public int? MaxLength { get; set; }
    public string? Pattern { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public string? Unit { get; set; }
    public List<string> EnumValues { get; set; } = [];
}
