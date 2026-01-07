using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetByIdAssetCommandHandler(IAssetRepository assetRepository) 
    : IRequestHandler<GetByIdAssetCommand, GetByIdAssetResponse>
{
    public async Task<GetByIdAssetResponse> Handle(GetByIdAssetCommand request, CancellationToken cancellationToken)
    {
        var asset = await assetRepository.GetByIdAsync(request.Id, cancellationToken);

        if (asset == null)
        {
            return new GetByIdAssetResponse(null);
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
}
