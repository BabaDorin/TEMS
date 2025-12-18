namespace TicketManagement.Contract.Responses;

public record CreateTicketResponse(string TicketId, string HumanReadableId);

public record GetTicketResponse(
    string TicketId,
    string TenantId,
    string TicketTypeId,
    string HumanReadableId,
    string Summary,
    string CurrentStateId,
    string Priority,
    ReporterResponse Reporter,
    string? AssigneeId,
    Dictionary<string, object> Attributes,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? ResolvedAt
);

public record GetAllTicketsResponse(
    List<GetTicketResponse> Tickets
);

public record UpdateTicketResponse(bool Success);

public record DeleteTicketResponse(bool Success);

public record ReporterResponse(
    string UserId,
    string ChannelSource,
    string? ChannelThreadId
);

public record AddTicketMessageResponse(bool Success);

public record GetTicketMessagesResponse(
    List<TicketMessageResponse> Messages
);

public record TicketMessageResponse(
    string SenderType,
    string SenderId,
    DateTime Timestamp,
    string Content,
    string? ChannelMessageId,
    bool IsInternalNote
);
