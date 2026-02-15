using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChangeLog.Infrastructure.Entities;

[BsonDiscriminator(RootClass = true)]
[BsonKnownTypes(
    typeof(UserCreatedLogEntity),
    typeof(UserDeletedLogEntity),
    typeof(UserRolesUpdatedLogEntity),
    typeof(UserAssetAssignedLogEntity),
    typeof(UserAssetUnassignedLogEntity)
)]
public class UserChangeLogEntity
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

    [BsonElement("target_user_id")]
    public string TargetUserId { get; set; } = string.Empty;

    [BsonElement("user_name")]
    public string UserName { get; set; } = string.Empty;
}

public class UserCreatedLogEntity : UserChangeLogEntity
{
    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;
}

public class UserDeletedLogEntity : UserChangeLogEntity;

public class UserRolesUpdatedLogEntity : UserChangeLogEntity
{
    [BsonElement("added_roles")]
    public List<string> AddedRoles { get; set; } = [];

    [BsonElement("removed_roles")]
    public List<string> RemovedRoles { get; set; } = [];
}

public class UserAssetAssignedLogEntity : UserChangeLogEntity
{
    [BsonElement("asset_id")]
    public string AssetId { get; set; } = string.Empty;

    [BsonElement("asset_tag")]
    public string AssetTag { get; set; } = string.Empty;
}

public class UserAssetUnassignedLogEntity : UserChangeLogEntity
{
    [BsonElement("asset_id")]
    public string AssetId { get; set; } = string.Empty;

    [BsonElement("asset_tag")]
    public string AssetTag { get; set; } = string.Empty;

    [BsonElement("reason")]
    [BsonIgnoreIfNull]
    public string? Reason { get; set; }
}
