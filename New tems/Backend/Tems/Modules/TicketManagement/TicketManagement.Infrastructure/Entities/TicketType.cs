using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketManagement.Infrastructure.Entities;

[BsonIgnoreExtraElements]
public class TicketType
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string TicketTypeId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("tenant_id")]
    [BsonRepresentation(BsonType.String)]
    public string TenantId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("itil_category")]
    public string ItilCategory { get; set; } = string.Empty;

    [BsonElement("version")]
    public int Version { get; set; } = 1;

    [BsonElement("workflow_config")]
    public WorkflowConfig WorkflowConfig { get; set; } = new();

    [BsonElement("attribute_definitions")]
    public List<AttributeDefinition> AttributeDefinitions { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
