using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;
using Tems.Common.Notifications;

namespace AssetManagement.Application.Commands;

public class UpdateAssetCommandHandler(
    IAssetRepository assetRepository,
    IPublisher publisher) 
    : IRequestHandler<UpdateAssetCommand, UpdateAssetResponse>
{
    public async Task<UpdateAssetResponse> Handle(UpdateAssetCommand request, CancellationToken cancellationToken)
    {
        var existingAsset = await assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingAsset == null)
        {
            return new UpdateAssetResponse(false);
        }

        var previousUserId = existingAsset.Assignment?.AssignedToUserId;
        var previousUserName = existingAsset.Assignment?.AssignedToName;
        var previousLocationId = existingAsset.LocationId;
        var previousLocationName = existingAsset.Location != null 
            ? $"{existingAsset.Location.Building} / {existingAsset.Location.Room}".Trim(' ', '/') 
            : null;

        var fieldChanges = new List<AssetFieldChange>();
        if (existingAsset.SerialNumber != request.SerialNumber)
            fieldChanges.Add(new AssetFieldChange("SerialNumber", existingAsset.SerialNumber, request.SerialNumber));
        if (existingAsset.AssetTag != request.AssetTag)
            fieldChanges.Add(new AssetFieldChange("AssetTag", existingAsset.AssetTag, request.AssetTag));
        if (existingAsset.Status != request.Status)
            fieldChanges.Add(new AssetFieldChange("Status", existingAsset.Status, request.Status));
        if (existingAsset.Notes != request.Notes)
            fieldChanges.Add(new AssetFieldChange("Notes", existingAsset.Notes, request.Notes));

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
        
        if (request.LocationId != null)
        {
            existingAsset.LocationId = request.LocationId;
        }
        
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

        if (success)
        {
            var newUserId = request.Assignment?.AssignedToUserId;
            var newUserName = request.Assignment?.AssignedToName;

            if (previousUserId != newUserId)
            {
                if (previousUserId != null)
                {
                    await publisher.Publish(new AssetUnassignedFromUserNotification(
                        request.Id, existingAsset.AssetTag,
                        previousUserId, previousUserName ?? string.Empty,
                        null, null, null
                    ), cancellationToken);
                }

                if (newUserId != null)
                {
                    await publisher.Publish(new AssetAssignedToUserNotification(
                        request.Id, existingAsset.AssetTag,
                        newUserId, newUserName ?? string.Empty,
                        previousUserId, previousUserName, null, null
                    ), cancellationToken);
                }
            }

            var newLocationId = request.LocationId;
            var newLocationName = request.Location != null 
                ? $"{request.Location.Building} / {request.Location.Room}".Trim(' ', '/') 
                : null;

            if (previousLocationId != newLocationId && newLocationId != null)
            {
                if (previousLocationId != null)
                {
                    await publisher.Publish(new AssetUnassignedFromLocationNotification(
                        request.Id, existingAsset.AssetTag,
                        previousLocationId, previousLocationName ?? string.Empty,
                        null, null, null
                    ), cancellationToken);
                }

                await publisher.Publish(new AssetAssignedToLocationNotification(
                    request.Id, existingAsset.AssetTag,
                    newLocationId, newLocationName ?? string.Empty,
                    previousLocationId, previousLocationName, null, null
                ), cancellationToken);
            }

            if (fieldChanges.Count > 0)
            {
                await publisher.Publish(new AssetUpdatedNotification(
                    request.Id, existingAsset.AssetTag, fieldChanges, null, null
                ), cancellationToken);
            }
        }

        return new UpdateAssetResponse(success);
    }
}
