using AssetManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Notifications;

namespace AssetManagement.Application.Handlers;

public class LocationDeletedNotificationHandler(
    IAssetRepository assetRepository,
    IPublisher publisher,
    ILogger<LocationDeletedNotificationHandler> logger
) : INotificationHandler<LocationDeletedNotification>
{
    public async Task Handle(LocationDeletedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling LocationDeletedNotification for {LocationType} {LocationId}",
            notification.LocationType, notification.LocationId);

        var assets = await assetRepository.GetByLocationIdAsync(notification.LocationId, cancellationToken);

        if (assets.Count == 0)
        {
            logger.LogInformation("No assets at deleted location {LocationId}", notification.LocationId);
            return;
        }

        logger.LogInformation(
            "Clearing location from {Count} asset(s) at deleted {LocationType} {LocationId}",
            assets.Count, notification.LocationType, notification.LocationId);

        foreach (var asset in assets)
        {
            var locationName = asset.Location != null
                ? $"{asset.Location.Building} / {asset.Location.Room}".Trim(' ', '/')
                : string.Empty;

            asset.LocationId = null;
            asset.Location = null;
            asset.UpdatedAt = DateTime.UtcNow;
            await assetRepository.UpdateAsync(asset, cancellationToken);

            await publisher.Publish(new AssetUnassignedFromLocationNotification(
                asset.Id, asset.AssetTag,
                notification.LocationId, locationName,
                $"{notification.LocationType} deleted", null, null
            ), cancellationToken);
        }

        logger.LogInformation(
            "Successfully cleared location from {Count} asset(s)",
            assets.Count);
    }
}
