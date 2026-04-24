namespace TicketManagement.Application.Models;

public sealed record TicketAiSummaryWorkItem(
    string TicketId,
    string TenantId,
    string TicketTypeId,
    string TicketTypeName,
    string TicketDescription,
    string Priority,
    string CurrentStateId,
    IReadOnlyDictionary<string, object> Attributes
);
