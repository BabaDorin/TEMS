using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetTypes;

public class GetByIdAssetTypeEndpoint(IMediator mediator) : EndpointWithoutRequest<GetByIdAssetTypeResponse>
{
    public override void Configure()
    {
        Get("/asset-type/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("Id")!;
        var command = new GetByIdAssetTypeCommand(id);
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
