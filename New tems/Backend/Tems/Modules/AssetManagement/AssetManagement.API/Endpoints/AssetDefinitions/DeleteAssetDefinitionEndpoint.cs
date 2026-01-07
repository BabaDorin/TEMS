using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetDefinitions;

public class DeleteAssetDefinitionEndpoint(IMediator mediator) : Endpoint<DeleteAssetDefinitionCommand, DeleteAssetDefinitionResponse>
{
    public override void Configure()
    {
        Delete("/asset-definition/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(DeleteAssetDefinitionCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
