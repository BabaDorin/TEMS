using AssetManagement.Application.Helpers;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tems.Common.Tenant;
using UserManagement.Infrastructure.Repositories;

namespace AssetManagement.Application.Commands;

public class DeletePurchaseOrderCommandHandler(
    IPurchaseOrderRepository purchaseOrderRepository,
    IAssetRepository assetRepository,
    ITenantContext tenantContext,
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository) : IRequestHandler<DeletePurchaseOrderCommand, DeletePurchaseOrderResponse>
{
    public async Task<DeletePurchaseOrderResponse> Handle(DeletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.Id, tenantContext.TenantId, cancellationToken);
        if (purchaseOrder == null)
        {
            throw new KeyNotFoundException($"Purchase order with ID {request.Id} not found");
        }

        var currentUserIdentifiers = await AssetUserAccessHelper.ResolveCurrentUserIdentifiersAsync(
            httpContextAccessor.HttpContext?.User,
            userRepository,
            cancellationToken);

        if (!AssetUserAccessHelper.MatchesCurrentUser(purchaseOrder.CreatedByUserId, currentUserIdentifiers) &&
            !AssetUserAccessHelper.MatchesCurrentUser(purchaseOrder.AccountableUserId, currentUserIdentifiers))
        {
            throw new UnauthorizedAccessException("Only the ticket author or accountable user can delete this purchase order");
        }

        var linkedAssets = await assetRepository.GetByPurchaseOrderIdAsync(purchaseOrder.Id, cancellationToken);
        foreach (var asset in linkedAssets)
        {
            if (asset.PurchaseInfo == null)
            {
                continue;
            }

            asset.PurchaseInfo.PurchaseOrderId = null;
            asset.PurchaseInfo.PurchaseOrderNumber = null;
            await assetRepository.UpdateAsync(asset, cancellationToken);
        }

        var success = await purchaseOrderRepository.DeleteAsync(request.Id, tenantContext.TenantId, cancellationToken);
        return new DeletePurchaseOrderResponse(success);
    }
}
