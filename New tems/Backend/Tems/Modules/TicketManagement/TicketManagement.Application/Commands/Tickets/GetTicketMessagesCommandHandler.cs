using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class GetTicketMessagesCommandHandler : IRequestHandler<GetTicketMessagesCommand, GetTicketMessagesResponse>
{
    private readonly ITicketConversationRepository _repository;

    public GetTicketMessagesCommandHandler(ITicketConversationRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetTicketMessagesResponse> Handle(GetTicketMessagesCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _repository.GetByTicketIdAsync(request.TicketId, cancellationToken);

        if (conversation == null)
            return new GetTicketMessagesResponse(new List<TicketMessageResponse>());

        var messages = conversation.Messages.Select(m => new TicketMessageResponse(
            m.SenderType,
            m.SenderId,
            m.Timestamp,
            m.Content,
            m.ChannelMessageId,
            m.IsInternalNote
        )).ToList();

        return new GetTicketMessagesResponse(messages);
    }
}
