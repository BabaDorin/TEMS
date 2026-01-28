using MongoDB.Bson.Serialization.Attributes;

namespace AssetManagement.Infrastructure.Entities;

public class PropertyValidation
{
    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;
    
    [BsonElement("max_length")]
    public int? MaxLength { get; set; }
    
    [BsonElement("pattern")]
    public string? Pattern { get; set; }
    
    [BsonElement("min")]
    public int? Min { get; set; }
    
    [BsonElement("max")]
    public int? Max { get; set; }
    
    [BsonElement("unit")]
    public string? Unit { get; set; }
    
    [BsonElement("enum_values")]
    public List<string> EnumValues { get; set; } = [];
}
