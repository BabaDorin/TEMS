using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record DeleteTicketCommand(
    string TicketId
) : IRequest<DeleteTicketResponse>;
