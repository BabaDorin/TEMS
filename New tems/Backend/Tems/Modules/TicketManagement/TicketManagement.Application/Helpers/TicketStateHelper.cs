using TicketManagement.Application.Domain;

namespace TicketManagement.Application.Helpers;

public static class TicketStateHelper
{
    private static readonly HashSet<string> NewStateIds = new(StringComparer.OrdinalIgnoreCase)
    {
        "new",
        "open",
        "state-new"
    };

    private static readonly HashSet<string> InProgressStateIds = new(StringComparer.OrdinalIgnoreCase)
    {
        "in-progress",
        "state-in-progress",
        "state-wip",
        "wip",
        "progress"
    };

    private static readonly HashSet<string> ClosedStateIds = new(StringComparer.OrdinalIgnoreCase)
    {
        "closed",
        "state-closed"
    };

    public static List<WorkflowState> CreateDefaultWorkflowStates()
    {
        return new List<WorkflowState>
        {
            new() { Id = "new", Label = "New", Type = "OPEN", AllowedTransitions = new List<string> { "in-progress", "closed" } },
            new() { Id = "in-progress", Label = "In Progress", Type = "ACTIVE", AllowedTransitions = new List<string> { "new", "closed" } },
            new() { Id = "closed", Label = "Closed", Type = "CLOSED", AllowedTransitions = new List<string>() }
        };
    }

    public static bool IsManagedTicketStatus(string? stateId)
    {
        return GetManagedStatusGroup(stateId) != null;
    }

    public static string GetManagedStatusLabel(string? stateId)
    {
        return GetManagedStatusGroup(stateId) switch
        {
            "new" => "New",
            "in-progress" => "In progress",
            "closed" => "Closed",
            _ => Humanize(stateId)
        };
    }

    public static bool IsManagedStatusForWorkflow(IEnumerable<WorkflowState> workflowStates, string? stateId)
    {
        return ResolveManagedStatusId(workflowStates, stateId) != null;
    }

    public static string? ResolveManagedStatusId(IEnumerable<WorkflowState> workflowStates, string? stateId)
    {
        if (string.IsNullOrWhiteSpace(stateId))
        {
            return null;
        }

        var directMatch = workflowStates.FirstOrDefault(state =>
            string.Equals(state.Id, stateId, StringComparison.OrdinalIgnoreCase));
        if (directMatch != null)
        {
            return directMatch.Id;
        }

        var targetGroup = GetManagedStatusGroup(stateId);
        if (targetGroup == null)
        {
            return null;
        }

        var groupMatch = workflowStates.FirstOrDefault(state =>
            string.Equals(GetManagedStatusGroup(state.Id), targetGroup, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(GetManagedStatusGroup(state.Label), targetGroup, StringComparison.OrdinalIgnoreCase));

        return groupMatch?.Id;
    }

    private static string? GetManagedStatusGroup(string? stateId)
    {
        if (string.IsNullOrWhiteSpace(stateId))
        {
            return null;
        }

        var normalized = NormalizeStateId(stateId);

        if (NewStateIds.Contains(normalized))
        {
            return "new";
        }

        if (InProgressStateIds.Contains(normalized))
        {
            return "in-progress";
        }

        if (ClosedStateIds.Contains(normalized))
        {
            return "closed";
        }

        return null;
    }

    private static string NormalizeStateId(string stateId)
    {
        return (stateId ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Replace('_', '-')
            .Replace(' ', '-');
    }

    private static string Humanize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var text = value.Replace('_', ' ').Replace('-', ' ').Trim();
        if (text.Length == 0)
        {
            return string.Empty;
        }

        return string.Join(' ', text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => word.Length == 1
                ? word.ToUpperInvariant()
                : char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
    }
}
