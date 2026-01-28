using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.AssetTypes;

public class GetAllAssetTypeEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllAssetTypeResponse>
{
    public override void Configure()
    {
        Get("/asset-type");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var includeArchived = Query<bool>("includeArchived", false);
        var command = new GetAllAssetTypeCommand(includeArchived);
        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}
