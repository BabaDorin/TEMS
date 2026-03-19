using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record EditTicketMessageCommand(
    string TicketId,
    string MessageId,
    string Content
) : IRequest<EditTicketMessageResponse>;
