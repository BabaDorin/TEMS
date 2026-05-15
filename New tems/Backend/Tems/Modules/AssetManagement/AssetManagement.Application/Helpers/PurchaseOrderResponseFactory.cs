using AssetManagement.Application.Domain;
using AssetManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;

namespace AssetManagement.Application.Helpers;

internal static class PurchaseOrderResponseFactory
{
    public static async Task<PurchaseOrderDto> CreateAsync(
        PurchaseOrder purchaseOrder,
        IReadOnlyCollection<Asset> assets,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        var createdByDisplayName = await ResolveUserDisplayNameAsync(purchaseOrder.CreatedByUserId, userRepository, cancellationToken);
        var accountableDisplayName = await ResolveUserDisplayNameAsync(purchaseOrder.AccountableUserId, userRepository, cancellationToken);

        var items = assets
            .OrderBy(asset => asset.AssetTag, StringComparer.OrdinalIgnoreCase)
            .Select(asset => new PurchaseOrderItemDto(
                asset.Id,
                asset.AssetTag,
                asset.SerialNumber,
                asset.PurchaseInfo?.PurchasePrice ?? 0m,
                asset.CreatedAt
            ))
            .ToList();

        var usedAmount = items.Sum(item => item.Price);
        var availableAmount = purchaseOrder.Amount - usedAmount;

        return new PurchaseOrderDto(
            purchaseOrder.Id,
            purchaseOrder.TicketId,
            purchaseOrder.TicketHumanReadableId,
            purchaseOrder.PoNumber,
            purchaseOrder.Vendor,
            purchaseOrder.Amount,
            purchaseOrder.Currency,
            purchaseOrder.Description,
            purchaseOrder.CreatedByUserId,
            createdByDisplayName,
            purchaseOrder.AccountableUserId,
            accountableDisplayName,
            usedAmount,
            availableAmount,
            items.Count,
            items,
            purchaseOrder.CreatedAt,
            purchaseOrder.UpdatedAt
        );
    }

    private static async Task<string> ResolveUserDisplayNameAsync(
        string userId,
        IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return string.Empty;
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? await userRepository.GetByIdentityProviderIdAsync(userId, cancellationToken)
            ?? await userRepository.GetByKeycloakIdAsync(userId, cancellationToken)
            ?? await userRepository.GetByEmailAsync(userId, cancellationToken);

        if (user == null)
        {
            return userId;
        }

        if (!string.IsNullOrWhiteSpace(user.Name))
        {
            return user.Name.Trim();
        }

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            return user.Email.Trim();
        }

        return user.Id;
    }
}
