using System.Security.Claims;
using AssetManagement.Application.Interfaces;
using AssetManagement.Contract.Commands;
using AssetManagement.Contract.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tems.Common.Notifications;

namespace AssetManagement.Application.Commands;

public class AddAssetHistoryLogCommandHandler(
    IAssetRepository assetRepository,
    IHttpContextAccessor httpContextAccessor,
    IPublisher publisher)
    : IRequestHandler<AddAssetHistoryLogCommand, AddAssetHistoryLogResponse>
{
    private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Maintenance log",
        "Component replacement",
        "Other"
    };

    private static readonly HashSet<string> AllowedCurrencies = new(StringComparer.OrdinalIgnoreCase)
    {
        "MDL",
        "EUR",
        "USD"
    };

    public async Task<AddAssetHistoryLogResponse> Handle(AddAssetHistoryLogCommand request, CancellationToken cancellationToken)
    {
        var asset = await assetRepository.GetByIdAsync(request.Id, cancellationToken);
        if (asset == null)
        {
            return new AddAssetHistoryLogResponse(false);
        }

        var description = request.Description.Trim();
        if (string.IsNullOrWhiteSpace(description) || description.Length > 400)
        {
            return new AddAssetHistoryLogResponse(false);
        }

        if (!TryNormalizeType(request.Type, out var type))
        {
            return new AddAssetHistoryLogResponse(false);
        }

        if (!TryNormalizeCost(request.CostIncluded, request.CostAmount, request.CostCurrency, out var amount, out var currency))
        {
            return new AddAssetHistoryLogResponse(false);
        }

        var principal = httpContextAccessor.HttpContext?.User;

        await publisher.Publish(new AssetManualLogAddedNotification(
            asset.Id,
            asset.AssetTag,
            type,
            description,
            amount,
            currency,
            ResolveCurrentUserId(principal),
            ResolveCurrentUserName(principal)
        ), cancellationToken);

        return new AddAssetHistoryLogResponse(true);
    }

    private static bool TryNormalizeType(string type, out string normalizedType)
    {
        var normalized = type.Trim();
        if (!AllowedTypes.Contains(normalized))
        {
            normalizedType = string.Empty;
            return false;
        }

        normalizedType = AllowedTypes.First(value => value.Equals(normalized, StringComparison.OrdinalIgnoreCase));
        return true;
    }

    private static bool TryNormalizeCost(bool costIncluded, decimal? costAmount, string? costCurrency, out decimal? normalizedAmount, out string? normalizedCurrency)
    {
        if (!costIncluded)
        {
            normalizedAmount = null;
            normalizedCurrency = null;
            return true;
        }

        if (!costAmount.HasValue || costAmount.Value <= 0)
        {
            normalizedAmount = null;
            normalizedCurrency = null;
            return false;
        }

        var currency = costCurrency?.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(currency) || !AllowedCurrencies.Contains(currency))
        {
            normalizedAmount = null;
            normalizedCurrency = null;
            return false;
        }

        normalizedAmount = decimal.Round(costAmount.Value, 2);
        normalizedCurrency = currency;
        return true;
    }

    private static string? ResolveCurrentUserId(ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal?.FindFirstValue("sub");
    }

    private static string ResolveCurrentUserName(ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue("name")
            ?? principal?.FindFirstValue("preferred_username")
            ?? principal?.FindFirstValue(ClaimTypes.Email)
            ?? principal?.FindFirstValue("email")
            ?? principal?.FindFirstValue("sub")
            ?? "Unknown user";
    }
}
