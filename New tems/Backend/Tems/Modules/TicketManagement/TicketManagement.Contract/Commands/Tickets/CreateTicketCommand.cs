using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record CreateTicketCommand(
    string TicketTypeId,
    string Summary,
    string Priority,
    ReporterDto Reporter,
    string? AssigneeId,
    Dictionary<string, object> Attributes
) : IRequest<CreateTicketResponse>;

public record ReporterDto(
    string UserId,
    string ChannelSource,
    string? ChannelThreadId
);
