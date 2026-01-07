using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetProperties;

public class GetByIdAssetPropertyEndpoint(IMediator mediator) : EndpointWithoutRequest<GetByIdAssetPropertyResponse>
{
    public override void Configure()
    {
        Get("/asset-property/{PropertyId}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("PropertyId")!;
        var command = new GetByIdAssetPropertyCommand(id);
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}