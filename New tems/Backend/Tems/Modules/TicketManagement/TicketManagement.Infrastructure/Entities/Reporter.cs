using MongoDB.Bson.Serialization.Attributes;

namespace TicketManagement.Infrastructure.Entities;

public class Reporter
{
    [BsonElement("user_id")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("channel_source")]
    public string ChannelSource { get; set; } = string.Empty;

    [BsonElement("channel_thread_id")]
    [BsonIgnoreIfNull]
    public string? ChannelThreadId { get; set; }
}
