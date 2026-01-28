using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.Assets;

public class DeleteAssetEndpoint(IMediator mediator) : Endpoint<DeleteAssetCommand, DeleteAssetResponse>
{
    public override void Configure()
    {
        Delete("/asset/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(DeleteAssetCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
