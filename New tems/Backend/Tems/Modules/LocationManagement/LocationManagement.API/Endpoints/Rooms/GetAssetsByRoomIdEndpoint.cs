using AssetManagement.Contract.Commands;
using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace LocationManagement.Api.Endpoints.Rooms;

public class GetAssetsByRoomIdEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllAssetResponse>
{
    public override void Configure()
    {
        Get("/location/rooms/{roomId}/assets");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roomId = Route<string>("roomId");
        var assetTypeIdsParam = Query<string>("assetTypeIds", false);
        var definitionIdsParam = Query<string>("definitionIds", false);
        var definitionNamesParam = Query<string>("definitionNames", false);
        var assetTag = Query<string>("assetTag", false);
        var pageNumber = Query<int>("pageNumber", false);
        var pageSize = Query<int>("pageSize", false);

        var typeIdList = string.IsNullOrEmpty(assetTypeIdsParam)
            ? null
            : assetTypeIdsParam.Split(',').ToList();

        var definitionIdList = string.IsNullOrEmpty(definitionIdsParam)
            ? null
            : definitionIdsParam.Split(',').ToList();

        var definitionNameList = string.IsNullOrEmpty(definitionNamesParam)
            ? null
            : definitionNamesParam.Split(',').ToList();
        
        var filter = new AssetFilterDto(
            AssetTag: string.IsNullOrWhiteSpace(assetTag) ? null : assetTag,
            AssetTypeIds: typeIdList,
            DefinitionIds: definitionIdList,
            DefinitionNames: definitionNameList,
            LocationId: roomId,
            IncludeArchived: false
        );
        
        var command = new GetAllAssetCommand(
            Filter: filter,
            PageNumber: pageNumber > 0 ? pageNumber : 1, 
            PageSize: pageSize > 0 ? pageSize : 20);
            
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
