using ChangeLog.Application.Domain.UserLogs;
using ChangeLog.Application.Interfaces;
using ChangeLog.Contract.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Notifications;
using Tems.Common.Tenant;

namespace ChangeLog.Application.Handlers;

public class UserCreatedNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<UserCreatedNotificationHandler> logger
) : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging UserCreated for {UserName}", n.UserName);

        await repository.CreateAsync(new UserCreatedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.UserCreated,
            Description = $"User '{n.UserName}' was created",
            TargetUserId = n.UserId,
            UserName = n.UserName,
            Email = n.Email,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}

public class ChangeLogUserDeletedNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<ChangeLogUserDeletedNotificationHandler> logger
) : INotificationHandler<UserDeletedNotification>
{
    public async Task Handle(UserDeletedNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging UserDeleted for {UserName}", n.UserName);

        await repository.CreateAsync(new UserDeletedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.UserDeleted,
            Description = $"User '{n.UserName}' was deleted",
            TargetUserId = n.UserId,
            UserName = n.UserName ?? string.Empty,
            PerformedByUserId = null,
            PerformedByUserName = null
        }, ct);
    }
}

public class UserRolesUpdatedNotificationHandler(
    IChangeLogRepository repository,
    ITenantContext tenantContext,
    ILogger<UserRolesUpdatedNotificationHandler> logger
) : INotificationHandler<UserRolesUpdatedNotification>
{
    public async Task Handle(UserRolesUpdatedNotification n, CancellationToken ct)
    {
        logger.LogInformation("Logging UserRolesUpdated for {UserName}", n.UserName);

        await repository.CreateAsync(new UserRolesUpdatedLog
        {
            TenantId = tenantContext.TenantId ?? "default",
            Action = ChangeLogAction.UserRolesUpdated,
            Description = $"Roles updated for user '{n.UserName}'",
            TargetUserId = n.UserId,
            UserName = n.UserName,
            AddedRoles = n.AddedRoles,
            RemovedRoles = n.RemovedRoles,
            PerformedByUserId = n.PerformedByUserId,
            PerformedByUserName = n.PerformedByUserName
        }, ct);
    }
}
