using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class UpdateAssetCommandHandler(IAssetRepository assetRepository) 
    : IRequestHandler<UpdateAssetCommand, UpdateAssetResponse>
{
    public async Task<UpdateAssetResponse> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var existingAsset = await assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingAsset == null)
        {
            return new UpdateAssetResponse(false);
        }

        existingAsset.SerialNumber = request.SerialNumber;
        existingAsset.AssetTag = request.AssetTag;
        existingAsset.Status = request.Status;
        existingAsset.Definition.IsCustomized = request.IsCustomized;
        existingAsset.Definition.Specifications = request.Specifications.Select(s => new AssetSpecification
        {
            PropertyId = s.PropertyId,
            Name = s.Name,
            Value = s.Value,
            DataType = s.DataType,
            Unit = s.Unit
        }).ToList();
        existingAsset.PurchaseInfo = request.PurchaseInfo != null ? new PurchaseInfo
        {
            PurchaseDate = request.PurchaseInfo.PurchaseDate,
            PurchasePrice = request.PurchaseInfo.PurchasePrice,
            Currency = request.PurchaseInfo.Currency,
            Vendor = request.PurchaseInfo.Vendor,
            WarrantyExpiry = request.PurchaseInfo.WarrantyExpiry
        } : null;
        existingAsset.Location = request.Location != null ? new AssetLocation
        {
            Building = request.Location.Building,
            Room = request.Location.Room,
            Desk = request.Location.Desk
        } : null;
        existingAsset.Assignment = request.Assignment != null ? new AssetAssignment
        {
            AssignedToUserId = request.Assignment.AssignedToUserId,
            AssignedToName = request.Assignment.AssignedToName,
            AssignedAt = request.Assignment.AssignedAt,
            AssignmentType = request.Assignment.AssignmentType
        } : null;
        existingAsset.ParentAssetId = request.ParentAssetId;
        existingAsset.ChildAssetIds = request.ChildAssetIds;
        existingAsset.Notes = request.Notes;
        existingAsset.MaintenanceHistory = request.MaintenanceHistory.Select(m => new MaintenanceRecord
        {
            Date = m.Date,
            Type = m.Type,
            Description = m.Description,
            PerformedBy = m.PerformedBy,
            Cost = m.Cost
        }).ToList();

        var success = await assetRepository.UpdateAsync(existingAsset, cancellationToken);

        return new UpdateAssetResponse(success);
    }
}
