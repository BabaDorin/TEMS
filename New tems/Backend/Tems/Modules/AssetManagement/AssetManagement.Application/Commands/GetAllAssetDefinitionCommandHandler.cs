using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetAllAssetDefinitionCommandHandler(IAssetDefinitionRepository assetDefinitionRepository) 
    : IRequestHandler<GetAllAssetDefinitionCommand, GetAllAssetDefinitionResponse>
{
    public async Task<GetAllAssetDefinitionResponse> Handle(GetAllAssetDefinitionCommand request, CancellationToken cancellationToken)
    {
        var assetDefinitions = await assetDefinitionRepository.GetAllAsync(request.IncludeArchived, cancellationToken);

        var assetDefinitionDtos = assetDefinitions.Select(ad => new AssetDefinitionDto(
            ad.Id,
            ad.Name,
            ad.ShortName,
            ad.AssetTypeId,
            ad.AssetTypeName,
            ad.Manufacturer,
            ad.Model,
            ad.Specifications.Select(s => new AssetSpecificationDto(
                s.PropertyId,
                s.Name,
                s.Value,
                s.DataType,
                s.Unit
            )).ToList(),
            ad.UsageCount,
            ad.Description,
            ad.Notes,
            ad.Tags,
            ad.CreatedAt,
            ad.UpdatedAt,
            ad.CreatedBy,
            ad.IsArchived,
            ad.ArchivedAt,
            ad.ArchivedBy
        )).ToList();

        return new GetAllAssetDefinitionResponse(assetDefinitionDtos);
    }
}
