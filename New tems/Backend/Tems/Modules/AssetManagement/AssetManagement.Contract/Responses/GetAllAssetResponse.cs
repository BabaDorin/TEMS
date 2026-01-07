using AssetManagement.Contract.Commands;
using AssetManagement.Contract.DTOs;

namespace AssetManagement.Contract.Responses;

public record GetAllAssetResponse(List<AssetDto> Assets);

public record AssetDto(
    string Id,
    string SerialNumber,
    string AssetTag,
    string Status,
    AssetDefinitionSnapshotDto Definition,
    PurchaseInfoDto? PurchaseInfo,
    AssetLocationDto? Location,
    AssetAssignmentDto? Assignment,
    string? ParentAssetId,
    List<string> ChildAssetIds,
    string Notes,
    List<MaintenanceRecordDto> MaintenanceHistory,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string CreatedBy,
    bool IsArchived,
    DateTime? ArchivedAt,
    string? ArchivedBy);

public record AssetDefinitionSnapshotDto(
    string DefinitionId,
    bool IsCustomized,
    DateTime SnapshotAt,
    string Name,
    string AssetTypeId,
    string AssetTypeName,
    string Manufacturer,
    string Model,
    List<AssetSpecificationDto> Specifications);
