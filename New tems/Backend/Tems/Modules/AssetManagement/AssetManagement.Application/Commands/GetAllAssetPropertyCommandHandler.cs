using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class GetAllAssetPropertyCommandHandler(IAssetPropertyRepository assetPropertyRepository)
    : IRequestHandler<GetAllAssetPropertyCommand, GetAllAssetPropertyResponse>
{
    public async Task<GetAllAssetPropertyResponse> Handle(GetAllAssetPropertyCommand request, CancellationToken cancellationToken)
    {
        var domainEntities = await assetPropertyRepository.GetAllAsync(cancellationToken);

        var items = domainEntities.Select(entity => new AssetPropertyDto(
            PropertyId: entity.PropertyId,
            Name: entity.Name,
            Description: entity.Description,
            DataType: entity.DataType,
            Category: entity.Category,
            DefaultValidation: entity.DefaultValidation != null ? new PropertyValidationDto(
                entity.DefaultValidation.Type,
                entity.DefaultValidation.MaxLength,
                entity.DefaultValidation.Pattern,
                entity.DefaultValidation.Min,
                entity.DefaultValidation.Max,
                entity.DefaultValidation.Unit,
                entity.DefaultValidation.EnumValues
            ) : null,
            EnumValues: entity.EnumValues,
            Unit: entity.Unit,
            CreatedAt: entity.CreatedAt,
            UpdatedAt: entity.UpdatedAt,
            CreatedBy: entity.CreatedBy
        )).ToList();

        return new GetAllAssetPropertyResponse(items);
    }
}