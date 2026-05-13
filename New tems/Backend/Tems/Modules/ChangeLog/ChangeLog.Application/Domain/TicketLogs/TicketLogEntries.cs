using ChangeLog.Contract.Enums;

namespace ChangeLog.Application.Domain.TicketLogs;

public class TicketCreatedLog : ChangeLogEntry
{
    public string TicketId { get; set; } = string.Empty;
    public string HumanReadableId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Ticket;
    public override string EntityId => TicketId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["ticketId"] = TicketId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["humanReadableId"] = HumanReadableId,
        ["title"] = Title,
        ["summary"] = Summary,
        ["status"] = Status
    };
}

public class TicketUpdatedLog : ChangeLogEntry
{
    public string TicketId { get; set; } = string.Empty;
    public string HumanReadableId { get; set; } = string.Empty;
    public List<FieldChange> Changes { get; set; } = [];

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Ticket;
    public override string EntityId => TicketId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["ticketId"] = TicketId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["humanReadableId"] = HumanReadableId,
        ["changes"] = Changes
    };
}

public class TicketApprovalGateAddedLog : ChangeLogEntry
{
    public string TicketId { get; set; } = string.Empty;
    public string HumanReadableId { get; set; } = string.Empty;
    public string ApprovalGateId { get; set; } = string.Empty;
    public string GateTitle { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
    public bool AllApproversRequired { get; set; }
    public List<TicketApprovalGateApproverInfo> Approvers { get; set; } = [];

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Ticket;
    public override string EntityId => TicketId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["ticketId"] = TicketId,
        ["approvalGateId"] = ApprovalGateId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["humanReadableId"] = HumanReadableId,
        ["gateTitle"] = GateTitle,
        ["justification"] = Justification,
        ["allApproversRequired"] = AllApproversRequired,
        ["approvers"] = Approvers
    };
}

public class TicketApprovalGateRemovedLog : ChangeLogEntry
{
    public string TicketId { get; set; } = string.Empty;
    public string HumanReadableId { get; set; } = string.Empty;
    public string ApprovalGateId { get; set; } = string.Empty;
    public string GateTitle { get; set; } = string.Empty;
    public string Justification { get; set; } = string.Empty;
    public bool AllApproversRequired { get; set; }
    public List<TicketApprovalGateApproverInfo> Approvers { get; set; } = [];

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Ticket;
    public override string EntityId => TicketId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["ticketId"] = TicketId,
        ["approvalGateId"] = ApprovalGateId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["humanReadableId"] = HumanReadableId,
        ["gateTitle"] = GateTitle,
        ["justification"] = Justification,
        ["allApproversRequired"] = AllApproversRequired,
        ["approvers"] = Approvers
    };
}

public class TicketStatusUpdatedLog : ChangeLogEntry
{
    public string TicketId { get; set; } = string.Empty;
    public string HumanReadableId { get; set; } = string.Empty;
    public string PreviousStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Ticket;
    public override string EntityId => TicketId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["ticketId"] = TicketId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["humanReadableId"] = HumanReadableId,
        ["previousStatus"] = PreviousStatus,
        ["newStatus"] = NewStatus
    };
}

public class TicketApprovalGateReviewedLog : ChangeLogEntry
{
    public string TicketId { get; set; } = string.Empty;
    public string HumanReadableId { get; set; } = string.Empty;
    public string ApprovalGateId { get; set; } = string.Empty;
    public string GateTitle { get; set; } = string.Empty;
    public string ReviewStatus { get; set; } = string.Empty;
    public string GateState { get; set; } = string.Empty;
    public string ApproverUserId { get; set; } = string.Empty;
    public string ApproverName { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Ticket;
    public override string EntityId => TicketId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["ticketId"] = TicketId,
        ["approvalGateId"] = ApprovalGateId,
        ["approverUserId"] = ApproverUserId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["humanReadableId"] = HumanReadableId,
        ["gateTitle"] = GateTitle,
        ["reviewStatus"] = ReviewStatus,
        ["gateState"] = GateState,
        ["approverName"] = ApproverName
    };
}

public class TicketApprovalGateApproverInfo
{
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
