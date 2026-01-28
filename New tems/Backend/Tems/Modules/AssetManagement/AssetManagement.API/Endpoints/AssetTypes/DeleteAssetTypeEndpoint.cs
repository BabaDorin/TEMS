using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetTypes;

public class DeleteAssetTypeEndpoint(IMediator mediator) : Endpoint<DeleteAssetTypeCommand, DeleteAssetTypeResponse>
{
    public override void Configure()
    {
        Delete("/asset-type/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(DeleteAssetTypeCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
