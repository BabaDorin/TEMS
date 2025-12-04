using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record GetTicketByIdCommand(
    string TicketId,
    string TenantId
) : IRequest<GetTicketResponse>;
