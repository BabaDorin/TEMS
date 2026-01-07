using AssetManagement.Contract.Commands;
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
        var command = new GetAllAssetCommand(includeArchived);
        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}
