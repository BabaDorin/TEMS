using AssetManagement.Application.Interfaces;
using FastEndpoints;

namespace UserManagement.API.Endpoints.Users;

public class GetUserAssetCountEndpoint(IAssetRepository assetRepository) : EndpointWithoutRequest<GetUserAssetCountResponse>
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

        var assets = await assetRepository.GetByAssignedUserIdAsync(userId, ct);
        
        await Send.OkAsync(new GetUserAssetCountResponse(assets.Count), ct);
    }
}

public record GetUserAssetCountResponse(int Count);
