using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record CreateAssetDefinitionCommand(
    string Name,
    string ShortName,
    string AssetTypeId,
    string AssetTypeName,
    string Manufacturer,
    string Model,
    List<AssetSpecificationDto> Specifications,
    string Description,
    string Notes,
    List<string> Tags,
    string CreatedBy) 
    : IRequest<CreateAssetDefinitionResponse>;

public record AssetSpecificationDto(
    string PropertyId,
    string Name,
    object Value,
    string DataType,
    string? Unit);
