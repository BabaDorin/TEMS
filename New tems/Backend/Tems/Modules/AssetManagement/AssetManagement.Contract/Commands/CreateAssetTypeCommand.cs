using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record CreateAssetTypeCommand(
    string Name,
    string Description,
    string? ParentTypeId,
    List<AssetTypePropertyDto> Properties,
    string CreatedBy) 
    : IRequest<CreateAssetTypeResponse>;

public record AssetTypePropertyDto(
    string PropertyId,
    string Name,
    string Description,
    string DataType,
    bool Required,
    PropertyValidationDto? Validation,
    int DisplayOrder);
