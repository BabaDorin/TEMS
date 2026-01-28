using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.Assets;

public class GetByIdAssetEndpoint(IMediator mediator) : EndpointWithoutRequest<GetByIdAssetResponse>
{
    public override void Configure()
    {
        Get("/asset/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("Id")!;
        var command = new GetByIdAssetCommand(id);
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
