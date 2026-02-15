using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChangeLog.Infrastructure.Entities;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(
    typeof(LocationCreatedLogEntity),
    typeof(LocationUpdatedLogEntity),
    typeof(LocationDeletedLogEntity),
    typeof(LocationAssetAssignedLogEntity),
    typeof(LocationAssetUnassignedLogEntity)
)]
public class LocationChangeLogEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("tenant_id")]
    public string TenantId { get; set; } = string.Empty;

    [BsonElement("action")]
    public string Action { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    public DateTime Timestamp { get; set; }

    [BsonElement("performed_by_user_id")]
    public string? PerformedByUserId { get; set; }

    [BsonElement("performed_by_user_name")]
    public string? PerformedByUserName { get; set; }

    [BsonElement("location_id")]
    public string LocationId { get; set; } = string.Empty;

    [BsonElement("location_name")]
    public string LocationName { get; set; } = string.Empty;
}

public class LocationCreatedLogEntity : LocationChangeLogEntity
{
    [BsonElement("location_type")]
    public string LocationType { get; set; } = string.Empty;

    [BsonElement("parent_id")]
    public string? ParentId { get; set; }

    [BsonElement("parent_name")]
    public string? ParentName { get; set; }
}

public class LocationUpdatedLogEntity : LocationChangeLogEntity
{
    [BsonElement("location_type")]
    public string LocationType { get; set; } = string.Empty;

    [BsonElement("changes")]
    public List<FieldChangeEntity> Changes { get; set; } = [];
}

public class LocationDeletedLogEntity : LocationChangeLogEntity
{
    [BsonElement("location_type")]
    public string LocationType { get; set; } = string.Empty;
}

public class LocationAssetAssignedLogEntity : LocationChangeLogEntity
{
    [BsonElement("asset_id")]
    public string AssetId { get; set; } = string.Empty;

    [BsonElement("asset_tag")]
    public string AssetTag { get; set; } = string.Empty;
}

public class LocationAssetUnassignedLogEntity : LocationChangeLogEntity
{
    [BsonElement("asset_id")]
    public string AssetId { get; set; } = string.Empty;

    [BsonElement("asset_tag")]
    public string AssetTag { get; set; } = string.Empty;

    [BsonElement("reason")]
    [BsonIgnoreIfNull]
    public string? Reason { get; set; }
}
