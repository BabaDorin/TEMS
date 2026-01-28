using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetAllAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository) 
    : IRequestHandler<GetAllAssetTypeCommand, GetAllAssetTypeResponse>
{
    public async Task<GetAllAssetTypeResponse> Handle(GetAllAssetTypeCommand request, CancellationToken cancellationToken)
    {
        var assetTypes = await assetTypeRepository.GetAllAsync(request.IncludeArchived, cancellationToken);

        var assetTypeDtos = assetTypes.Select(at => new AssetTypeDto(
            at.Id,
            at.Name,
            at.Description,
            at.ParentTypeId,
            at.Properties.Select(p => new AssetTypePropertyDto(
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
            at.CreatedAt,
            at.UpdatedAt,
            at.CreatedBy,
            at.IsArchived,
            at.ArchivedAt,
            at.ArchivedBy
        )).ToList();

        return new GetAllAssetTypeResponse(assetTypeDtos);
    }
}
