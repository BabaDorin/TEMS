using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record UpdateAssetDefinitionCommand(
    string Id,
    string Name,
    string ShortName,
    string AssetTypeId,
    string AssetTypeName,
    string Manufacturer,
    string Model,
    List<AssetSpecificationDto> Specifications,
    string Description,
    string Notes,
    List<string> Tags) 
    : IRequest<UpdateAssetDefinitionResponse>;
