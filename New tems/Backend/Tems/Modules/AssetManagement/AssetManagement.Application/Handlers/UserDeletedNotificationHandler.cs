using AssetManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Notifications;

namespace AssetManagement.Application.Handlers;

public class UserDeletedNotificationHandler(
    IAssetRepository assetRepository,
    IPublisher publisher,
    ILogger<UserDeletedNotificationHandler> logger
) : INotificationHandler<UserDeletedNotification>
{
    public async Task Handle(UserDeletedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling UserDeletedNotification for user {UserId} ({UserName})",
            notification.UserId, notification.UserName);

        var assets = await assetRepository.GetByAssignedUserIdAsync(notification.UserId, cancellationToken);

        if (assets.Count == 0)
        {
            logger.LogInformation("No assets assigned to deleted user {UserId}", notification.UserId);
            return;
        }

        logger.LogInformation(
            "Unassigning {Count} asset(s) from deleted user {UserId}",
            assets.Count, notification.UserId);

        foreach (var asset in assets)
        {
            asset.Assignment = null;
            asset.UpdatedAt = DateTime.UtcNow;
            await assetRepository.UpdateAsync(asset, cancellationToken);

            await publisher.Publish(new AssetUnassignedFromUserNotification(
                asset.Id, asset.AssetTag,
                notification.UserId, notification.UserName ?? string.Empty,
                "User deleted", null, null
            ), cancellationToken);
        }

        logger.LogInformation(
            "Successfully unassigned {Count} asset(s) from deleted user {UserId}",
            assets.Count, notification.UserId);
    }
}
