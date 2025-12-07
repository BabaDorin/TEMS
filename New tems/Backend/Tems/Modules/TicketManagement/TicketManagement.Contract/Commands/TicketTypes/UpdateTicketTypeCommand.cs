using MediatR;
using TicketManagement.Contract.Responses;
using static TicketManagement.Contract.Commands.TicketTypes.CreateTicketTypeCommand;

namespace TicketManagement.Contract.Commands.TicketTypes;

public record UpdateTicketTypeCommand(
    string TicketTypeId,
    string Name,
    string Description,
    string ItilCategory,
    int Version,
    WorkflowConfigDto? WorkflowConfig,
    List<AttributeDefinitionDto>? AttributeDefinitions
) : IRequest<UpdateTicketTypeResponse>;
