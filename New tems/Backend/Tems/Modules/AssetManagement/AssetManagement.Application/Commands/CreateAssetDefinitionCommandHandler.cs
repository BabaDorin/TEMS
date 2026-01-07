using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class CreateAssetDefinitionCommandHandler(IAssetDefinitionRepository assetDefinitionRepository) 
    : IRequestHandler<CreateAssetDefinitionCommand, CreateAssetDefinitionResponse>
{
    public async Task<CreateAssetDefinitionResponse> Handle(CreateAssetDefinitionCommand request, CancellationToken cancellationToken)
    {
        var domainEntity = new AssetDefinition
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            ShortName = request.ShortName,
            AssetTypeId = request.AssetTypeId,
            AssetTypeName = request.AssetTypeName,
            Manufacturer = request.Manufacturer,
            Model = request.Model,
            Specifications = request.Specifications.Select(s => new AssetSpecification
            {
                PropertyId = s.PropertyId,
                Name = s.Name,
                Value = s.Value,
                DataType = s.DataType,
                Unit = s.Unit
            }).ToList(),
            Description = request.Description,
            Notes = request.Notes,
            Tags = request.Tags,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsArchived = false
        };

        await assetDefinitionRepository.CreateAsync(domainEntity, cancellationToken);

        return new CreateAssetDefinitionResponse(domainEntity.Id);
    }
}
