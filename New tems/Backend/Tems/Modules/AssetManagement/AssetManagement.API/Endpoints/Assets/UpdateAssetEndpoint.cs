using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.Assets;

public class UpdateAssetEndpoint(IMediator mediator) : Endpoint<UpdateAssetCommand, UpdateAssetResponse>
{
    public override void Configure()
    {
        Put("/asset/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(UpdateAssetCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
