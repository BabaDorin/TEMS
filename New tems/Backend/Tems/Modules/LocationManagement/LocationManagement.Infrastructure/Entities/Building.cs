using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LocationManagement.Infrastructure.Entities;

public class Building
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required string Id { get; set; }

    [BsonElement("site_id")]
    [BsonRepresentation(BsonType.String)]
    public required string SiteId { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("address_line")]
    public string AddressLine { get; set; } = string.Empty;

    [BsonElement("manager_contact")]
    public string ManagerContact { get; set; } = string.Empty;

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
