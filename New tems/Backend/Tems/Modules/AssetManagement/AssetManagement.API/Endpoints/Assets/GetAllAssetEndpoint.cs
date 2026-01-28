using AssetManagement.Contract.Commands;
using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.Assets;

public class GetAllAssetEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllAssetResponse>
{
    public override void Configure()
    {
        Get("/asset");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var includeArchived = Query<bool>("includeArchived", false);
        var assetTypeIdsParam = Query<string>("assetTypeIds", false);
        var definitionIdsParam = Query<string>("definitionIds", false);
        var assetTag = Query<string>("assetTag", false);
        var locationId = Query<string>("locationId", false);
        var pageNumber = Query<int>("pageNumber", false);
        var pageSize = Query<int>("pageSize", false);
        
        var typeIdList = string.IsNullOrEmpty(assetTypeIdsParam) 
            ? null 
            : assetTypeIdsParam.Split(',').ToList();

        var definitionIdList = string.IsNullOrEmpty(definitionIdsParam)
            ? null
            : definitionIdsParam.Split(',').ToList();

        var filter = new AssetFilterDto(
            AssetTag: string.IsNullOrWhiteSpace(assetTag) ? null : assetTag,
            AssetTypeIds: typeIdList,
            DefinitionIds: definitionIdList,
            LocationId: string.IsNullOrWhiteSpace(locationId) ? null : locationId,
            IncludeArchived: includeArchived
        );
        
        var command = new GetAllAssetCommand(
            Filter: filter,
            PageNumber: pageNumber > 0 ? pageNumber : 1, 
            PageSize: pageSize > 0 ? pageSize : 50);
        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}
