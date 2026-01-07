using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetManagement.Infrastructure.Entities;

public class AssetDefinition
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("short_name")]
    public string ShortName { get; set; } = string.Empty;

    [BsonElement("asset_type_id")]
    [BsonRepresentation(BsonType.String)]
    public string AssetTypeId { get; set; } = string.Empty;

    [BsonElement("asset_type_name")]
    public string AssetTypeName { get; set; } = string.Empty;

    [BsonElement("manufacturer")]
    public string Manufacturer { get; set; } = string.Empty;

    [BsonElement("model")]
    public string Model { get; set; } = string.Empty;

    [BsonElement("specifications")]
    public List<AssetSpecification> Specifications { get; set; } = [];

    [BsonElement("usage_count")]
    public int UsageCount { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("notes")]
    public string Notes { get; set; } = string.Empty;

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = [];

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [BsonElement("is_archived")]
    public bool IsArchived { get; set; }

    [BsonElement("archived_at")]
    public DateTime? ArchivedAt { get; set; }

    [BsonElement("archived_by")]
    public string? ArchivedBy { get; set; }
}

public class AssetSpecification
{
    [BsonElement("property_id")]
    [BsonRepresentation(BsonType.String)]
    public string PropertyId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("value")]
    public object Value { get; set; } = string.Empty;

    [BsonElement("data_type")]
    public string DataType { get; set; } = string.Empty;

    [BsonElement("unit")]
    public string? Unit { get; set; }
}
