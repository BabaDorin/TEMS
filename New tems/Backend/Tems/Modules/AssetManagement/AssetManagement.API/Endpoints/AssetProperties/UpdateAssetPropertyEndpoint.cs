using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetProperties;

public class UpdateAssetPropertyEndpoint(IMediator mediator) : Endpoint<UpdateAssetPropertyCommand, UpdateAssetPropertyResponse>
{
    public override void Configure()
    {
        Put("/asset-property/{PropertyId}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(UpdateAssetPropertyCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}