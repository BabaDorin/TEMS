using ChangeLog.Application.Domain;
using ChangeLog.Application.Domain.AssetLogs;
using ChangeLog.Application.Domain.LocationLogs;
using ChangeLog.Application.Domain.UserLogs;
using ChangeLog.Application.Interfaces;
using ChangeLog.Contract.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Notifications;
using Tems.Common.Tenant;

namespace ChangeLog.Application.Handlers;

public class AssetCreatedNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<AssetCreatedNotificationHandler> logger
) : INotificationHandler<AssetCreatedNotification>
{
    public async Task Handle(AssetCreatedNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging AssetCreated for {AssetTag}", n.AssetTag);

        await repository.CreateAsync(new AssetCreatedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.AssetCreated,
            Description = $"Asset '{n.AssetTag}' was created",
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            DefinitionName = n.DefinitionName,
            AssetTypeName = n.AssetTypeName,
            Status = n.Status,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}

public class AssetUpdatedNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<AssetUpdatedNotificationHandler> logger
) : INotificationHandler<AssetUpdatedNotification>
{
    public async Task Handle(AssetUpdatedNotification n, CancellationToken ct)
    {
        if (n.Changes.Count == 0) return;

        logger.LogInformation("Logging AssetUpdated for {AssetTag} with {Count} changes", n.AssetTag, n.Changes.Count);

        await repository.CreateAsync(new AssetUpdatedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.AssetUpdated,
            Description = $"Asset '{n.AssetTag}' was updated",
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            Changes = n.Changes.Select(c => new FieldChange
            {
                FieldName = c.FieldName,
                OldValue = c.OldValue,
                NewValue = c.NewValue
            }).ToList(),
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}

public class AssetDeletedNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<AssetDeletedNotificationHandler> logger
) : INotificationHandler<AssetDeletedNotification>
{
    public async Task Handle(AssetDeletedNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging AssetDeleted for {AssetTag}", n.AssetTag);

        await repository.CreateAsync(new AssetDeletedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.AssetDeleted,
            Description = $"Asset '{n.AssetTag}' was deleted",
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}

public class AssetAssignedToUserNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<AssetAssignedToUserNotificationHandler> logger
) : INotificationHandler<AssetAssignedToUserNotification>
{
    public async Task Handle(AssetAssignedToUserNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging AssetAssignedToUser: {AssetTag} → {UserName}", n.AssetTag, n.UserName);

        await repository.CreateAsync(new AssetAssignedToUserLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.AssetAssignedToUser,
            Description = $"Asset '{n.AssetTag}' was assigned to {n.UserName}",
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            UserId = n.UserId,
            UserName = n.UserName,
            PreviousUserId = n.PreviousUserId,
            PreviousUserName = n.PreviousUserName,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);

        await repository.CreateAsync(new UserAssetAssignedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.UserAssetAssigned,
            Description = $"Asset '{n.AssetTag}' was assigned to this user",
            TargetUserId = n.UserId,
            UserName = n.UserName,
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}

public class AssetUnassignedFromUserNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<AssetUnassignedFromUserNotificationHandler> logger
) : INotificationHandler<AssetUnassignedFromUserNotification>
{
    public async Task Handle(AssetUnassignedFromUserNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging AssetUnassignedFromUser: {AssetTag} ← {UserName}", n.AssetTag, n.UserName);

        await repository.CreateAsync(new AssetUnassignedFromUserLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.AssetUnassignedFromUser,
            Description = $"Asset '{n.AssetTag}' was unassigned from {n.UserName}",
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            UserId = n.UserId,
            UserName = n.UserName,
            Reason = n.Reason,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);

        await repository.CreateAsync(new UserAssetUnassignedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.UserAssetUnassigned,
            Description = $"Asset '{n.AssetTag}' was unassigned from this user",
            TargetUserId = n.UserId,
            UserName = n.UserName,
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            Reason = n.Reason,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}

public class AssetAssignedToLocationNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<AssetAssignedToLocationNotificationHandler> logger
) : INotificationHandler<AssetAssignedToLocationNotification>
{
    public async Task Handle(AssetAssignedToLocationNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging AssetAssignedToLocation: {AssetTag} → {LocationName}", n.AssetTag, n.LocationName);

        await repository.CreateAsync(new AssetAssignedToLocationLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.AssetAssignedToLocation,
            Description = $"Asset '{n.AssetTag}' was assigned to location '{n.LocationName}'",
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            LocationId = n.LocationId,
            LocationName = n.LocationName,
            PreviousLocationId = n.PreviousLocationId,
            PreviousLocationName = n.PreviousLocationName,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);

        await repository.CreateAsync(new LocationAssetAssignedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.LocationAssetAssigned,
            Description = $"Asset '{n.AssetTag}' was assigned to this location",
            LocationId = n.LocationId,
            LocationName = n.LocationName,
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}

public class AssetUnassignedFromLocationNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<AssetUnassignedFromLocationNotificationHandler> logger
) : INotificationHandler<AssetUnassignedFromLocationNotification>
{
    public async Task Handle(AssetUnassignedFromLocationNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging AssetUnassignedFromLocation: {AssetTag} ← {LocationName}", n.AssetTag, n.LocationName);

        await repository.CreateAsync(new AssetUnassignedFromLocationLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.AssetUnassignedFromLocation,
            Description = $"Asset '{n.AssetTag}' was unassigned from location '{n.LocationName}'",
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            LocationId = n.LocationId,
            LocationName = n.LocationName,
            Reason = n.Reason,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);

        await repository.CreateAsync(new LocationAssetUnassignedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.LocationAssetUnassigned,
            Description = $"Asset '{n.AssetTag}' was unassigned from this location",
            LocationId = n.LocationId,
            LocationName = n.LocationName,
            AssetId = n.AssetId,
            AssetTag = n.AssetTag,
            Reason = n.Reason,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}
