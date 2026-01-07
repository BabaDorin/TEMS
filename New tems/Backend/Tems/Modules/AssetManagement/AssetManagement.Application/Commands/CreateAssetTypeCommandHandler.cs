using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class CreateAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository) 
    : IRequestHandler<CreateAssetTypeCommand, CreateAssetTypeResponse>
{
    public async Task<CreateAssetTypeResponse> Handle(CreateAssetTypeCommand request, CancellationToken cancellationToken)
    {
        var domainEntity = new AssetType
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            ParentTypeId = request.ParentTypeId,
            Properties = request.Properties.Select(p => new AssetTypeProperty
            {
                PropertyId = p.PropertyId,
                Name = p.Name,
                Description = p.Description,
                DataType = p.DataType,
                Required = p.Required,
                Validation = p.Validation != null ? new PropertyValidation
                {
                    Type = p.Validation.Type,
                    MaxLength = p.Validation.MaxLength,
                    Pattern = p.Validation.Pattern,
                    Min = p.Validation.Min,
                    Max = p.Validation.Max,
                    Unit = p.Validation.Unit,
                    EnumValues = p.Validation.EnumValues ?? []
                } : null,
                DisplayOrder = p.DisplayOrder
            }).ToList(),
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsArchived = false
        };

        await assetTypeRepository.CreateAsync(domainEntity, cancellationToken);

        return new CreateAssetTypeResponse(domainEntity.Id);
    }
}
