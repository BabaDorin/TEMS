using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetProperties;
 
public class DeleteAssetPropertyEndpoint(IMediator mediator) : Endpoint<DeleteAssetPropertyCommand, DeleteAssetPropertyResponse>
{
    public override void Configure()
    {
        Delete("/asset-property/{PropertyId}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(DeleteAssetPropertyCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
