using MongoDB.Bson.Serialization.Attributes;

namespace TicketManagement.Infrastructure.Entities;

public class AttributeDefinition
{
    [BsonElement("key")]
    public string Key { get; set; } = string.Empty;

    [BsonElement("label")]
    public string Label { get; set; } = string.Empty;

    [BsonElement("data_type")]
    public string DataType { get; set; } = string.Empty;

    [BsonElement("is_required")]
    public bool IsRequired { get; set; }

    [BsonElement("is_predefined")]
    public bool IsPredefined { get; set; }

    [BsonElement("ai_extraction_hint")]
    [BsonIgnoreIfNull]
    public string? AiExtractionHint { get; set; }

    [BsonElement("validation_rule")]
    [BsonIgnoreIfNull]
    public string? ValidationRule { get; set; }
}
