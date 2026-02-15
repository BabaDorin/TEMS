using ChangeLog.Application.Domain;
using ChangeLog.Application.Domain.AssetLogs;
using ChangeLog.Application.Domain.LocationLogs;
using ChangeLog.Application.Domain.UserLogs;
using ChangeLog.Contract.Enums;
using ChangeLog.Infrastructure.Entities;

namespace ChangeLog.Infrastructure.Mappers;

public static class AssetChangeLogMapper
{
    public static AssetChangeLogEntity ToDatabase(this ChangeLogEntry entry)
    {
        return entry switch
        {
            AssetCreatedLog e => new AssetCreatedLogEntity
            {
                Id = e.Id,
                TenantId = e.TenantId,
                Action = e.Action.ToString(),
                Description = e.Description,
                Timestamp = e.Timestamp,
                PerformedByUserId = e.PerformedByUserId,
                PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId,
                AssetTag = e.AssetTag,
                DefinitionName = e.DefinitionName,
                AssetTypeName = e.AssetTypeName,
                Status = e.Status
            },
            AssetUpdatedLog e => new AssetUpdatedLogEntity
            {
                Id = e.Id,
                TenantId = e.TenantId,
                Action = e.Action.ToString(),
                Description = e.Description,
                Timestamp = e.Timestamp,
                PerformedByUserId = e.PerformedByUserId,
                PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId,
                AssetTag = e.AssetTag,
                Changes = e.Changes.Select(c => new FieldChangeEntity
                {
                    FieldName = c.FieldName,
                    OldValue = c.OldValue,
                    NewValue = c.NewValue
                }).ToList()
            },
            AssetDeletedLog e => new AssetDeletedLogEntity
            {
                Id = e.Id,
                TenantId = e.TenantId,
                Action = e.Action.ToString(),
                Description = e.Description,
                Timestamp = e.Timestamp,
                PerformedByUserId = e.PerformedByUserId,
                PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId,
                AssetTag = e.AssetTag
            },
            AssetAssignedToUserLog e => new AssetAssignedToUserLogEntity
            {
                Id = e.Id,
                TenantId = e.TenantId,
                Action = e.Action.ToString(),
                Description = e.Description,
                Timestamp = e.Timestamp,
                PerformedByUserId = e.PerformedByUserId,
                PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId,
                AssetTag = e.AssetTag,
                UserId = e.UserId,
                UserName = e.UserName,
                PreviousUserId = e.PreviousUserId,
                PreviousUserName = e.PreviousUserName
            },
            AssetUnassignedFromUserLog e => new AssetUnassignedFromUserLogEntity
            {
                Id = e.Id,
                TenantId = e.TenantId,
                Action = e.Action.ToString(),
                Description = e.Description,
                Timestamp = e.Timestamp,
                PerformedByUserId = e.PerformedByUserId,
                PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId,
                AssetTag = e.AssetTag,
                UserId = e.UserId,
                UserName = e.UserName,
                Reason = e.Reason
            },
            AssetAssignedToLocationLog e => new AssetAssignedToLocationLogEntity
            {
                Id = e.Id,
                TenantId = e.TenantId,
                Action = e.Action.ToString(),
                Description = e.Description,
                Timestamp = e.Timestamp,
                PerformedByUserId = e.PerformedByUserId,
                PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId,
                AssetTag = e.AssetTag,
                LocationId = e.LocationId,
                LocationName = e.LocationName,
                PreviousLocationId = e.PreviousLocationId,
                PreviousLocationName = e.PreviousLocationName
            },
            AssetUnassignedFromLocationLog e => new AssetUnassignedFromLocationLogEntity
            {
                Id = e.Id,
                TenantId = e.TenantId,
                Action = e.Action.ToString(),
                Description = e.Description,
                Timestamp = e.Timestamp,
                PerformedByUserId = e.PerformedByUserId,
                PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId,
                AssetTag = e.AssetTag,
                LocationId = e.LocationId,
                LocationName = e.LocationName,
                Reason = e.Reason
            },
            _ => throw new InvalidOperationException($"Unknown asset log type: {entry.GetType().Name}")
        };
    }

    public static ChangeLogEntry ToDomain(this AssetChangeLogEntity entity)
    {
        var action = Enum.Parse<ChangeLogAction>(entity.Action);

        return entity switch
        {
            AssetCreatedLogEntity e => new AssetCreatedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId, AssetTag = e.AssetTag, DefinitionName = e.DefinitionName,
                AssetTypeName = e.AssetTypeName, Status = e.Status
            },
            AssetUpdatedLogEntity e => new AssetUpdatedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId, AssetTag = e.AssetTag,
                Changes = e.Changes.Select(c => new FieldChange { FieldName = c.FieldName, OldValue = c.OldValue, NewValue = c.NewValue }).ToList()
            },
            AssetDeletedLogEntity e => new AssetDeletedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId, AssetTag = e.AssetTag
            },
            AssetAssignedToUserLogEntity e => new AssetAssignedToUserLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId, AssetTag = e.AssetTag, UserId = e.UserId, UserName = e.UserName,
                PreviousUserId = e.PreviousUserId, PreviousUserName = e.PreviousUserName
            },
            AssetUnassignedFromUserLogEntity e => new AssetUnassignedFromUserLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId, AssetTag = e.AssetTag, UserId = e.UserId, UserName = e.UserName, Reason = e.Reason
            },
            AssetAssignedToLocationLogEntity e => new AssetAssignedToLocationLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId, AssetTag = e.AssetTag, LocationId = e.LocationId, LocationName = e.LocationName,
                PreviousLocationId = e.PreviousLocationId, PreviousLocationName = e.PreviousLocationName
            },
            AssetUnassignedFromLocationLogEntity e => new AssetUnassignedFromLocationLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                AssetId = e.AssetId, AssetTag = e.AssetTag, LocationId = e.LocationId, LocationName = e.LocationName, Reason = e.Reason
            },
            _ => throw new InvalidOperationException($"Unknown entity type: {entity.GetType().Name}")
        };
    }
}

public static class UserChangeLogMapper
{
    public static UserChangeLogEntity ToDatabase(this ChangeLogEntry entry)
    {
        return entry switch
        {
            UserCreatedLog e => new UserCreatedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName, Email = e.Email
            },
            UserDeletedLog e => new UserDeletedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName
            },
            UserRolesUpdatedLog e => new UserRolesUpdatedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName, AddedRoles = e.AddedRoles, RemovedRoles = e.RemovedRoles
            },
            UserAssetAssignedLog e => new UserAssetAssignedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName, AssetId = e.AssetId, AssetTag = e.AssetTag
            },
            UserAssetUnassignedLog e => new UserAssetUnassignedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName, AssetId = e.AssetId, AssetTag = e.AssetTag, Reason = e.Reason
            },
            _ => throw new InvalidOperationException($"Unknown user log type: {entry.GetType().Name}")
        };
    }

    public static ChangeLogEntry ToDomain(this UserChangeLogEntity entity)
    {
        var action = Enum.Parse<ChangeLogAction>(entity.Action);

        return entity switch
        {
            UserCreatedLogEntity e => new UserCreatedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName, Email = e.Email
            },
            UserDeletedLogEntity e => new UserDeletedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName
            },
            UserRolesUpdatedLogEntity e => new UserRolesUpdatedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName, AddedRoles = e.AddedRoles, RemovedRoles = e.RemovedRoles
            },
            UserAssetAssignedLogEntity e => new UserAssetAssignedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName, AssetId = e.AssetId, AssetTag = e.AssetTag
            },
            UserAssetUnassignedLogEntity e => new UserAssetUnassignedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                TargetUserId = e.TargetUserId, UserName = e.UserName, AssetId = e.AssetId, AssetTag = e.AssetTag, Reason = e.Reason
            },
            _ => throw new InvalidOperationException($"Unknown entity type: {entity.GetType().Name}")
        };
    }
}

public static class LocationChangeLogMapper
{
    public static LocationChangeLogEntity ToDatabase(this ChangeLogEntry entry)
    {
        return entry switch
        {
            LocationCreatedLog e => new LocationCreatedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, LocationType = e.LocationType,
                ParentId = e.ParentId, ParentName = e.ParentName
            },
            LocationUpdatedLog e => new LocationUpdatedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, LocationType = e.LocationType,
                Changes = e.Changes.Select(c => new FieldChangeEntity { FieldName = c.FieldName, OldValue = c.OldValue, NewValue = c.NewValue }).ToList()
            },
            LocationDeletedLog e => new LocationDeletedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, LocationType = e.LocationType
            },
            LocationAssetAssignedLog e => new LocationAssetAssignedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, AssetId = e.AssetId, AssetTag = e.AssetTag
            },
            LocationAssetUnassignedLog e => new LocationAssetUnassignedLogEntity
            {
                Id = e.Id, TenantId = e.TenantId, Action = e.Action.ToString(), Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, AssetId = e.AssetId, AssetTag = e.AssetTag, Reason = e.Reason
            },
            _ => throw new InvalidOperationException($"Unknown location log type: {entry.GetType().Name}")
        };
    }

    public static ChangeLogEntry ToDomain(this LocationChangeLogEntity entity)
    {
        var action = Enum.Parse<ChangeLogAction>(entity.Action);

        return entity switch
        {
            LocationCreatedLogEntity e => new LocationCreatedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, LocationType = e.LocationType,
                ParentId = e.ParentId, ParentName = e.ParentName
            },
            LocationUpdatedLogEntity e => new LocationUpdatedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, LocationType = e.LocationType,
                Changes = e.Changes.Select(c => new FieldChange { FieldName = c.FieldName, OldValue = c.OldValue, NewValue = c.NewValue }).ToList()
            },
            LocationDeletedLogEntity e => new LocationDeletedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, LocationType = e.LocationType
            },
            LocationAssetAssignedLogEntity e => new LocationAssetAssignedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, AssetId = e.AssetId, AssetTag = e.AssetTag
            },
            LocationAssetUnassignedLogEntity e => new LocationAssetUnassignedLog
            {
                Id = e.Id, TenantId = e.TenantId, Action = action, Description = e.Description,
                Timestamp = e.Timestamp, PerformedByUserId = e.PerformedByUserId, PerformedByUserName = e.PerformedByUserName,
                LocationId = e.LocationId, LocationName = e.LocationName, AssetId = e.AssetId, AssetTag = e.AssetTag, Reason = e.Reason
            },
            _ => throw new InvalidOperationException($"Unknown entity type: {entity.GetType().Name}")
        };
    }
}
