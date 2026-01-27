using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record UpdateAssetCommand(
    string Id,
    string SerialNumber,
    string AssetTag,
    string Status,
    bool IsCustomized,
    List<AssetSpecificationDto> Specifications,
    PurchaseInfoDto? PurchaseInfo,
    string? LocationId,
    AssetLocationDto? Location,
    AssetAssignmentDto? Assignment,
    string? ParentAssetId,
    List<string> ChildAssetIds,
    string Notes,
    List<MaintenanceRecordDto> MaintenanceHistory) 
    : IRequest<UpdateAssetResponse>;
