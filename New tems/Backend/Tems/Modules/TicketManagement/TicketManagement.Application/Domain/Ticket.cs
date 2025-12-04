namespace TicketManagement.Application.Domain;

public class Ticket
{
    public string TicketId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string TicketTypeId { get; set; } = string.Empty;
    public string HumanReadableId { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string CurrentStateId { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public Reporter Reporter { get; set; } = new();
    public string? AssigneeId { get; set; }
    public Dictionary<string, object> Attributes { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
