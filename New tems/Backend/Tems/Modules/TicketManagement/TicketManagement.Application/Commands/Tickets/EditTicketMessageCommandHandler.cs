using MediatR;
using Microsoft.AspNetCore.Http;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Commands.Tickets;

public class EditTicketMessageCommandHandler : IRequestHandler<EditTicketMessageCommand, EditTicketMessageResponse>
{
    private readonly ITicketConversationRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public EditTicketMessageCommandHandler(
        ITicketConversationRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public async Task<EditTicketMessageResponse> Handle(EditTicketMessageCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return new EditTicketMessageResponse(false, null);

        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(_httpContextAccessor.HttpContext?.User, _userRepository, cancellationToken);
        if (currentUserIdentifiers.Count == 0)
        {
            throw new UnauthorizedAccessException("Could not determine the current user");
        }

        var conversation = await _repository.GetByTicketIdAsync(request.TicketId, cancellationToken);
        var message = conversation?.Messages.FirstOrDefault(m => string.Equals(m.MessageId, request.MessageId, StringComparison.OrdinalIgnoreCase));
        if (message == null)
            return new EditTicketMessageResponse(false, null);

        if (!ApprovalGateHelper.MatchesCurrentUser(message.SenderId, currentUserIdentifiers))
        {
            throw new UnauthorizedAccessException("You can only edit your own messages");
        }

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
