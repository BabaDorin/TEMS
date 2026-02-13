using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using LocationManagement.Application.Interfaces;
using Tems.Common.Tenant;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetAllAssetCommandHandler(
    IAssetRepository assetRepository,
    ISiteRepository siteRepository,
    IBuildingRepository buildingRepository,
    IRoomRepository roomRepository,
    ITenantContext tenantContext) 
    : IRequestHandler<GetAllAssetCommand, GetAllAssetResponse>
{
    public async Task<GetAllAssetResponse> Handle(GetAllAssetCommand request, CancellationToken cancellationToken)
    {
        var filter = request.Filter ?? new AssetFilterDto();
        var tenantId = tenantContext.TenantId;
        
        var (assets, totalCount) = await assetRepository.GetPagedAsync(
            assetTypeIds: filter.AssetTypeIds,
            includeArchived: filter.IncludeArchived,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            definitionIds: filter.DefinitionIds,
            assetTag: filter.AssetTag,
            locationId: filter.LocationId,
            assignedToUserId: filter.AssignedToUserId,
            cancellationToken: cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var assetDtos = new List<AssetDto>();
        foreach (var a in assets)
        {
            LocationDetailsDto? locationDetails = null;
            if (!string.IsNullOrEmpty(a.LocationId))
            {
                locationDetails = await FetchLocationDetailsAsync(a.LocationId, tenantId, cancellationToken);
            }

            assetDtos.Add(new AssetDto(
                a.Id,
                a.SerialNumber,
                a.AssetTag,
                a.Status,
                new AssetDefinitionSnapshotDto(
                a.Definition.DefinitionId,
                a.Definition.IsCustomized,
                a.Definition.SnapshotAt,
                a.Definition.Name,
                a.Definition.AssetTypeId,
                a.Definition.AssetTypeName,
                a.Definition.Manufacturer,
                a.Definition.Model,
                a.Definition.Specifications.Select(s => new AssetSpecificationDto(
                    s.PropertyId,
                    s.Name,
                    s.Value,
                    s.DataType,
                    s.Unit
                )).ToList()
            ),
            a.PurchaseInfo != null ? new PurchaseInfoDto(
                a.PurchaseInfo.PurchaseDate,
                a.PurchaseInfo.PurchasePrice,
                a.PurchaseInfo.Currency,
                a.PurchaseInfo.Vendor,
                a.PurchaseInfo.WarrantyExpiry
            ) : null,
            a.LocationId,
            locationDetails,
            a.Location != null ? new AssetLocationDto(
                a.Location.Building,
                a.Location.Room,
                a.Location.Desk
            ) : null,
            a.Assignment != null ? new AssetAssignmentDto(
                a.Assignment.AssignedToUserId,
                a.Assignment.AssignedToName,
                a.Assignment.AssignedAt,
                a.Assignment.AssignmentType
            ) : null,
            a.ParentAssetId,
            a.ChildAssetIds,
            a.Notes,
            a.MaintenanceHistory.Select(m => new MaintenanceRecordDto(
                m.Date,
                m.Type,
                m.Description,
                m.PerformedBy,
                m.Cost
            )).ToList(),
            a.CreatedAt,
            a.UpdatedAt,
            a.CreatedBy,
            a.IsArchived,
            a.ArchivedAt,
            a.ArchivedBy
            ));
        }

        return new GetAllAssetResponse(
            assetDtos,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages
        );
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
