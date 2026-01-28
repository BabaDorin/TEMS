using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetByIdAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository) 
    : IRequestHandler<GetByIdAssetTypeCommand, GetByIdAssetTypeResponse>
{
    public async Task<GetByIdAssetTypeResponse> Handle(GetByIdAssetTypeCommand request, CancellationToken cancellationToken)
    {
        var assetType = await assetTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (assetType == null)
        {
            return new GetByIdAssetTypeResponse(null);
        }

        var assetTypeDto = new AssetTypeDto(
            assetType.Id,
            assetType.Name,
            assetType.Description,
            assetType.ParentTypeId,
            assetType.Properties.Select(p => new AssetTypePropertyDto(
                p.PropertyId,
                p.Name,
                p.Description,
                p.DataType,
                p.Required,
                p.Validation != null ? new PropertyValidationDto(
                    p.Validation.Type,
                    p.Validation.MaxLength,
                    p.Validation.Pattern,
                    p.Validation.Min,
                    p.Validation.Max,
                    p.Validation.Unit,
                    p.Validation.EnumValues
                ) : null,
                p.DisplayOrder
            )).ToList(),
            assetType.CreatedAt,
            assetType.UpdatedAt,
            assetType.CreatedBy,
            assetType.IsArchived,
            assetType.ArchivedAt,
            assetType.ArchivedBy
        );

        return new GetByIdAssetTypeResponse(assetTypeDto);
    }
}
