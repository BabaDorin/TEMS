using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChangeLog.Infrastructure.Entities;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(
    typeof(TicketCreatedLogEntity),
    typeof(TicketUpdatedLogEntity),
    typeof(TicketApprovalGateAddedLogEntity),
    typeof(TicketApprovalGateRemovedLogEntity),
    typeof(TicketStatusUpdatedLogEntity),
    typeof(TicketApprovalGateReviewedLogEntity)
)]
public class TicketChangeLogEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("tenant_id")]
    public string TenantId { get; set; } = string.Empty;

    [BsonElement("action")]
    public string Action { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }

    [BsonElement("performed_by_user_id")]
    public string? PerformedByUserId { get; set; }

    [BsonElement("performed_by_user_name")]
    public string? PerformedByUserName { get; set; }

    [BsonElement("ticket_id")]
    public string TicketId { get; set; } = string.Empty;

    [BsonElement("human_readable_id")]
    public string HumanReadableId { get; set; } = string.Empty;
}

public class TicketCreatedLogEntity : TicketChangeLogEntity
{
    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("summary")]
    public string Summary { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;
}

public class TicketUpdatedLogEntity : TicketChangeLogEntity
{
    [BsonElement("changes")]
    public List<FieldChangeEntity> Changes { get; set; } = [];
}

public class TicketApprovalGateAddedLogEntity : TicketChangeLogEntity
{
    [BsonElement("approval_gate_id")]
    public string ApprovalGateId { get; set; } = string.Empty;

    [BsonElement("gate_title")]
    public string GateTitle { get; set; } = string.Empty;

    [BsonElement("justification")]
    public string Justification { get; set; } = string.Empty;

    [BsonElement("all_approvers_required")]
    public bool AllApproversRequired { get; set; }

    [BsonElement("approvers")]
    public List<TicketApprovalGateApproverInfoEntity> Approvers { get; set; } = [];
}

public class TicketApprovalGateRemovedLogEntity : TicketChangeLogEntity
{
    [BsonElement("approval_gate_id")]
    public string ApprovalGateId { get; set; } = string.Empty;

    [BsonElement("gate_title")]
    public string GateTitle { get; set; } = string.Empty;

    [BsonElement("justification")]
    public string Justification { get; set; } = string.Empty;

    [BsonElement("all_approvers_required")]
    public bool AllApproversRequired { get; set; }

    [BsonElement("approvers")]
    public List<TicketApprovalGateApproverInfoEntity> Approvers { get; set; } = [];
}

public class TicketStatusUpdatedLogEntity : TicketChangeLogEntity
{
    [BsonElement("previous_status")]
    public string PreviousStatus { get; set; } = string.Empty;

    [BsonElement("new_status")]
    public string NewStatus { get; set; } = string.Empty;
}

public class TicketApprovalGateReviewedLogEntity : TicketChangeLogEntity
{
    [BsonElement("approval_gate_id")]
    public string ApprovalGateId { get; set; } = string.Empty;

    [BsonElement("gate_title")]
    public string GateTitle { get; set; } = string.Empty;

    [BsonElement("review_status")]
    public string ReviewStatus { get; set; } = string.Empty;

    [BsonElement("gate_state")]
    public string GateState { get; set; } = string.Empty;

    [BsonElement("approver_user_id")]
    public string ApproverUserId { get; set; } = string.Empty;

    [BsonElement("approver_name")]
    public string ApproverName { get; set; } = string.Empty;
}

public class TicketApprovalGateApproverInfoEntity
{
    [BsonElement("user_id")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;
}
