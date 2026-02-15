using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;
using Tems.Common.Notifications;

namespace AssetManagement.Application.Commands;

public class DeleteAssetCommandHandler(
    IAssetRepository assetRepository,
    IPublisher publisher) 
    : IRequestHandler<DeleteAssetCommand, DeleteAssetResponse>
{
    public async Task<DeleteAssetResponse> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (asset == null)
        {
            return new DeleteAssetResponse(false);
        }

        var success = await assetRepository.DeleteAsync(request.Id, cancellationToken);

        if (success)
        {
            await publisher.Publish(new AssetDeletedNotification(
                asset.Id, asset.AssetTag, null, null
            ), cancellationToken);

            if (asset.Assignment?.AssignedToUserId != null)
            {
                await publisher.Publish(new AssetUnassignedFromUserNotification(
                    asset.Id, asset.AssetTag,
                    asset.Assignment.AssignedToUserId,
                    asset.Assignment.AssignedToName,
                    "Asset deleted", null, null
                ), cancellationToken);
            }

            if (asset.LocationId != null)
            {
                var locationName = asset.Location != null
                    ? $"{asset.Location.Building} / {asset.Location.Room}".Trim(' ', '/')
                    : string.Empty;

                await publisher.Publish(new AssetUnassignedFromLocationNotification(
                    asset.Id, asset.AssetTag,
                    asset.LocationId, locationName,
                    "Asset deleted", null, null
                ), cancellationToken);
            }
        }

        return new DeleteAssetResponse(success);
    }
}
