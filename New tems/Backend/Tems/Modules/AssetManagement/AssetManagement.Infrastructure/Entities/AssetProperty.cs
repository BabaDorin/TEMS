using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetManagement.Infrastructure.Entities;

public class AssetProperty
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string PropertyId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("data_type")]
    public string DataType { get; set; } = string.Empty;

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("default_validation")]
    public PropertyValidation? DefaultValidation { get; set; }

    [BsonElement("enum_values")]
    public List<string> EnumValues { get; set; } = [];

    [BsonElement("unit")]
    public string? Unit { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("created_by")]
    public string CreatedBy { get; set; } = string.Empty;
}
