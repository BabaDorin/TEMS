using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.DTOs;
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

        return new AssetDto(
            asset.Id,
            asset.SerialNumber,
            asset.AssetTag,
            asset.Status,
            new AssetDefinitionSnapshotDto(
                asset.Definition.DefinitionId,
                asset.Definition.IsCustomized,
                asset.Definition.SnapshotAt,
                asset.Definition.Name,
                asset.Definition.AssetTypeId,
                asset.Definition.AssetTypeName,
                asset.Definition.Manufacturer,
                asset.Definition.Model,
                asset.Definition.Specifications.Select(s => new AssetSpecificationDto(
                    s.PropertyId,
                    s.Name,
                    s.Value,
                    s.DataType,
                    s.Unit
                )).ToList()
            ),
            asset.PurchaseInfo != null ? new PurchaseInfoDto(
                asset.PurchaseInfo.PurchaseDate,
                asset.PurchaseInfo.PurchasePrice,
                asset.PurchaseInfo.Currency,
                asset.PurchaseInfo.Vendor,
                asset.PurchaseInfo.WarrantyExpiry
            ) : null,
            asset.LocationId,
            null,
            asset.Location != null ? new AssetLocationDto(
                asset.Location.Building,
                asset.Location.Room,
                asset.Location.Desk
            ) : null,
            asset.Assignment != null ? new AssetAssignmentDto(
                asset.Assignment.AssignedToUserId,
                asset.Assignment.AssignedToName,
                asset.Assignment.AssignedAt,
                asset.Assignment.AssignmentType
            ) : null,
            asset.ParentAssetId,
            asset.ChildAssetIds,
            asset.Notes,
            asset.MaintenanceHistory.Select(m => new MaintenanceRecordDto(
                m.Date,
                m.Type,
                m.Description,
                m.PerformedBy,
                m.Cost
            )).ToList(),
            asset.CreatedAt,
            asset.UpdatedAt,
            asset.CreatedBy,
            asset.IsArchived,
            asset.ArchivedAt,
            asset.ArchivedBy);
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
