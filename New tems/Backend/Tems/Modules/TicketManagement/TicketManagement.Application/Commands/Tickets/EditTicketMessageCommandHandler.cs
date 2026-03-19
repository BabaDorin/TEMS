using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class EditTicketMessageCommandHandler : IRequestHandler<EditTicketMessageCommand, EditTicketMessageResponse>
{
    private readonly ITicketConversationRepository _repository;

    public EditTicketMessageCommandHandler(ITicketConversationRepository repository)
    {
        _repository = repository;
    }

    public async Task<EditTicketMessageResponse> Handle(EditTicketMessageCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return new EditTicketMessageResponse(false, null);

        var updatedMessage = await _repository.EditMessageAsync(
            request.TicketId,
            request.MessageId,
            request.Content.Trim(),
            cancellationToken);

        if (updatedMessage == null)
            return new EditTicketMessageResponse(false, null);

        var response = new TicketMessageResponse(
            updatedMessage.MessageId,
            updatedMessage.SenderType,
            updatedMessage.SenderId,
            updatedMessage.Timestamp,
            updatedMessage.Content,
            updatedMessage.ChannelMessageId,
            updatedMessage.IsInternalNote,
            updatedMessage.EditedAt
        );

        return new EditTicketMessageResponse(true, response);
    }
}
