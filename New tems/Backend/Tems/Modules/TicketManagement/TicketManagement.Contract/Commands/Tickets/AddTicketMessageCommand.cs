using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record AddTicketMessageCommand(
    string TicketId,
    string SenderType,
    string SenderId,
    string Content,
    string? ChannelMessageId,
    bool IsInternalNote
) : IRequest<AddTicketMessageResponse>;
