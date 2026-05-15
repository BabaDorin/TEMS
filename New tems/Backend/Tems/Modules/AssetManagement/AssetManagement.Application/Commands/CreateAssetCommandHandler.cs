using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;
using Tems.Common.Notifications;
using Tems.Common.Tenant;

namespace AssetManagement.Application.Commands;

public class CreateAssetCommandHandler(
    IAssetRepository assetRepository,
    IAssetDefinitionRepository assetDefinitionRepository,
    IPurchaseOrderRepository purchaseOrderRepository,
    ITenantContext tenantContext,
    IPublisher publisher) 
    : IRequestHandler<CreateAssetCommand, CreateAssetResponse>
{
    public async Task<CreateAssetResponse> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate Serial Number or Asset Tag
        var existingAsset = await assetRepository.GetBySerialNumberOrTagAsync(request.SerialNumber, request.AssetTag, cancellationToken);
        if (existingAsset != null)
        {
            throw new InvalidOperationException("An asset with this Serial Number or TEMS ID already exists.");
        }

        // Verify definition exists (but use data from command)
        var definition = await assetDefinitionRepository.GetByIdAsync(request.DefinitionId, cancellationToken);
        if (definition == null)
        {
            throw new InvalidOperationException($"AssetDefinition with ID {request.DefinitionId} not found");
        }

        var specifications = request.CustomizeDefinition && request.CustomSpecifications != null
            ? request.CustomSpecifications.Select(s => new AssetSpecification
            {
                PropertyId = s.PropertyId,
                Name = s.Name,
                Value = AssetSpecificationValueConverter.Convert(s.Value),
                DataType = s.DataType,
                Unit = s.Unit
            }).ToList()
            : definition.Specifications;

        var resolvedPurchaseInfo = await ResolvePurchaseInfoAsync(request.PurchaseInfo, purchaseOrderRepository, tenantContext.TenantId, cancellationToken);

        var domainEntity = new Asset
        {
            Id = Guid.NewGuid().ToString(),
            SerialNumber = request.SerialNumber,
            AssetTag = request.AssetTag,
            Status = request.Status,
            Definition = new AssetDefinitionSnapshot
            {
                DefinitionId = request.DefinitionId,
                IsCustomized = request.CustomizeDefinition,
                SnapshotAt = DateTime.UtcNow,
                Name = request.DefinitionName,
                AssetTypeId = request.AssetTypeId,
                AssetTypeName = request.AssetTypeName,
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Specifications = specifications
            },
            PurchaseInfo = resolvedPurchaseInfo,
            LocationId = request.LocationId,
            Location = request.Location != null ? new AssetLocation
            {
                Building = request.Location.Building,
                Room = request.Location.Room,
                Desk = request.Location.Desk
            } : null,
            Assignment = request.Assignment != null ? new AssetAssignment
            {
                AssignedToUserId = request.Assignment.AssignedToUserId,
                AssignedToName = request.Assignment.AssignedToName,
                AssignedAt = request.Assignment.AssignedAt,
                AssignmentType = request.Assignment.AssignmentType
            } : null,
            ParentAssetId = request.ParentAssetId,
            ChildAssetIds = request.ChildAssetIds,
            Notes = request.Notes,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsArchived = false
        };

        await assetRepository.CreateAsync(domainEntity, cancellationToken);

        await assetDefinitionRepository.IncrementUsageCountAsync(definition.Id, cancellationToken);

        await publisher.Publish(new AssetCreatedNotification(
            domainEntity.Id,
            domainEntity.AssetTag,
            request.DefinitionName,
            request.AssetTypeName,
            request.Status,
            request.CreatedBy,
            null
        ), cancellationToken);

        if (request.Assignment?.AssignedToUserId != null)
        {
            await publisher.Publish(new AssetAssignedToUserNotification(
                domainEntity.Id,
                domainEntity.AssetTag,
                request.Assignment.AssignedToUserId,
                request.Assignment.AssignedToName,
                null,
                null,
                request.CreatedBy,
                null
            ), cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(request.LocationId))
        {
            var locationName = request.Location != null
                ? $"{request.Location.Building} / {request.Location.Room}".Trim(' ', '/')
                : string.Empty;

            await publisher.Publish(new AssetAssignedToLocationNotification(
                domainEntity.Id,
                domainEntity.AssetTag,
                request.LocationId,
                locationName,
                null,
                null,
                request.CreatedBy,
                null
            ), cancellationToken);
        }

        return new CreateAssetResponse(domainEntity.Id);
    }
    private static async Task<PurchaseInfo?> ResolvePurchaseInfoAsync(
        PurchaseInfoDto? requestPurchaseInfo,
        IPurchaseOrderRepository purchaseOrderRepository,
        string tenantId,
        CancellationToken cancellationToken)
    {
        if (requestPurchaseInfo == null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(requestPurchaseInfo.PurchaseOrderId))
        {
            var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(requestPurchaseInfo.PurchaseOrderId, tenantId, cancellationToken);
            if (purchaseOrder == null)
            {
                throw new InvalidOperationException("The selected purchase order no longer exists.");
            }

            if (requestPurchaseInfo.PurchasePrice is null or <= 0)
            {
                throw new InvalidOperationException("A linked purchase order requires a valid asset item price.");
            }

            return new PurchaseInfo
            {
                PurchaseDate = requestPurchaseInfo.PurchaseDate,
                PurchasePrice = requestPurchaseInfo.PurchasePrice,
                Currency = purchaseOrder.Currency,
                Vendor = purchaseOrder.Vendor,
                PurchaseOrderId = purchaseOrder.Id,
                PurchaseOrderNumber = purchaseOrder.PoNumber,
                WarrantyExpiry = requestPurchaseInfo.WarrantyExpiry
            };
        }

        return new PurchaseInfo
        {
            PurchaseDate = requestPurchaseInfo.PurchaseDate,
            PurchasePrice = requestPurchaseInfo.PurchasePrice,
            Currency = requestPurchaseInfo.Currency,
            Vendor = requestPurchaseInfo.Vendor,
            PurchaseOrderId = requestPurchaseInfo.PurchaseOrderId,
            PurchaseOrderNumber = requestPurchaseInfo.PurchaseOrderNumber,
            WarrantyExpiry = requestPurchaseInfo.WarrantyExpiry
        };
    }
}
