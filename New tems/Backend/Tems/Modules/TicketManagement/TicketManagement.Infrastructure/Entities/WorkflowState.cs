using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketManagement.Infrastructure.Entities;

public class WorkflowState
{
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("label")]
    public string Label { get; set; } = string.Empty;

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("allowed_transitions")]
    public List<string> AllowedTransitions { get; set; } = new();

    [BsonElement("automation_hook")]
    [BsonIgnoreIfNull]
    public string? AutomationHook { get; set; }
}
