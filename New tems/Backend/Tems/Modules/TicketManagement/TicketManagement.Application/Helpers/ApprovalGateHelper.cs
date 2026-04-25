using System.Security.Claims;
using TicketManagement.Application.Domain;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Helpers;

public static class ApprovalGateHelper
{
    private static readonly HashSet<string> PendingStates = new(StringComparer.OrdinalIgnoreCase)
    {
        "pending"
    };

    private static readonly HashSet<string> ApprovedStates = new(StringComparer.OrdinalIgnoreCase)
    {
        "approved"
    };

    private static readonly HashSet<string> RejectedStates = new(StringComparer.OrdinalIgnoreCase)
    {
        "rejected",
        "not-approved",
        "not_approved"
    };

    public static string NormalizeState(string? value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant().Replace('_', '-').Replace(' ', '-');
    }

    public static string ResolveGateState(ApprovalGate gate)
    {
        if (gate.Approvers.Any(approver => IsRejected(approver.Status)))
        {
            return "not-approved";
        }

        var approvedCount = gate.Approvers.Count(approver => IsApproved(approver.Status));
        if (approvedCount == 0)
        {
            return "pending";
        }

        if (gate.AllApproversRequired)
        {
            return gate.Approvers.All(approver => IsApproved(approver.Status))
                ? "approved"
                : "pending";
        }

        return "approved";
    }

    public static bool IsApproved(string? value) => ApprovedStates.Contains(NormalizeState(value));
    public static bool IsRejected(string? value) => RejectedStates.Contains(NormalizeState(value));
    public static bool IsPending(string? value) => PendingStates.Contains(NormalizeState(value));

    public static string GetLabel(string? value)
    {
        return NormalizeState(value) switch
        {
            "approved" => "Approved",
            "pending" => "Pending",
            "not-approved" => "Not approved",
            "rejected" => "Rejected",
            _ => string.IsNullOrWhiteSpace(value) ? string.Empty : value
        };
    }

    public static string? GetCurrentUserId(ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue("sub")
            ?? principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal?.FindFirstValue("preferred_username");
    }

    public static async Task<string?> ResolveCurrentUserIdAsync(
        ClaimsPrincipal? principal,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        var identityProviderId = principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal?.FindFirstValue("sub")
            ?? principal?.FindFirstValue("oid")
            ?? principal?.FindFirstValue("preferred_username")
            ?? principal?.FindFirstValue("upn")
            ?? principal?.FindFirstValue("unique_name");

        if (!string.IsNullOrWhiteSpace(identityProviderId))
        {
            var user = await userRepository.GetByIdentityProviderIdAsync(identityProviderId, cancellationToken)
                ?? await userRepository.GetByKeycloakIdAsync(identityProviderId, cancellationToken)
                ?? await userRepository.GetByEmailAsync(identityProviderId, cancellationToken);
            if (user != null && !string.IsNullOrWhiteSpace(user.Id))
            {
                return user.Id;
            }
        }

        var email = principal?.FindFirstValue(ClaimTypes.Email)
            ?? principal?.FindFirstValue("email");

        if (!string.IsNullOrWhiteSpace(email))
        {
            var user = await userRepository.GetByEmailAsync(email, cancellationToken);
            if (user != null && !string.IsNullOrWhiteSpace(user.Id))
            {
                return user.Id;
            }
        }

        return principal?.FindFirstValue("preferred_username")
            ?? principal?.FindFirstValue("sub")
            ?? identityProviderId;
    }

    public static async Task<HashSet<string>> ResolveCurrentUserIdentifiersAsync(
        ClaimsPrincipal? principal,
        IUserRepository userRepository,
        CancellationToken cancellationToken = default)
    {
        var identifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var identityProviderId = principal?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal?.FindFirstValue("sub")
            ?? principal?.FindFirstValue("oid")
            ?? principal?.FindFirstValue("preferred_username")
            ?? principal?.FindFirstValue("upn")
            ?? principal?.FindFirstValue("unique_name");

        if (!string.IsNullOrWhiteSpace(identityProviderId))
        {
            identifiers.Add(identityProviderId.Trim());
        }

        var preferredUsername = principal?.FindFirstValue("preferred_username");
        if (!string.IsNullOrWhiteSpace(preferredUsername))
        {
            identifiers.Add(preferredUsername.Trim());
        }

        var email = principal?.FindFirstValue(ClaimTypes.Email)
            ?? principal?.FindFirstValue("email");
        if (!string.IsNullOrWhiteSpace(email))
        {
            identifiers.Add(email.Trim());
        }

        var name = principal?.FindFirstValue("name")
            ?? principal?.FindFirstValue(ClaimTypes.Name);
        if (!string.IsNullOrWhiteSpace(name))
        {
            identifiers.Add(name.Trim());
        }

        if (!string.IsNullOrWhiteSpace(identityProviderId))
        {
            var user = await userRepository.GetByIdentityProviderIdAsync(identityProviderId, cancellationToken)
                ?? await userRepository.GetByKeycloakIdAsync(identityProviderId, cancellationToken)
                ?? await userRepository.GetByEmailAsync(email ?? string.Empty, cancellationToken)
                ?? await userRepository.GetByEmailAsync(preferredUsername ?? string.Empty, cancellationToken);
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(user.Id))
                {
                    identifiers.Add(user.Id.Trim());
                }

                if (!string.IsNullOrWhiteSpace(user.IdentityProviderId))
                {
                    identifiers.Add(user.IdentityProviderId.Trim());
                }

                if (!string.IsNullOrWhiteSpace(user.KeycloakId))
                {
                    identifiers.Add(user.KeycloakId.Trim());
                }

                if (!string.IsNullOrWhiteSpace(user.Name))
                {
                    identifiers.Add(user.Name.Trim());
                }

                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    identifiers.Add(user.Email.Trim());
                }
            }
        }
        else if (!string.IsNullOrWhiteSpace(email))
        {
            var user = await userRepository.GetByEmailAsync(email, cancellationToken);
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(user.Id))
                {
                    identifiers.Add(user.Id.Trim());
                }

                if (!string.IsNullOrWhiteSpace(user.IdentityProviderId))
                {
                    identifiers.Add(user.IdentityProviderId.Trim());
                }

                if (!string.IsNullOrWhiteSpace(user.KeycloakId))
                {
                    identifiers.Add(user.KeycloakId.Trim());
                }

                if (!string.IsNullOrWhiteSpace(user.Name))
                {
                    identifiers.Add(user.Name.Trim());
                }

                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    identifiers.Add(user.Email.Trim());
                }
            }
        }

        return identifiers;
    }

    public static bool IsManager(ClaimsPrincipal? principal)
    {
        return principal?.IsInRole("can_manage_tickets") == true;
    }

    public static bool CanViewTicket(Ticket ticket, ISet<string>? currentUserIdentifiers, bool isManager)
    {
        if (currentUserIdentifiers == null || currentUserIdentifiers.Count == 0)
        {
            return false;
        }

        if (isManager)
        {
            return true;
        }

        if (MatchesCurrentUser(ticket.Reporter.UserId, currentUserIdentifiers))
        {
            return true;
        }

        return (ticket.ApprovalGates ?? new List<ApprovalGate>()).Any(gate =>
            gate.Approvers.Any(approver => MatchesCurrentUser(approver.UserId, currentUserIdentifiers)));
    }

    public static bool CanConfigureApprovalGates(Ticket ticket, ISet<string>? currentUserIdentifiers, bool isManager)
    {
        if (currentUserIdentifiers == null || currentUserIdentifiers.Count == 0)
        {
            return false;
        }

        return isManager || MatchesCurrentUser(ticket.Reporter.UserId, currentUserIdentifiers);
    }

    public static bool CanReviewGate(ApprovalGate gate, ISet<string>? currentUserIdentifiers)
    {
        if (currentUserIdentifiers == null || currentUserIdentifiers.Count == 0)
        {
            return false;
        }

        return gate.Approvers.Any(approver => MatchesCurrentUser(approver.UserId, currentUserIdentifiers));
    }

    public static bool EnsureApprovalGateIds(Ticket ticket)
    {
        if (ticket.ApprovalGates == null || ticket.ApprovalGates.Count == 0)
        {
            return false;
        }

        var changed = false;
        foreach (var gate in ticket.ApprovalGates)
        {
            if (string.IsNullOrWhiteSpace(gate.ApprovalGateId))
            {
                gate.ApprovalGateId = Guid.NewGuid().ToString();
                changed = true;
            }
        }

        return changed;
    }

    public static string GetCurrentUserGateStatus(Ticket ticket, ISet<string>? currentUserIdentifiers)
    {
        if (currentUserIdentifiers == null || currentUserIdentifiers.Count == 0)
        {
            return string.Empty;
        }

        var statuses = (ticket.ApprovalGates ?? new List<ApprovalGate>())
            .Where(gate => gate.Approvers.Any(approver => MatchesCurrentUser(approver.UserId, currentUserIdentifiers)))
            .SelectMany(gate => gate.Approvers.Where(approver => MatchesCurrentUser(approver.UserId, currentUserIdentifiers)))
            .Select(approver => NormalizeState(approver.Status))
            .ToList();

        if (statuses.Any(IsRejected))
        {
            return "rejected";
        }

        if (statuses.Any(IsApproved))
        {
            return "approved";
        }

        if (statuses.Count > 0)
        {
            return "pending";
        }

        return string.Empty;
    }

    public static bool MatchesCurrentUser(string? value, ISet<string>? currentUserIdentifiers)
    {
        if (string.IsNullOrWhiteSpace(value) || currentUserIdentifiers == null || currentUserIdentifiers.Count == 0)
        {
            return false;
        }

        return currentUserIdentifiers.Contains(value.Trim());
    }
}
