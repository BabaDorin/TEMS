using AssetManagement.Contract.Commands;
using FastEndpoints;
using MediatR;

namespace UserManagement.API.Endpoints.Users;

public class GetUserAssetCountEndpoint(IMediator mediator) : EndpointWithoutRequest<GetUserAssetCountResponse>
{
    public override void Configure()
    {
        Get("/users/{userId}/assets/count");
        Policies("CanManageUsers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Route<string>("userId");
        
        if (string.IsNullOrEmpty(userId))
        {
            await Send.OkAsync(new GetUserAssetCountResponse(0), ct);
            return;
        }

        var response = await mediator.Send(new GetAssetCountsByUsersCommand([userId]), ct);
        var totalCount = 0;

        if (response.Success && response.Data.TryGetValue(userId, out var counts))
        {
            totalCount = counts.Values.Sum();
        }

        await Send.OkAsync(new GetUserAssetCountResponse(totalCount), ct);
    }
}

public record GetUserAssetCountResponse(int Count);
