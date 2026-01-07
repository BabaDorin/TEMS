using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetDefinitions;

public class GetByIdAssetDefinitionEndpoint(IMediator mediator) : EndpointWithoutRequest<GetByIdAssetDefinitionResponse>
{
    public override void Configure()
    {
        Get("/asset-definition/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("Id")!;
        var command = new GetByIdAssetDefinitionCommand(id);
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
