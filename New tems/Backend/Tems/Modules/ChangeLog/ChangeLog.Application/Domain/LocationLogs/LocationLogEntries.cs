using ChangeLog.Contract.Enums;

namespace ChangeLog.Application.Domain.LocationLogs;

public class LocationCreatedLog : ChangeLogEntry
{
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public string? ParentId { get; set; }
    public string? ParentName { get; set; }

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Location;
    public override string EntityId => LocationId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["locationId"] = LocationId,
        ["parentLocationId"] = ParentId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["locationName"] = LocationName,
        ["locationType"] = LocationType,
        ["parentName"] = ParentName
    };
}

public class LocationUpdatedLog : ChangeLogEntry
{
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;
    public List<FieldChange> Changes { get; set; } = [];

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Location;
    public override string EntityId => LocationId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["locationId"] = LocationId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["locationName"] = LocationName,
        ["locationType"] = LocationType,
        ["changes"] = Changes
    };
}

public class LocationDeletedLog : ChangeLogEntry
{
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string LocationType { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Location;
    public override string EntityId => LocationId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["locationId"] = LocationId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["locationName"] = LocationName,
        ["locationType"] = LocationType
    };
}

public class LocationAssetAssignedLog : ChangeLogEntry
{
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Location;
    public override string EntityId => LocationId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["locationId"] = LocationId,
        ["assetId"] = AssetId
    };

    public override Dictionary<string, object?>? GetDetails() => new()
    {
        ["locationName"] = LocationName,
        ["assetTag"] = AssetTag
    };
}

public class LocationAssetUnassignedLog : ChangeLogEntry
{
    public string LocationId { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public string AssetId { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string? Reason { get; set; }

    public override ChangeLogEntityType EntityType => ChangeLogEntityType.Location;
    public override string EntityId => LocationId;

    public override Dictionary<string, string?> GetReferences() => new()
    {
        ["locationId"] = LocationId,
        ["assetId"] = AssetId
    };

    public override Dictionary<string, object?>? GetDetails()
    {
        var details = new Dictionary<string, object?>
        {
            ["locationName"] = LocationName,
            ["assetTag"] = AssetTag
        };
        if (!string.IsNullOrEmpty(Reason)) details["reason"] = Reason;
        return details;
    }
}
