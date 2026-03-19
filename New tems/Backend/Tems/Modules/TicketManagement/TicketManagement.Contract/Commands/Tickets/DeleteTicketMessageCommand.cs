using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record DeleteTicketMessageCommand(
    string TicketId,
    string MessageId
) : IRequest<DeleteTicketMessageResponse>;
