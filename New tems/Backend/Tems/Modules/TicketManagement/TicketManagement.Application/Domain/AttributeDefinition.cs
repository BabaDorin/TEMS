namespace TicketManagement.Application.Domain;

public class AttributeDefinition
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsPredefined { get; set; }
    public List<string>? Options { get; set; }
    public string? AiExtractionHint { get; set; }
    public string? ValidationRule { get; set; }
}
