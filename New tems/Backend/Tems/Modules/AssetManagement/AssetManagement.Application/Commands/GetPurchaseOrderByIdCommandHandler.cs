using AssetManagement.Application.Helpers;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;
using Tems.Common.Tenant;
using UserManagement.Infrastructure.Repositories;

namespace AssetManagement.Application.Commands;

public class GetPurchaseOrderByIdCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IAssetRepository assetRepository,
    IUserRepository userRepository,
    ITenantContext tenantContext) : IRequestHandler<GetPurchaseOrderByIdCommand, GetPurchaseOrderByIdResponse>
{
    public async Task<GetPurchaseOrderByIdResponse> Handle(GetPurchaseOrderByIdCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.Id, tenantContext.TenantId, cancellationToken);
        if (purchaseOrder == null)
        {
            return new GetPurchaseOrderByIdResponse(null);
        }

        var linkedAssets = await assetRepository.GetByPurchaseOrderIdAsync(purchaseOrder.Id, cancellationToken);
        var dto = await PurchaseOrderResponseFactory.CreateAsync(purchaseOrder, linkedAssets, userRepository, cancellationToken);

        return new GetPurchaseOrderByIdResponse(dto);
    }
}
