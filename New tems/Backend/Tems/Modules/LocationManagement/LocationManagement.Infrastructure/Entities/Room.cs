using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LocationManagement.Infrastructure.Entities;

public class Room
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public required string Id { get; set; }

    [BsonElement("building_id")]
    [BsonRepresentation(BsonType.String)]
    public required string BuildingId { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("room_number")]
    public string? RoomNumber { get; set; }

    [BsonElement("floor_label")]
    public string FloorLabel { get; set; } = string.Empty;

    [BsonElement("type")]
    [BsonRepresentation(BsonType.String)]
    public required string Type { get; set; }

    [BsonElement("capacity")]
    public int Capacity { get; set; }

    [BsonElement("area")]
    public double? Area { get; set; }

    [BsonElement("status")]
    [BsonRepresentation(BsonType.String)]
    public required string Status { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

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
