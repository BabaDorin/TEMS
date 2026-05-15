using AssetManagement.Application.Domain;
using AssetManagement.Application.Interfaces;
using TicketManagement.Application.Domain;

namespace TicketManagement.Application.Helpers;

internal static class TicketAssetLinkingHelper
{
    public static bool SupportsAssetLinking(TicketType ticketType)
    {
        var normalized = Normalize($"{ticketType.Name} {ticketType.Description} {ticketType.TicketTypeId}");
        return normalized.Contains("hardware issue", StringComparison.Ordinal)
            || normalized.Contains("hardware_issue", StringComparison.Ordinal)
            || normalized.Contains("network issue", StringComparison.Ordinal)
            || normalized.Contains("network_issue", StringComparison.Ordinal);
    }

    public static List<string> NormalizeAssetIds(IEnumerable<string>? assetIds)
    {
        return (assetIds ?? [])
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static async Task<List<Asset>> ResolveLinkedAssetsAsync(
        TicketType ticketType,
        IEnumerable<string>? assetIds,
        IAssetRepository assetRepository,
        CancellationToken cancellationToken = default)
    {
        var normalizedAssetIds = NormalizeAssetIds(assetIds);
        if (normalizedAssetIds.Count == 0)
        {
            return [];
        }

        if (!SupportsAssetLinking(ticketType))
        {
            throw new InvalidOperationException("Only Network Issue and Hardware Issue tickets can link assets.");
        }

        var assets = new List<Asset>(normalizedAssetIds.Count);
        foreach (var assetId in normalizedAssetIds)
        {
            var asset = await assetRepository.GetByIdAsync(assetId, cancellationToken);
            if (asset == null)
            {
                throw new KeyNotFoundException($"Asset with ID {assetId} not found");
            }

            assets.Add(asset);
        }

        return assets;
    }

    public static string FormatAssetTags(IEnumerable<Asset> assets)
    {
        var tags = assets
            .Select(asset => asset.AssetTag?.Trim())
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .ToList();

        return tags.Count == 0 ? "None" : string.Join(", ", tags);
    }

    public static bool AreSameSelections(IEnumerable<string>? left, IEnumerable<string>? right)
    {
        var leftSet = NormalizeAssetIds(left).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var rightSet = NormalizeAssetIds(right).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return leftSet.SetEquals(rightSet);
    }

    private static string Normalize(string? value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }
}
