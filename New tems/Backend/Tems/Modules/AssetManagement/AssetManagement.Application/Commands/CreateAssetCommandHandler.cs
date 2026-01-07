using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class CreateAssetCommandHandler(
    IAssetRepository assetRepository,
    IAssetDefinitionRepository assetDefinitionRepository) 
    : IRequestHandler<CreateAssetCommand, CreateAssetResponse>
{
    public async Task<CreateAssetResponse> Handle(CreateAssetCommand request, CancellationToken cancellationToken)
    {
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
                Value = s.Value,
                DataType = s.DataType,
                Unit = s.Unit
            }).ToList()
            : definition.Specifications;

        var domainEntity = new Asset
        {
            Id = Guid.NewGuid().ToString(),
            SerialNumber = request.SerialNumber,
            AssetTag = request.AssetTag,
            Status = request.Status,
            Definition = new AssetDefinitionSnapshot
            {
                DefinitionId = definition.Id,
                IsCustomized = request.CustomizeDefinition,
                SnapshotAt = DateTime.UtcNow,
                Name = definition.Name,
                AssetTypeId = definition.AssetTypeId,
                AssetTypeName = definition.AssetTypeName,
                Manufacturer = definition.Manufacturer,
                Model = definition.Model,
                Specifications = specifications
            },
            PurchaseInfo = request.PurchaseInfo != null ? new PurchaseInfo
            {
                PurchaseDate = request.PurchaseInfo.PurchaseDate,
                PurchasePrice = request.PurchaseInfo.PurchasePrice,
                Currency = request.PurchaseInfo.Currency,
                Vendor = request.PurchaseInfo.Vendor,
                WarrantyExpiry = request.PurchaseInfo.WarrantyExpiry
            } : null,
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
            MaintenanceHistory = [],
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsArchived = false
        };

        await assetRepository.CreateAsync(domainEntity, cancellationToken);

        await assetDefinitionRepository.IncrementUsageCountAsync(definition.Id, cancellationToken);

        return new CreateAssetResponse(domainEntity.Id);
    }
}
