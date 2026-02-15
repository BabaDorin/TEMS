using AssetManagement.Contract.Commands;
using AssetManagement.Contract.DTOs;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace UserManagement.API.Endpoints.Users;

public class GetAssetsByUserIdEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllAssetResponse>
{
    public override void Configure()
    {
        Get("/users/{userId}/assets");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Route<string>("userId");
        var pageNumber = Query<int>("pageNumber", false);
        var pageSize = Query<int>("pageSize", false);
        
        var filter = new AssetFilterDto(
            AssignedToUserId: userId,
            IncludeArchived: false
        );
        
        var command = new GetAllAssetCommand(
            Filter: filter,
            PageNumber: pageNumber > 0 ? pageNumber : 1, 
            PageSize: pageSize > 0 ? pageSize : 50);
            
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, ct);
    }
}
