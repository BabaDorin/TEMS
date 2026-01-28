using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using LocationManagement.Application.Interfaces;
using Tems.Common.Tenant;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetByIdAssetCommandHandler(
    IAssetRepository assetRepository,
    ISiteRepository siteRepository,
    IBuildingRepository buildingRepository,
    IRoomRepository roomRepository,
    ITenantContext tenantContext) 
    : IRequestHandler<GetByIdAssetCommand, GetByIdAssetResponse>
{
    public async Task<GetByIdAssetResponse> Handle(GetByIdAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await assetRepository.GetByIdAsync(request.Id, cancellationToken);

        if (asset == null)
        {
            return new GetByIdAssetResponse(null);
        }

        var tenantId = tenantContext.TenantId;
        LocationDetailsDto? locationDetails = null;

        if (!string.IsNullOrEmpty(asset.LocationId))
        {
            locationDetails = await FetchLocationDetailsAsync(asset.LocationId, tenantId, cancellationToken);
        }

        var assetDto = new AssetDto(
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
            locationDetails,
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
            asset.ArchivedBy
        );

        return new GetByIdAssetResponse(assetDto);
    }

    private async Task<LocationDetailsDto?> FetchLocationDetailsAsync(string locationId, string tenantId, CancellationToken cancellationToken)
    {
        var room = await roomRepository.GetByIdAsync(locationId, tenantId, cancellationToken);
        if (room != null)
        {
            var building = await buildingRepository.GetByIdAsync(room.BuildingId, tenantId, cancellationToken);
            var site = building != null ? await siteRepository.GetByIdAsync(building.SiteId, tenantId, cancellationToken) : null;

            return new LocationDetailsDto(
                room.Id,
                room.Name,
                "Room",
                room.BuildingId,
                $"{site?.Name ?? "Unknown"} > {building?.Name ?? "Unknown"} > {room.Name}",
                site != null ? new LocationSiteDto(site.Id, site.Name, site.Code, site.Timezone) : null,
                building != null ? new LocationBuildingDto(building.Id, building.Name, building.AddressLine) : null
            );
        }

        var buildingDirect = await buildingRepository.GetByIdAsync(locationId, tenantId, cancellationToken);
        if (buildingDirect != null)
        {
            var site = await siteRepository.GetByIdAsync(buildingDirect.SiteId, tenantId, cancellationToken);

            return new LocationDetailsDto(
                buildingDirect.Id,
                buildingDirect.Name,
                "Building",
                buildingDirect.SiteId,
                $"{site?.Name ?? "Unknown"} > {buildingDirect.Name}",
                site != null ? new LocationSiteDto(site.Id, site.Name, site.Code, site.Timezone) : null,
                new LocationBuildingDto(buildingDirect.Id, buildingDirect.Name, buildingDirect.AddressLine)
            );
        }

        var siteDirect = await siteRepository.GetByIdAsync(locationId, tenantId, cancellationToken);
        if (siteDirect != null)
        {
            return new LocationDetailsDto(
                siteDirect.Id,
                siteDirect.Name,
                "Site",
                null,
                siteDirect.Name,
                new LocationSiteDto(siteDirect.Id, siteDirect.Name, siteDirect.Code, siteDirect.Timezone),
                null
            );
        }

        return null;
    }
}
