using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetDefinitions;

public class UpdateAssetDefinitionEndpoint(IMediator mediator) : Endpoint<UpdateAssetDefinitionCommand, UpdateAssetDefinitionResponse>
{
    public override void Configure()
    {
        Put("/asset-definition/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(UpdateAssetDefinitionCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
