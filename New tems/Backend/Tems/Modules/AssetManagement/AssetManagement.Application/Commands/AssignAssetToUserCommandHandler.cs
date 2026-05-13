using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Application.Mappers;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;
using Tems.Common.Notifications;

namespace AssetManagement.Application.Commands;

public class AssignAssetToUserCommandHandler(IAssetRepository assetRepository, IPublisher publisher) 
    : IRequestHandler<AssignAssetToUserCommand, AssetDto>
{
    public async Task<AssetDto> Handle(AssignAssetToUserCommand request, CancellationToken cancellationToken)
    {
        var asset = await assetRepository.GetByIdAsync(request.AssetId, cancellationToken) 
            ?? throw new KeyNotFoundException($"Asset with ID {request.AssetId} not found");

        var previousUserId = asset.Assignment?.AssignedToUserId;
        var previousUserName = asset.Assignment?.AssignedToName;

        asset.Assignment = new AssetAssignment
        {
            AssignedToUserId = request.UserId,
            AssignedToName = request.UserName,
            AssignedAt = DateTime.UtcNow,
            AssignmentType = "permanent"
        };
        asset.UpdatedAt = DateTime.UtcNow;

        await assetRepository.UpdateAsync(asset, cancellationToken);

        await publisher.Publish(new AssetAssignedToUserNotification(
            asset.Id, asset.AssetTag, request.UserId, request.UserName,
            previousUserId, previousUserName, null, null), cancellationToken);

        return asset.ToDto();
    }
}
