using AssetManagement.Application.Interfaces;
using AssetManagement.Application.Mappers;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using LocationManagement.Application.Interfaces;
using MediatR;
using Tems.Common.Notifications;
using Tems.Common.Tenant;

namespace AssetManagement.Application.Commands;

public class AssignAssetToRoomCommandHandler(
    IAssetRepository assetRepository,
    IRoomRepository roomRepository,
    IBuildingRepository buildingRepository,
    ISiteRepository siteRepository,
    ITenantContext tenantContext,
    IPublisher publisher) 
    : IRequestHandler<AssignAssetToRoomCommand, AssetDto>
{
    public async Task<AssetDto> Handle(AssignAssetToRoomCommand request, CancellationToken cancellationToken)
    {
        var asset = await assetRepository.GetByIdAsync(request.AssetId, cancellationToken) 
            ?? throw new KeyNotFoundException($"Asset with ID {request.AssetId} not found");

        var previousLocationId = asset.LocationId;
        var previousLocationName = asset.Location != null 
            ? $"{asset.Location.Building} / {asset.Location.Room}" 
            : null;

        asset.LocationId = request.RoomId;

        await assetRepository.UpdateAsync(asset, cancellationToken);

        var newLocationName = await ResolveLocationNameAsync(request.RoomId, cancellationToken);

        await publisher.Publish(new AssetAssignedToLocationNotification(
            asset.Id, asset.AssetTag, request.RoomId, newLocationName,
            previousLocationId, previousLocationName, null, null), cancellationToken);

        return asset.ToDto();
    }

    private async Task<string> ResolveLocationNameAsync(string roomId, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? "default";
        var room = await roomRepository.GetByIdAsync(roomId, tenantId, cancellationToken);
        if (room == null)
        {
            return roomId;
        }

        var building = await buildingRepository.GetByIdAsync(room.BuildingId, tenantId, cancellationToken);
        var site = building != null ? await siteRepository.GetByIdAsync(building.SiteId, tenantId, cancellationToken) : null;

        return string.Join(" > ", new[] { site?.Name, building?.Name, room.Name }.Where(part => !string.IsNullOrWhiteSpace(part)));
    }
}
