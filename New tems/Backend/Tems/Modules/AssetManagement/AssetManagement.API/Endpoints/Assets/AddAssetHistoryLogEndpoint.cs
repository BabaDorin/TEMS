using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.Assets;

public class AddAssetHistoryLogEndpoint(IMediator mediator) : Endpoint<AddAssetHistoryLogCommand, AddAssetHistoryLogResponse>
{
    public override void Configure()
    {
        Post("/asset/{Id}/history-log");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(AddAssetHistoryLogCommand command, CancellationToken ct)
    {
        var response = await mediator.Send(command, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
