using ChangeLog.Contract.Enums;

namespace ChangeLog.Application.Domain.UserLogs;

public class UserCreatedLog : ChangeLogEntry
{
    public string TargetUserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.User;
    public override string EntityId => TargetUserId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["userId"] = TargetUserId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["userName"] = UserName,
        ["email"] = Email
    };
}

public class UserDeletedLog : ChangeLogEntry
{
    public string TargetUserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.User;
    public override string EntityId => TargetUserId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["userId"] = TargetUserId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["userName"] = UserName
    };
}

public class UserRolesUpdatedLog : ChangeLogEntry
{
    public string TargetUserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> AddedRoles { get; set; } = [];
    public List<string> RemovedRoles { get; set; } = [];

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.User;
    public override string EntityId => TargetUserId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["userId"] = TargetUserId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["userName"] = UserName,
        ["addedRoles"] = AddedRoles,
        ["removedRoles"] = RemovedRoles
    };
}

public class UserAssetAssignedLog : ChangeLogEntry
{
    public string TargetUserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.User;
    public override string EntityId => TargetUserId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["userId"] = TargetUserId,
        ["assetId"] = AssetId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["userName"] = UserName,
        ["assetTag"] = AssetTag
    };
}

public class UserAssetUnassignedLog : ChangeLogEntry
{
    public string TargetUserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string? Reason { get; set; }

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.User;
    public override string EntityId => TargetUserId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["userId"] = TargetUserId,
        ["assetId"] = AssetId
    };

    public override Dictionary<string, object?>? GetDetails()
    {
        var details = new Dictionary<string, object?>
        {
            ["userName"] = UserName,
            ["assetTag"] = AssetTag
        };
        if (!string.IsNullOrEmpty(Reason)) details["reason"] = Reason;
        return details;
    }
}
