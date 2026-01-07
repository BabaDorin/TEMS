using AssetManagement.Contract.Commands;
using AssetManagement.Contract.DTOs;

namespace AssetManagement.Contract.Responses;

public record GetAllAssetTypeResponse(List<AssetTypeDto> AssetTypes);

public record AssetTypeDto(
    string Id,
    string Name,
    string Description,
    string? ParentTypeId,
    List<AssetTypePropertyDto> Properties,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string CreatedBy,
    bool IsArchived,
    DateTime? ArchivedAt,
    string? ArchivedBy);
