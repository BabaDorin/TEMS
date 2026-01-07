using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetDefinitions;

public class GetAllAssetDefinitionEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllAssetDefinitionResponse>
{
    public override void Configure()
    {
        Get("/asset-definition");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var includeArchived = Query<bool>("includeArchived", false);
        var command = new GetAllAssetDefinitionCommand(includeArchived);
        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}
