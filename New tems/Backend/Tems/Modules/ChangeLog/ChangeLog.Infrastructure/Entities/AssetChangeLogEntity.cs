using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChangeLog.Infrastructure.Entities;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(
    typeof(AssetCreatedLogEntity),
    typeof(AssetUpdatedLogEntity),
    typeof(AssetDeletedLogEntity),
    typeof(AssetAssignedToUserLogEntity),
    typeof(AssetUnassignedFromUserLogEntity),
    typeof(AssetAssignedToLocationLogEntity),
    typeof(AssetUnassignedFromLocationLogEntity)
)]
public class AssetChangeLogEntity
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

    [BsonElement("asset_id")]
    public string AssetId { get; set; } = string.Empty;

    [BsonElement("asset_tag")]
    public string AssetTag { get; set; } = string.Empty;
}

public class AssetCreatedLogEntity : AssetChangeLogEntity
{
    [BsonElement("definition_name")]
    public string DefinitionName { get; set; } = string.Empty;

    [BsonElement("asset_type_name")]
    public string? AssetTypeName { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = string.Empty;
}

public class AssetUpdatedLogEntity : AssetChangeLogEntity
{
    [BsonElement("changes")]
    public List<FieldChangeEntity> Changes { get; set; } = [];
}

public class AssetDeletedLogEntity : AssetChangeLogEntity;

public class AssetAssignedToUserLogEntity : AssetChangeLogEntity
{
    [BsonElement("user_id")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("user_name")]
    public string UserName { get; set; } = string.Empty;

    [BsonElement("previous_user_id")]
    public string? PreviousUserId { get; set; }

    [BsonElement("previous_user_name")]
    public string? PreviousUserName { get; set; }
}

public class AssetUnassignedFromUserLogEntity : AssetChangeLogEntity
{
    [BsonElement("user_id")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("user_name")]
    public string UserName { get; set; } = string.Empty;

    [BsonElement("reason")]
    [BsonIgnoreIfNull]
    public string? Reason { get; set; }
}

public class AssetAssignedToLocationLogEntity : AssetChangeLogEntity
{
    [BsonElement("location_id")]
    public string LocationId { get; set; } = string.Empty;

    [BsonElement("location_name")]
    public string LocationName { get; set; } = string.Empty;

    [BsonElement("previous_location_id")]
    public string? PreviousLocationId { get; set; }

    [BsonElement("previous_location_name")]
    public string? PreviousLocationName { get; set; }
}

public class AssetUnassignedFromLocationLogEntity : AssetChangeLogEntity
{
    [BsonElement("location_id")]
    public string LocationId { get; set; } = string.Empty;

    [BsonElement("location_name")]
    public string LocationName { get; set; } = string.Empty;

    [BsonElement("reason")]
    [BsonIgnoreIfNull]
    public string? Reason { get; set; }
}

public class FieldChangeEntity
{
    [BsonElement("field_name")]
    public string FieldName { get; set; } = string.Empty;

    [BsonElement("old_value")]
    public string? OldValue { get; set; }

    [BsonElement("new_value")]
    public string? NewValue { get; set; }
}
