using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetTypes;

public class CreateAssetTypeEndpoint(IMediator mediator) : Endpoint<CreateAssetTypeCommand, CreateAssetTypeResponse>
{
    public override void Configure()
    {
        Post("/asset-type");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CreateAssetTypeCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
