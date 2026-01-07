using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetProperties;

public class GetAllAssetPropertyEndpoint(IMediator mediator)
    : EndpointWithoutRequest<GetAllAssetPropertyResponse>
{
    public override void Configure()
    {
        Get("/asset-property");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var command = new GetAllAssetPropertyCommand();
        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}