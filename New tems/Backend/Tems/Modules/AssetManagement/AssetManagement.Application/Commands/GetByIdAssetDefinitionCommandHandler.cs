using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetByIdAssetDefinitionCommandHandler(IAssetDefinitionRepository assetDefinitionRepository) 
    : IRequestHandler<GetByIdAssetDefinitionCommand, GetByIdAssetDefinitionResponse>
{
    public async Task<GetByIdAssetDefinitionResponse> Handle(GetByIdAssetDefinitionCommand request, CancellationToken cancellationToken)
    {
        var assetDefinition = await assetDefinitionRepository.GetByIdAsync(request.Id, cancellationToken);

        if (assetDefinition == null)
        {
            return new GetByIdAssetDefinitionResponse(null);
        }

        var assetDefinitionDto = new AssetDefinitionDto(
            assetDefinition.Id,
            assetDefinition.Name,
            assetDefinition.ShortName,
            assetDefinition.AssetTypeId,
            assetDefinition.AssetTypeName,
            assetDefinition.Manufacturer,
            assetDefinition.Model,
            assetDefinition.Specifications.Select(s => new AssetSpecificationDto(
                s.PropertyId,
                s.Name,
                s.Value,
                s.DataType,
                s.Unit
            )).ToList(),
            assetDefinition.UsageCount,
            assetDefinition.Description,
            assetDefinition.Notes,
            assetDefinition.Tags,
            assetDefinition.CreatedAt,
            assetDefinition.UpdatedAt,
            assetDefinition.CreatedBy,
            assetDefinition.IsArchived,
            assetDefinition.ArchivedAt,
            assetDefinition.ArchivedBy
        );

        return new GetByIdAssetDefinitionResponse(assetDefinitionDto);
    }
}
