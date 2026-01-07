using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.Assets;

public class CreateAssetEndpoint(IMediator mediator) : Endpoint<CreateAssetCommand, CreateAssetResponse>
{
    public override void Configure()
    {
        Post("/asset");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CreateAssetCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
