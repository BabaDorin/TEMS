namespace TicketManagement.Contract.Responses;

public record CreateTicketResponse(string TicketId, string HumanReadableId);

public record GetTicketResponse(
    string TicketId,
    string TenantId,
    string TicketTypeId,
    string HumanReadableId,
    string Title,
    string Summary,
    string? AiSummary,
    string CurrentStateId,
    string Priority,
    ReporterResponse Reporter,
    string AccountableUserId,
    string? AccountableDisplayName,
    string? AssigneeId,
    Dictionary<string, object> Attributes,
    List<string> AssetIds,
    List<AssetLinkResponse> LinkedAssets,
    List<ApprovalGateResponse> ApprovalGates,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? ResolvedAt
);

public record AssetLinkResponse(
    string AssetId,
    string AssetTag
);

public record GetAllTicketsResponse(
    List<GetTicketResponse> Tickets
);

public record UpdateTicketResponse(bool Success);

public record DeleteTicketResponse(bool Success);

public record ReporterResponse(
    string UserId,
    string ChannelSource,
    string? ChannelThreadId,
    string? DisplayName = null
);

public record ApprovalGateResponse(
    string ApprovalGateId,
    string Title,
    string Justification,
    string State,
    bool AllApproversRequired,
    List<ApprovalGateApproverResponse> Approvers,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ApprovalGateApproverResponse(
    string UserId,
    string Status,
    DateTime? ReviewedAt
);

public record CreateApprovalGateResponse(bool Success, ApprovalGateResponse? Gate);
public record UpdateApprovalGateResponse(bool Success, ApprovalGateResponse? Gate);
public record ReviewApprovalGateResponse(bool Success, ApprovalGateResponse? Gate);

public record AddTicketMessageResponse(
    bool Success,
    TicketMessageResponse? Message
);

public record EditTicketMessageResponse(
    bool Success,
    TicketMessageResponse? Message
);

public record DeleteTicketMessageResponse(bool Success);

public record GetTicketMessagesResponse(
    List<TicketMessageResponse> Messages
);

public record TicketMessageResponse(
    string MessageId,
    string SenderType,
    string SenderId,
    DateTime Timestamp,
    string Content,
    string? ChannelMessageId,
    bool IsInternalNote,
    DateTime? EditedAt
);
