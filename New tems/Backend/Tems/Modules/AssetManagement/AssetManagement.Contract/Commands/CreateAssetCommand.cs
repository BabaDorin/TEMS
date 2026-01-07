using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record CreateAssetCommand(
    string SerialNumber,
    string AssetTag,
    string Status,
    string DefinitionId,
    bool CustomizeDefinition,
    List<AssetSpecificationDto>? CustomSpecifications,
    PurchaseInfoDto? PurchaseInfo,
    AssetLocationDto? Location,
    AssetAssignmentDto? Assignment,
    string? ParentAssetId,
    List<string> ChildAssetIds,
    string Notes,
    string CreatedBy) 
    : IRequest<CreateAssetResponse>;

public record PurchaseInfoDto(
    DateTime? PurchaseDate,
    decimal? PurchasePrice,
    string Currency,
    string Vendor,
    DateTime? WarrantyExpiry);

public record AssetLocationDto(
    string Building,
    string Room,
    string Desk);

public record AssetAssignmentDto(
    string? AssignedToUserId,
    string AssignedToName,
    DateTime? AssignedAt,
    string AssignmentType);

public record MaintenanceRecordDto(
    DateTime Date,
    string Type,
    string Description,
    string PerformedBy,
    decimal? Cost);
