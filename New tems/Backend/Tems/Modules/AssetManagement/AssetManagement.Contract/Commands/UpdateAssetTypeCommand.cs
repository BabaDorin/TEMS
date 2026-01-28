using AssetManagement.Contract.Responses;
using MediatR;

namespace AssetManagement.Contract.Commands;

public record UpdateAssetTypeCommand(
    string Id,
    string Name,
    string Description,
    string? ParentTypeId,
    List<AssetTypePropertyDto> Properties) 
    : IRequest<UpdateAssetTypeResponse>;
