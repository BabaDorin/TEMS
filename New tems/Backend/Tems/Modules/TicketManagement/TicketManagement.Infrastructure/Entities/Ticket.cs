using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketManagement.Infrastructure.Entities;

[BsonIgnoreExtraElements]
public class Ticket
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string TicketId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("tenant_id")]
    [BsonRepresentation(BsonType.String)]
    public string TenantId { get; set; } = string.Empty;

    [BsonElement("ticket_type_id")]
    [BsonRepresentation(BsonType.String)]
    public string TicketTypeId { get; set; } = string.Empty;

    [BsonElement("human_readable_id")]
    public string HumanReadableId { get; set; } = string.Empty;

    [BsonElement("summary")]
    public string Summary { get; set; } = string.Empty;

    [BsonElement("current_state_id")]
    public string CurrentStateId { get; set; } = string.Empty;

    [BsonElement("priority")]
    public string Priority { get; set; } = string.Empty;

    [BsonElement("reporter")]
    public Reporter Reporter { get; set; } = new();

    [BsonElement("assignee_id")]
    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.String)]
    public string? AssigneeId { get; set; }

    [BsonElement("attributes")]
    public Dictionary<string, object> Attributes { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("resolved_at")]
    [BsonIgnoreIfNull]
    public DateTime? ResolvedAt { get; set; }
}
