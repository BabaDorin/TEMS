using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record UpdateTicketCommand(
    string TicketId,
    string TenantId,
    string Summary,
    string CurrentStateId,
    string Priority,
    string? AssigneeId,
    Dictionary<string, object> Attributes
) : IRequest<UpdateTicketResponse>;
