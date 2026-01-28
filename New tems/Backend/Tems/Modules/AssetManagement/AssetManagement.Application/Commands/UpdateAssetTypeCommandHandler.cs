using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Application.Exceptions;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class UpdateAssetTypeCommandHandler(IAssetTypeRepository assetTypeRepository) 
    : IRequestHandler<UpdateAssetTypeCommand, UpdateAssetTypeResponse>
{
    public async Task<UpdateAssetTypeResponse> Handle(UpdateAssetTypeCommand request, CancellationToken cancellationToken)
    {
        var existingAssetType = await assetTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingAssetType == null)
        {
            return new UpdateAssetTypeResponse(false);
        }

        var normalizedName = request.Name?.Trim() ?? string.Empty;
        var conflicting = await assetTypeRepository.GetByNameInsensitiveAsync(normalizedName, cancellationToken);
        if (conflicting != null && conflicting.Id != request.Id)
        {
            throw new DuplicateAssetTypeNameException(normalizedName);
        }

        existingAssetType.Name = normalizedName;
        existingAssetType.Description = request.Description;
        existingAssetType.ParentTypeId = request.ParentTypeId;
        existingAssetType.Properties = request.Properties.Select(p => new AssetTypeProperty
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
        }).ToList();

        var success = await assetTypeRepository.UpdateAsync(existingAssetType, cancellationToken);

        return new UpdateAssetTypeResponse(success);
    }
}
