using AssetManagement.Application.Interfaces;
using TicketManagement.Application.Domain;
using TicketManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Helpers;

public static class TicketResponseFactory
{
    public static async Task<GetTicketResponse> ToResponseAsync(
        Ticket ticket,
        IAssetRepository assetRepository,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        var reporterDisplayName = await ResolveReporterDisplayNameAsync(ticket.Reporter.UserId, userRepository, cancellationToken);
        var accountableDisplayName = await ResolveReporterDisplayNameAsync(ticket.AccountableUserId, userRepository, cancellationToken);
        var linkedAssets = new List<AssetLinkResponse>();

        foreach (var assetId in ticket.AssetIds ?? [])
        {
            var asset = await assetRepository.GetByIdAsync(assetId, cancellationToken);
            if (asset == null)
            {
                continue;
            }

            linkedAssets.Add(new AssetLinkResponse(asset.Id, asset.AssetTag));
        }

        return new GetTicketResponse(
            ticket.TicketId,
            ticket.TenantId,
            ticket.TicketTypeId,
            ticket.HumanReadableId,
            string.IsNullOrWhiteSpace(ticket.Title) ? ticket.Summary : ticket.Title,
            ticket.Summary,
            ticket.AiSummary,
            ticket.CurrentStateId,
            ticket.Priority,
            new ReporterResponse(
                ticket.Reporter.UserId,
                ticket.Reporter.ChannelSource,
                ticket.Reporter.ChannelThreadId,
                reporterDisplayName
            ),
            ticket.AccountableUserId,
            accountableDisplayName,
            ticket.AssigneeId,
            ticket.Attributes,
            (ticket.AssetIds ?? []).ToList(),
            linkedAssets,
            ticket.ApprovalGates.Select(x => x.ToResponse()).ToList(),
            ticket.CreatedAt,
            ticket.UpdatedAt,
            ticket.ResolvedAt
        );
    }

    public static async Task<string> ResolveReporterDisplayNameAsync(
        string reporterUserId,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(reporterUserId))
        {
            return string.Empty;
        }

        var user = await userRepository.GetByIdAsync(reporterUserId, cancellationToken)
            ?? await userRepository.GetByIdentityProviderIdAsync(reporterUserId, cancellationToken)
            ?? await userRepository.GetByKeycloakIdAsync(reporterUserId, cancellationToken)
            ?? await userRepository.GetByEmailAsync(reporterUserId, cancellationToken);

        if (user == null)
        {
            return reporterUserId;
        }

        var displayName = user.Name?.Trim();
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            return displayName;
        }

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            return user.Email;
        }

        return user.Id;
    }
}
