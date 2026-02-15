using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.Assets;

public record AssignAssetToUserRequest(string UserId, string UserName);

public class AssignAssetToUserEndpoint(IMediator mediator) : Endpoint<AssignAssetToUserRequest, AssetDto>
{
    public override void Configure()
    {
        Put("/asset/{AssetId}/user");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(AssignAssetToUserRequest request, CancellationToken ct)
    {
        var assetId = Route<string>("AssetId") ?? throw new ArgumentException("Asset ID is required");
        var command = new AssignAssetToUserCommand(assetId, request.UserId, request.UserName);
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, ct);
    }
}
