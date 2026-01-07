using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetProperties;

public class CreateAssetPropertyEndpoint(IMediator mediator) : Endpoint<CreateAssetPropertyCommand, CreateAssetPropertyResponse>
{
    public override void Configure()
    {
        Post("/asset-property");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CreateAssetPropertyCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}