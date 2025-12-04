using MongoDB.Bson.Serialization.Attributes;

namespace TicketManagement.Infrastructure.Entities;

public class TicketMessage
{
    [BsonElement("sender_type")]
    public string SenderType { get; set; } = string.Empty;

    [BsonElement("sender_id")]
    public string SenderId { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("channel_message_id")]
    [BsonIgnoreIfNull]
    public string? ChannelMessageId { get; set; }

    [BsonElement("is_internal_note")]
    public bool IsInternalNote { get; set; }
}
