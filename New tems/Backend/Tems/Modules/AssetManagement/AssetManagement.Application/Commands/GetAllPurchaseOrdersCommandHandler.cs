using AssetManagement.Application.Helpers;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;
using Tems.Common.Tenant;
using UserManagement.Infrastructure.Repositories;

namespace AssetManagement.Application.Commands;

public class GetAllPurchaseOrdersCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IAssetRepository assetRepository,
    IUserRepository userRepository,
    ITenantContext tenantContext) : IRequestHandler<GetAllPurchaseOrdersCommand, GetAllPurchaseOrdersResponse>
{
    public async Task<GetAllPurchaseOrdersResponse> Handle(GetAllPurchaseOrdersCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrders = await purchaseOrderRepository.GetAllAsync(tenantContext.TenantId, cancellationToken);
        var results = new List<PurchaseOrderDto>(purchaseOrders.Count);

        foreach (var purchaseOrder in purchaseOrders)
        {
            var linkedAssets = await assetRepository.GetByPurchaseOrderIdAsync(purchaseOrder.Id, cancellationToken);
            results.Add(await PurchaseOrderResponseFactory.CreateAsync(purchaseOrder, linkedAssets, userRepository, cancellationToken));
        }

        return new GetAllPurchaseOrdersResponse(results);
    }
}
