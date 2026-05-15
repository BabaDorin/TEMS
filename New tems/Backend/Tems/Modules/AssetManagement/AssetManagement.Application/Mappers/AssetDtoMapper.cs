using AssetManagement.Application.Domain;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;

namespace AssetManagement.Application.Mappers;

internal static class AssetDtoMapper
{
    public static AssetDto ToDto(this Asset asset, LocationDetailsDto? locationDetails = null)
    {
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
                asset.PurchaseInfo.PurchaseOrderId,
                asset.PurchaseInfo.PurchaseOrderNumber,
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
            asset.CreatedAt,
            asset.UpdatedAt,
            asset.CreatedBy,
            asset.IsArchived,
            asset.ArchivedAt,
            asset.ArchivedBy
        );
    }
}
