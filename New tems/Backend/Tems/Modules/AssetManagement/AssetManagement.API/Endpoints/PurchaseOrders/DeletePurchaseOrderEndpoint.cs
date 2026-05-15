using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using FastEndpoints;
using MediatR;

namespace AssetManagement.API.Endpoints.PurchaseOrders;

public class DeletePurchaseOrderEndpoint(IMediator mediator) : Endpoint<DeletePurchaseOrderCommand, DeletePurchaseOrderResponse>
{
    public override void Configure()
    {
        Delete("/purchase-orders/{Id}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(DeletePurchaseOrderCommand command, CancellationToken ct)
    {
        try
        {
            var response = await mediator.Send(command, ct);
            await Send.OkAsync(response, ct);
        }
        catch (UnauthorizedAccessException)
        {
            await Send.ForbiddenAsync(ct);
        }
        catch (KeyNotFoundException)
        {
            await Send.NotFoundAsync(ct);
        }
    }
}
