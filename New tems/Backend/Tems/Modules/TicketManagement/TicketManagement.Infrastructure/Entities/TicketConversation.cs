using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketManagement.Infrastructure.Entities;

[BsonIgnoreExtraElements]
public class TicketConversation
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string ConversationId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("ticket_id")]
    [BsonRepresentation(BsonType.String)]
    public string TicketId { get; set; } = string.Empty;

    [BsonElement("messages")]
    public List<TicketMessage> Messages { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
