using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketManagement.Infrastructure.Entities;

[BsonIgnoreExtraElements]
public class ApprovalGate
{
    [BsonElement("approval_gate_id")]
    [BsonRepresentation(BsonType.String)]
    public string ApprovalGateId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("justification")]
    public string Justification { get; set; } = string.Empty;

    [BsonElement("state")]
    public string State { get; set; } = "pending";

    [BsonElement("all_approvers_required")]
    public bool AllApproversRequired { get; set; }

    [BsonElement("approvers")]
    public List<ApprovalGateApprover> Approvers { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[BsonIgnoreExtraElements]
public class ApprovalGateApprover
{
    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.String)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("status")]
    public string Status { get; set; } = "pending";

    [BsonElement("reviewed_at")]
    [BsonIgnoreIfNull]
    public DateTime? ReviewedAt { get; set; }
}
