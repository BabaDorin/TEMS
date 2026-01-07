using AssetManagement.Contract.Commands;

namespace AssetManagement.Contract.Responses;

public record GetByIdAssetTypeResponse(AssetTypeDto? AssetType);
