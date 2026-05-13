using AssetManagement.Application.Interfaces;
using AssetManagement.Application.Mappers;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;
using Tems.Common.Notifications;

namespace AssetManagement.Application.Commands;

public class UnassignAssetFromUserCommandHandler(IAssetRepository assetRepository, IPublisher publisher) 
    : IRequestHandler<UnassignAssetFromUserCommand, AssetDto>
{
    public async Task<AssetDto> Handle(UnassignAssetFromUserCommand request, CancellationToken cancellationToken)
    {
        var asset = await assetRepository.GetByIdAsync(request.AssetId, cancellationToken) 
            ?? throw new KeyNotFoundException($"Asset with ID {request.AssetId} not found");

        var previousUserId = asset.Assignment?.AssignedToUserId;
        var previousUserName = asset.Assignment?.AssignedToName;

        asset.Assignment = null;
        asset.UpdatedAt = DateTime.UtcNow;

        await assetRepository.UpdateAsync(asset, cancellationToken);

        if (!string.IsNullOrEmpty(previousUserId))
        {
            await publisher.Publish(new AssetUnassignedFromUserNotification(
                asset.Id, asset.AssetTag, previousUserId, previousUserName ?? "Unknown",
                null, null, null), cancellationToken);
        }

        return asset.ToDto();
    }
}
