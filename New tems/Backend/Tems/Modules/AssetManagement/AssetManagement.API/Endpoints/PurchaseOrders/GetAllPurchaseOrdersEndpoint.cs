using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.PurchaseOrders;

public class GetAllPurchaseOrdersEndpoint(IMediator mediator) : EndpointWithoutRequest<GetAllPurchaseOrdersResponse>
{
    public override void Configure()
    {
        Get("/purchase-orders");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await mediator.Send(new GetAllPurchaseOrdersCommand(), ct);
        await Send.OkAsync(response, ct);
    }
}
