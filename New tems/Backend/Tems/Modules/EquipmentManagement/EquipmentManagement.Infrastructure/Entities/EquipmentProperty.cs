using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EquipmentManagement.Infrastructure.Entities;

public class EquipmentProperty
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

    [BsonElement("required")]
    public bool Required { get; set; }

    [BsonElement("validation")]
    public PropertyValidation? Validation { get; set; }

    [BsonElement("display_order")]
    public int DisplayOrder { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
