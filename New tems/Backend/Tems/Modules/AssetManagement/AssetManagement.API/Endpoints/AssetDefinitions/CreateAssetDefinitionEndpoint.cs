using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetDefinitions;

public class CreateAssetDefinitionEndpoint(IMediator mediator) : Endpoint<CreateAssetDefinitionCommand, CreateAssetDefinitionResponse>
{
    public override void Configure()
    {
        Post("/asset-definition");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CreateAssetDefinitionCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
