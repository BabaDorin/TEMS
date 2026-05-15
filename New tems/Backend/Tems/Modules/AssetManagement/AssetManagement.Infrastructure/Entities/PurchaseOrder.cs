using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetManagement.Infrastructure.Entities;

[BsonIgnoreExtraElements]
public class PurchaseOrder
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("tenant_id")]
    [BsonRepresentation(BsonType.String)]
    public string TenantId { get; set; } = string.Empty;

    [BsonElement("ticket_id")]
    [BsonRepresentation(BsonType.String)]
    public string TicketId { get; set; } = string.Empty;

    [BsonElement("ticket_human_readable_id")]
    public string TicketHumanReadableId { get; set; } = string.Empty;

    [BsonElement("po_number")]
    public string PoNumber { get; set; } = string.Empty;

    [BsonElement("vendor")]
    public string Vendor { get; set; } = string.Empty;

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("currency")]
    public string Currency { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("created_by_user_id")]
    [BsonRepresentation(BsonType.String)]
    public string CreatedByUserId { get; set; } = string.Empty;

    [BsonElement("accountable_user_id")]
    [BsonRepresentation(BsonType.String)]
    public string AccountableUserId { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
