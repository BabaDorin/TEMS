using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AssetManagement.Infrastructure.Entities;

public class AssetType
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("parent_type_id")]
    [BsonRepresentation(BsonType.String)]
    public string? ParentTypeId { get; set; }

    [BsonElement("properties")]
    public List<AssetTypeProperty> Properties { get; set; } = [];

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

public class AssetTypeProperty
{
    [BsonElement("property_id")]
    [BsonRepresentation(BsonType.String)]
    public string PropertyId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("data_type")]
    public string DataType { get; set; } = string.Empty;

    [BsonElement("required")]
    public bool Required { get; set; }

    [BsonElement("validation")]
    public PropertyValidation? Validation { get; set; }

    [BsonElement("display_order")]
    public int DisplayOrder { get; set; }
}
