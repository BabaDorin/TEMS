namespace TicketManagement.Application.Domain;

public class WorkflowState
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<string> AllowedTransitions { get; set; } = new();
    public string? AutomationHook { get; set; }
}
