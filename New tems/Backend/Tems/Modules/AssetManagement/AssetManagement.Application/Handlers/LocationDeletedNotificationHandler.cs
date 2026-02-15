using AssetManagement.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Notifications;

namespace AssetManagement.Application.Handlers;

/// <summary>
/// Clears asset location references when a location is deleted from the system.
/// Sets LocationId and Location to null so the asset appears unlocated.
/// </summary>
public class LocationDeletedNotificationHandler(
    IAssetRepository assetRepository,
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
            asset.LocationId = null;
            asset.Location = null;
            asset.UpdatedAt = DateTime.UtcNow;
            await assetRepository.UpdateAsync(asset, cancellationToken);
        }

        logger.LogInformation(
            "Successfully cleared location from {Count} asset(s)",
            assets.Count);
    }
}
