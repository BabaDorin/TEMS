using Microsoft.AspNetCore.Http;
using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class GetTicketMessagesCommandHandler : IRequestHandler<GetTicketMessagesCommand, GetTicketMessagesResponse>
{
    private readonly ITicketConversationRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetTicketMessagesCommandHandler(
        ITicketConversationRepository repository,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GetTicketMessagesResponse> Handle(GetTicketMessagesCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _repository.GetByTicketIdAsync(request.TicketId, cancellationToken);

        if (conversation == null)
            return new GetTicketMessagesResponse(new List<TicketMessageResponse>());

        var canSeeInternalNotes = _httpContextAccessor.HttpContext?.User?.IsInRole("can_manage_tickets") == true;
        var messages = canSeeInternalNotes
            ? conversation.Messages.ToList()
            : conversation.Messages.Where(m => !m.IsInternalNote).ToList();

        var responseMessages = messages.Select(m => new TicketMessageResponse(
            m.MessageId,
            m.SenderType,
            m.SenderId,
            m.Timestamp,
            m.Content,
            m.ChannelMessageId,
            m.IsInternalNote,
            m.EditedAt
        )).ToList();

        return new GetTicketMessagesResponse(responseMessages);
    }
}
