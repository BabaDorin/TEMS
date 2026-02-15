using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.Assets;

public class UnassignAssetFromUserEndpoint(IMediator mediator) : EndpointWithoutRequest<AssetDto>
{
    public override void Configure()
    {
        Delete("/asset/{AssetId}/user");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var assetId = Route<string>("AssetId") ?? throw new ArgumentException("Asset ID is required");
        var command = new UnassignAssetFromUserCommand(assetId);
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, ct);
    }
}
