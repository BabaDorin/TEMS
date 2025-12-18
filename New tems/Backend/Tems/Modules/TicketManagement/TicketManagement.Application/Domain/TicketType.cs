namespace TicketManagement.Application.Domain;

public class TicketType
{
    public string TicketTypeId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ItilCategory { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public WorkflowConfig WorkflowConfig { get; set; } = new();
    public List<AttributeDefinition> AttributeDefinitions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
