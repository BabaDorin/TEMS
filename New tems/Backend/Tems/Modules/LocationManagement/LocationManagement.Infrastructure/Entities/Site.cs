using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LocationManagement.Infrastructure.Entities;

public class Site
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required string Id { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("code")]
    public required string Code { get; set; }

    [BsonElement("timezone")]
    public required string Timezone { get; set; }

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;

    [BsonElement("tenant_id")]
    [BsonRepresentation(BsonType.String)]
    public required string TenantId { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [BsonElement("updated_by")]
    public string UpdatedBy { get; set; } = string.Empty;
}
