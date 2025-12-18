namespace TicketManagement.Application.Domain;

public class WorkflowConfig
{
    public List<WorkflowState> States { get; set; } = new();
    public string InitialStateId { get; set; } = string.Empty;
}
