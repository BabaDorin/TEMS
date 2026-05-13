using System.Security.Claims;
using ChangeLog.Application.Domain;
using ChangeLog.Application.Domain.TicketLogs;
using ChangeLog.Application.Interfaces;
using ChangeLog.Contract.Enums;
using Microsoft.AspNetCore.Http;
using Tems.Common.Tenant;
using TicketManagement.Application.Domain;
using TicketManagement.Application.Helpers;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Services;

public class TicketHistoryLogService(
    IChangeLogRepository changeLogRepository,
    ITenantContext tenantContext,
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository)
{
    public async Task LogCreatedAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        var actor = await ResolveActorAsync(cancellationToken);
        await changeLogRepository.CreateAsync(new TicketCreatedLog
        {
            TenantId = tenantContext.TenantId,
            Action = ChangeLogAction.TicketCreated,
            Description = $"Created ticket {ticket.HumanReadableId} in {ToStatusLabel(ticket.CurrentStateId)} status.",
            Timestamp = DateTime.UtcNow,
            PerformedByUserId = actor.UserId,
            PerformedByUserName = actor.UserName,
            TicketId = ticket.TicketId,
            HumanReadableId = ticket.HumanReadableId,
            Title = ticket.Title,
            Summary = ticket.Summary,
            Status = ToStatusLabel(ticket.CurrentStateId)
        }, cancellationToken);
    }

    public async Task LogUpdatedAsync(Ticket ticket, List<FieldChange> changes, string? overrideDescription = null, CancellationToken cancellationToken = default)
    {
        if (changes.Count == 0)
        {
            return;
        }

        var actor = await ResolveActorAsync(cancellationToken);
        var description = overrideDescription ?? BuildUpdateDescription(changes);

        await changeLogRepository.CreateAsync(new TicketUpdatedLog
        {
            TenantId = tenantContext.TenantId,
            Action = ChangeLogAction.TicketUpdated,
            Description = description,
            Timestamp = DateTime.UtcNow,
            PerformedByUserId = actor.UserId,
            PerformedByUserName = actor.UserName,
            TicketId = ticket.TicketId,
            HumanReadableId = ticket.HumanReadableId,
            Changes = changes
        }, cancellationToken);
    }

    public async Task LogStatusChangedAsync(Ticket ticket, string previousStatus, string newStatus, CancellationToken cancellationToken = default)
    {
        var actor = await ResolveActorAsync(cancellationToken);
        await changeLogRepository.CreateAsync(new TicketStatusUpdatedLog
        {
            TenantId = tenantContext.TenantId,
            Action = ChangeLogAction.TicketStatusUpdated,
            Description = $"Status changed from {ToStatusLabel(previousStatus)} to {ToStatusLabel(newStatus)}.",
            Timestamp = DateTime.UtcNow,
            PerformedByUserId = actor.UserId,
            PerformedByUserName = actor.UserName,
            TicketId = ticket.TicketId,
            HumanReadableId = ticket.HumanReadableId,
            PreviousStatus = ToStatusLabel(previousStatus),
            NewStatus = ToStatusLabel(newStatus)
        }, cancellationToken);
    }

    public async Task LogApprovalGateAddedAsync(Ticket ticket, ApprovalGate gate, CancellationToken cancellationToken = default)
    {
        var actor = await ResolveActorAsync(cancellationToken);
        var approvers = await ResolveApproverInfosAsync(gate.Approvers.Select(x => x.UserId), cancellationToken);
        var approverNames = approvers.Select(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        var description = approverNames.Count > 0
            ? $"Added approval gate \"{gate.Title}\" for {string.Join(", ", approverNames)}."
            : $"Added approval gate \"{gate.Title}\".";

        await changeLogRepository.CreateAsync(new TicketApprovalGateAddedLog
        {
            TenantId = tenantContext.TenantId,
            Action = ChangeLogAction.TicketApprovalGateAdded,
            Description = description,
            Timestamp = DateTime.UtcNow,
            PerformedByUserId = actor.UserId,
            PerformedByUserName = actor.UserName,
            TicketId = ticket.TicketId,
            HumanReadableId = ticket.HumanReadableId,
            ApprovalGateId = gate.ApprovalGateId,
            GateTitle = gate.Title,
            Justification = gate.Justification,
            AllApproversRequired = gate.AllApproversRequired,
            Approvers = approvers
        }, cancellationToken);
    }

    public async Task LogApprovalGateRemovedAsync(Ticket ticket, ApprovalGate gate, CancellationToken cancellationToken = default)
    {
        var actor = await ResolveActorAsync(cancellationToken);
        var approvers = await ResolveApproverInfosAsync(gate.Approvers.Select(x => x.UserId), cancellationToken);
        var approverNames = approvers.Select(x => x.Name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        var description = approverNames.Count > 0
            ? $"Removed approval gate \"{gate.Title}\" for {string.Join(", ", approverNames)}."
            : $"Removed approval gate \"{gate.Title}\".";

        await changeLogRepository.CreateAsync(new TicketApprovalGateRemovedLog
        {
            TenantId = tenantContext.TenantId,
            Action = ChangeLogAction.TicketApprovalGateRemoved,
            Description = description,
            Timestamp = DateTime.UtcNow,
            PerformedByUserId = actor.UserId,
            PerformedByUserName = actor.UserName,
            TicketId = ticket.TicketId,
            HumanReadableId = ticket.HumanReadableId,
            ApprovalGateId = gate.ApprovalGateId,
            GateTitle = gate.Title,
            Justification = gate.Justification,
            AllApproversRequired = gate.AllApproversRequired,
            Approvers = approvers
        }, cancellationToken);
    }

    public async Task LogApprovalGateReviewedAsync(
        Ticket ticket,
        ApprovalGate gate,
        string approverUserId,
        string reviewStatus,
        CancellationToken cancellationToken = default)
    {
        var actor = await ResolveActorAsync(cancellationToken);
        var approverName = await ResolveUserDisplayNameAsync(approverUserId, cancellationToken);
        var reviewLabel = ApprovalGateHelper.GetLabel(reviewStatus);

        await changeLogRepository.CreateAsync(new TicketApprovalGateReviewedLog
        {
            TenantId = tenantContext.TenantId,
            Action = ChangeLogAction.TicketApprovalGateReviewed,
            Description = $"{approverName} marked approval gate \"{gate.Title}\" as {reviewLabel.ToLowerInvariant()}.",
            Timestamp = DateTime.UtcNow,
            PerformedByUserId = actor.UserId,
            PerformedByUserName = actor.UserName,
            TicketId = ticket.TicketId,
            HumanReadableId = ticket.HumanReadableId,
            ApprovalGateId = gate.ApprovalGateId,
            GateTitle = gate.Title,
            ReviewStatus = reviewLabel,
            GateState = ApprovalGateHelper.GetLabel(gate.State),
            ApproverUserId = approverUserId,
            ApproverName = approverName
        }, cancellationToken);
    }

    public async Task<List<FieldChange>> BuildTicketUpdateChangesAsync(Ticket existing, Ticket updated, CancellationToken cancellationToken = default)
    {
        var changes = new List<FieldChange>();

        AddChange(changes, "Summary", existing.Summary, updated.Summary);
        AddChange(changes, "Priority", ToPriorityLabel(existing.Priority), ToPriorityLabel(updated.Priority));

        if (!string.Equals(existing.AssigneeId ?? string.Empty, updated.AssigneeId ?? string.Empty, StringComparison.OrdinalIgnoreCase))
        {
            var oldAssignee = await ResolveUserDisplayNameAsync(existing.AssigneeId, cancellationToken);
            var newAssignee = await ResolveUserDisplayNameAsync(updated.AssigneeId, cancellationToken);
            AddChange(changes, "Assignee", oldAssignee, newAssignee);
        }

        var attributeKeys = existing.Attributes.Keys
            .Union(updated.Attributes.Keys, StringComparer.OrdinalIgnoreCase)
            .OrderBy(key => key, StringComparer.OrdinalIgnoreCase);

        foreach (var key in attributeKeys)
        {
            var oldValue = existing.Attributes.TryGetValue(key, out var existingValue)
                ? FormatValue(existingValue)
                : null;
            var newValue = updated.Attributes.TryGetValue(key, out var updatedValue)
                ? FormatValue(updatedValue)
                : null;

            AddChange(changes, HumanizeKey(key), oldValue, newValue);
        }

        return changes;
    }

    public List<FieldChange> BuildApprovalGateUpdateChanges(ApprovalGate existing, ApprovalGate updated, IReadOnlyDictionary<string, string> approverNames)
    {
        var changes = new List<FieldChange>();
        AddChange(changes, "Gate title", existing.Title, updated.Title);
        AddChange(changes, "Gate justification", existing.Justification, updated.Justification);
        AddChange(changes, "Approval policy", ToApprovalPolicyLabel(existing.AllApproversRequired), ToApprovalPolicyLabel(updated.AllApproversRequired));
        AddChange(
            changes,
            "Approvers",
            FormatApproverList(existing.Approvers.Select(x => x.UserId), approverNames),
            FormatApproverList(updated.Approvers.Select(x => x.UserId), approverNames));

        return changes;
    }

    public async Task<Dictionary<string, string>> ResolveApproverNamesAsync(IEnumerable<string> approverUserIds, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var userId in approverUserIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            result[userId] = await ResolveUserDisplayNameAsync(userId, cancellationToken);
        }

        return result;
    }

    private async Task<(string? UserId, string? UserName)> ResolveActorAsync(CancellationToken cancellationToken)
    {
        var principal = httpContextAccessor.HttpContext?.User;
        var userId = await ApprovalGateHelper.ResolveCurrentUserIdAsync(principal, userRepository, cancellationToken);
        var userName = await ResolveCurrentUserDisplayNameAsync(principal, userId, cancellationToken);
        return (userId, userName);
    }

    private async Task<string?> ResolveCurrentUserDisplayNameAsync(ClaimsPrincipal? principal, string? userId, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                return string.IsNullOrWhiteSpace(user.Name) ? user.Email : user.Name;
            }
        }

        return principal?.FindFirst("name")?.Value
            ?? principal?.FindFirst("preferred_username")?.Value
            ?? principal?.FindFirst(ClaimTypes.Email)?.Value
            ?? userId;
    }

    private async Task<List<TicketApprovalGateApproverInfo>> ResolveApproverInfosAsync(IEnumerable<string> approverUserIds, CancellationToken cancellationToken)
    {
        var approvers = new List<TicketApprovalGateApproverInfo>();
        foreach (var userId in approverUserIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            approvers.Add(new TicketApprovalGateApproverInfo
            {
                UserId = userId,
                Name = await ResolveUserDisplayNameAsync(userId, cancellationToken)
            });
        }

        return approvers;
    }

    private async Task<string> ResolveUserDisplayNameAsync(string? userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return "Unknown user";
        }

        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return "Unknown user";
        }

        if (!string.IsNullOrWhiteSpace(user.Name))
        {
            return user.Name;
        }

        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            return user.Email;
        }

        return "Unknown user";
    }

    private static void AddChange(List<FieldChange> changes, string fieldName, string? oldValue, string? newValue)
    {
        if (string.Equals(oldValue ?? string.Empty, newValue ?? string.Empty, StringComparison.Ordinal))
        {
            return;
        }

        changes.Add(new FieldChange
        {
            FieldName = fieldName,
            OldValue = NormalizeDisplayValue(oldValue),
            NewValue = NormalizeDisplayValue(newValue)
        });
    }

    private static string BuildUpdateDescription(List<FieldChange> changes)
    {
        var labels = changes.Select(change => change.FieldName).Take(3).ToList();
        return labels.Count switch
        {
            0 => "Updated ticket.",
            1 => $"Updated {labels[0].ToLowerInvariant()}.",
            2 => $"Updated {labels[0].ToLowerInvariant()} and {labels[1].ToLowerInvariant()}.",
            _ => $"Updated {labels[0].ToLowerInvariant()}, {labels[1].ToLowerInvariant()}, and {changes.Count - 2} more field(s)."
        };
    }

    private static string? NormalizeDisplayValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Empty";
        }

        return value.Trim();
    }

    private static string ToStatusLabel(string value)
    {
        var normalized = (value ?? string.Empty).Trim().ToLowerInvariant().Replace('_', '-').Replace(' ', '-');
        return normalized switch
        {
            "new" or "open" or "state-new" => "New",
            "in-progress" or "progress" or "state-in-progress" or "state-wip" or "wip" => "In progress",
            "closed" or "state-closed" => "Closed",
            _ => HumanizeKey(normalized)
        };
    }

    private static string ToPriorityLabel(string value)
    {
        return HumanizeKey((value ?? string.Empty).ToLowerInvariant());
    }

    private static string HumanizeKey(string value)
    {
        return string.Join(" ", (value ?? string.Empty)
            .Replace('_', ' ')
            .Replace('-', ' ')
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
    }

    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => "Empty",
            bool boolean => boolean ? "Yes" : "No",
            DateTime dateTime => dateTime.ToString("MMM d, yyyy HH:mm"),
            IEnumerable<object> list => string.Join(", ", list.Select(FormatValue)),
            _ => value.ToString()?.Trim() ?? "Empty"
        };
    }

    private static string ToApprovalPolicyLabel(bool allApproversRequired)
    {
        return allApproversRequired ? "All approvers required" : "Any approver can approve";
    }

    private static string FormatApproverList(IEnumerable<string> approverUserIds, IReadOnlyDictionary<string, string> approverNames)
    {
        var values = approverUserIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => approverNames.TryGetValue(id, out var name) ? name : id)
            .ToList();

        return values.Count == 0 ? "No approvers" : string.Join(", ", values);
    }
}
