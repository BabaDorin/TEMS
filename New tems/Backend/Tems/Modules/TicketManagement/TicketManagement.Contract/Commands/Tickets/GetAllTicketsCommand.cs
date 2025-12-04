using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record GetAllTicketsCommand(
    string TenantId
) : IRequest<GetAllTicketsResponse>;
