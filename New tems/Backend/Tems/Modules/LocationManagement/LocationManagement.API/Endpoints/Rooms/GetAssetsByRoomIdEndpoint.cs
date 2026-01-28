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
        var pageNumber = Query<int>("pageNumber", false);
        var pageSize = Query<int>("pageSize", false);
        
        var filter = new AssetFilterDto(
            LocationId: roomId,
            IncludeArchived: false
        );
        
        var command = new GetAllAssetCommand(
            Filter: filter,
            PageNumber: pageNumber > 0 ? pageNumber : 1, 
            PageSize: pageSize > 0 ? pageSize : 50);
            
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
