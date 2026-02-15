using ChangeLog.Application.Domain.LocationLogs;
using ChangeLog.Application.Interfaces;
using ChangeLog.Contract.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Notifications;
using Tems.Common.Tenant;

namespace ChangeLog.Application.Handlers;

public class LocationCreatedNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<LocationCreatedNotificationHandler> logger
) : INotificationHandler<LocationCreatedNotification>
{
    public async Task Handle(LocationCreatedNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging LocationCreated: {LocationName} ({LocationType})", n.LocationName, n.LocationType);

        await repository.CreateAsync(new LocationCreatedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.LocationCreated,
            Description = $"{n.LocationType} '{n.LocationName}' was created",
            LocationId = n.LocationId,
            LocationName = n.LocationName,
            LocationType = n.LocationType,
            ParentId = n.ParentId,
            ParentName = n.ParentName,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}

public class ChangeLogLocationDeletedNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<ChangeLogLocationDeletedNotificationHandler> logger
) : INotificationHandler<LocationDeletedNotification>
{
    public async Task Handle(LocationDeletedNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging LocationDeleted: {LocationId} ({LocationType})", n.LocationId, n.LocationType);

        await repository.CreateAsync(new LocationDeletedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.LocationDeleted,
            Description = $"{n.LocationType} was deleted",
            LocationId = n.LocationId,
            LocationName = string.Empty,
            LocationType = n.LocationType
        }, ct);
    }
}
