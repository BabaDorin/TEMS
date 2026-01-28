using AssetManagement.Contract.DTOs;
using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Application.Commands;

public class UpdateAssetDefinitionCommandHandler(IAssetDefinitionRepository assetDefinitionRepository) 
    : IRequestHandler<UpdateAssetDefinitionCommand, UpdateAssetDefinitionResponse>
{
    public async Task<UpdateAssetDefinitionResponse> Handle(UpdateAssetDefinitionCommand request, CancellationToken cancellationToken)
    {
        var existingAssetDefinition = await assetDefinitionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingAssetDefinition == null)
        {
            return new UpdateAssetDefinitionResponse(false);
        }

        existingAssetDefinition.Name = request.Name;
        existingAssetDefinition.ShortName = request.ShortName;
        existingAssetDefinition.AssetTypeId = request.AssetTypeId;
        existingAssetDefinition.AssetTypeName = request.AssetTypeName;
        existingAssetDefinition.Manufacturer = request.Manufacturer;
        existingAssetDefinition.Model = request.Model;
        existingAssetDefinition.Specifications = request.Specifications.Select(s => new AssetSpecification
        {
            PropertyId = s.PropertyId,
            Name = s.Name,
            Value = s.Value,
            DataType = s.DataType,
            Unit = s.Unit
        }).ToList();
        existingAssetDefinition.Description = request.Description;
        existingAssetDefinition.Notes = request.Notes;
        existingAssetDefinition.Tags = request.Tags;

        var success = await assetDefinitionRepository.UpdateAsync(existingAssetDefinition, cancellationToken);

        return new UpdateAssetDefinitionResponse(success);
    }
}
