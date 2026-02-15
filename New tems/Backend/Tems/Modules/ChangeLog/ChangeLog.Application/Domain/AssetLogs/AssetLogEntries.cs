using ChangeLog.Contract.Enums;

namespace ChangeLog.Application.Domain.AssetLogs;

public class AssetCreatedLog : ChangeLogEntry
{
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string DefinitionName { get; set; } = string.Empty;
    public string? AssetTypeName { get; set; }
    public string Status { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Asset;
    public override string EntityId => AssetId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["assetId"] = AssetId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["assetTag"] = AssetTag,
        ["definitionName"] = DefinitionName,
        ["assetTypeName"] = AssetTypeName,
        ["status"] = Status
    };
}

public class AssetUpdatedLog : ChangeLogEntry
{
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public List<FieldChange> Changes { get; set; } = [];

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Asset;
    public override string EntityId => AssetId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["assetId"] = AssetId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["assetTag"] = AssetTag,
        ["changes"] = Changes
    };
}

public class AssetDeletedLog : ChangeLogEntry
{
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Asset;
    public override string EntityId => AssetId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["assetId"] = AssetId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["assetTag"] = AssetTag
    };
}

public class AssetAssignedToUserLog : ChangeLogEntry
{
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? PreviousUserId { get; set; }
    public string? PreviousUserName { get; set; }

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Asset;
    public override string EntityId => AssetId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["assetId"] = AssetId,
        ["userId"] = UserId,
        ["previousUserId"] = PreviousUserId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["assetTag"] = AssetTag,
        ["userName"] = UserName,
        ["previousUserName"] = PreviousUserName
    };
}

public class AssetUnassignedFromUserLog : ChangeLogEntry
{
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Reason { get; set; }

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Asset;
    public override string EntityId => AssetId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["assetId"] = AssetId,
        ["userId"] = UserId
    };

    public override Dictionary<string, object?>? GetDetails()
    {
        var details = new Dictionary<string, object?>
        {
            ["assetTag"] = AssetTag,
            ["userName"] = UserName
        };
        if (!string.IsNullOrEmpty(Reason)) details["reason"] = Reason;
        return details;
    }
}

public class AssetAssignedToLocationLog : ChangeLogEntry
{
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string? PreviousLocationId { get; set; }
    public string? PreviousLocationName { get; set; }

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Asset;
    public override string EntityId => AssetId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["assetId"] = AssetId,
        ["locationId"] = LocationId,
        ["previousLocationId"] = PreviousLocationId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["assetTag"] = AssetTag,
        ["locationName"] = LocationName,
        ["previousLocationName"] = PreviousLocationName
    };
}

public class AssetUnassignedFromLocationLog : ChangeLogEntry
{
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string? Reason { get; set; }

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Asset;
    public override string EntityId => AssetId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["assetId"] = AssetId,
        ["locationId"] = LocationId
    };

    public override Dictionary<string, object?>? GetDetails()
    {
        var details = new Dictionary<string, object?>
        {
            ["assetTag"] = AssetTag,
            ["locationName"] = LocationName
        };
        if (!string.IsNullOrEmpty(Reason)) details["reason"] = Reason;
        return details;
    }
}
