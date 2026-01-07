using AssetManagement.Contract.DTOs;

namespace AssetManagement.Contract.Responses;

public record GetAllAssetPropertyResponse(List<AssetPropertyDto> Properties);

public record AssetPropertyDto(
    string PropertyId,
    string Name,
    string Description,
    string DataType,
    string Category,
    PropertyValidationDto? DefaultValidation,
    List<string> EnumValues,
    string? Unit,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string CreatedBy);