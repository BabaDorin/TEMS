namespace TicketManagement.Application.Domain;

public class ApprovalGate
{
    public string ApprovalGateId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
    public string State { get; set; } = "pending";
    public bool AllApproversRequired { get; set; }
    public List<ApprovalGateApprover> Approvers { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ApprovalGateApprover
{
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public DateTime? ReviewedAt { get; set; }
}
