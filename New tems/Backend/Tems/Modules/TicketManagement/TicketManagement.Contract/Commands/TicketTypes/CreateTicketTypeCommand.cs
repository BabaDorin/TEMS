using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.TicketTypes;

public record CreateTicketTypeCommand(
    string Name,
    string Description,
    string ItilCategory,
    int Version,
    WorkflowConfigDto? WorkflowConfig,
    List<AttributeDefinitionDto>? AttributeDefinitions
) : IRequest<CreateTicketTypeResponse>;

public record WorkflowConfigDto(
    List<WorkflowStateDto> States,
    string InitialStateId
);

public record WorkflowStateDto(
    string Id,
    string Label,
    string Type,
    List<string> AllowedTransitions,
    string? AutomationHook
);

public record AttributeDefinitionDto(
    string Key,
    string Label,
    string DataType,
    bool IsRequired,
    bool IsPredefined,
    List<string>? Options,
    string? AiExtractionHint,
    string? ValidationRule
);
