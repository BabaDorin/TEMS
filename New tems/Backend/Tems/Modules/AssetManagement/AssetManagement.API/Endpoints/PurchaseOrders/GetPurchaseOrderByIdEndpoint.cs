using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.PurchaseOrders;

public class GetPurchaseOrderByIdEndpoint(IMediator mediator) : EndpointWithoutRequest<GetPurchaseOrderByIdResponse>
{
    public override void Configure()
    {
        Get("/purchase-orders/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<string>("Id")!;
        var response = await mediator.Send(new GetPurchaseOrderByIdCommand(id), ct);
        await Send.OkAsync(response, ct);
    }
}
