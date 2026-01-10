using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetAllAssetCommandHandler(IAssetRepository assetRepository) 
    : IRequestHandler<GetAllAssetCommand, GetAllAssetResponse>
{
    public async Task<GetAllAssetResponse> Handle(GetAllAssetCommand request, CancellationToken cancellationToken)
    {
        var filter = request.Filter ?? new AssetFilterDto();
        
        var (assets, totalCount) = await assetRepository.GetPagedAsync(
            assetTypeIds: filter.AssetTypeIds,
            includeArchived: filter.IncludeArchived,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            definitionIds: filter.DefinitionIds,
            assetTag: filter.AssetTag,
            cancellationToken: cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var assetDtos = assets.Select(a => new AssetDto(
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
        )).ToList();

        return new GetAllAssetResponse(
            assetDtos,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages
        );
    }
}
