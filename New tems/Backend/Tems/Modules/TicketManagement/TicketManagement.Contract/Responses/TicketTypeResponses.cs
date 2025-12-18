namespace TicketManagement.Contract.Responses;

public record CreateTicketTypeResponse(string TicketTypeId);

public record GetTicketTypeResponse(
    string TicketTypeId,
    string TenantId,
    string Name,
    string Description,
    string ItilCategory,
    int Version,
    WorkflowConfigResponse WorkflowConfig,
    List<AttributeDefinitionResponse> AttributeDefinitions,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record GetAllTicketTypesResponse(
    List<GetTicketTypeResponse> TicketTypes
);

public record UpdateTicketTypeResponse(bool Success);

public record DeleteTicketTypeResponse(bool Success);

public record WorkflowConfigResponse(
    List<WorkflowStateResponse> States,
    string InitialStateId
);

public record WorkflowStateResponse(
    string Id,
    string Label,
    string Type,
    List<string> AllowedTransitions,
    string? AutomationHook
);

public record AttributeDefinitionResponse(
    string Key,
    string Label,
    string DataType,
    bool IsRequired,
    bool IsPredefined,
    List<string>? Options,
    string? AiExtractionHint,
    string? ValidationRule
);
