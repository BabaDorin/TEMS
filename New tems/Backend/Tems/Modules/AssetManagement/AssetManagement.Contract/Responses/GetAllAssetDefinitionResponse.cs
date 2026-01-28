using AssetManagement.Contract.Commands;
using AssetManagement.Contract.DTOs;

namespace AssetManagement.Contract.Responses;

public record GetAllAssetDefinitionResponse(List<AssetDefinitionDto> AssetDefinitions);

public record AssetDefinitionDto(
    string Id,
    string Name,
    string ShortName,
    string AssetTypeId,
    string AssetTypeName,
    string Manufacturer,
    string Model,
    List<AssetSpecificationDto> Specifications,
    int UsageCount,
    string Description,
    string Notes,
    List<string> Tags,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string CreatedBy,
    bool IsArchived,
    DateTime? ArchivedAt,
    string? ArchivedBy);
