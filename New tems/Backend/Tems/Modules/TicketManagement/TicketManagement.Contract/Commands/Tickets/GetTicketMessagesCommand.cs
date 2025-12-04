using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record GetTicketMessagesCommand(
    string TicketId
) : IRequest<GetTicketMessagesResponse>;
