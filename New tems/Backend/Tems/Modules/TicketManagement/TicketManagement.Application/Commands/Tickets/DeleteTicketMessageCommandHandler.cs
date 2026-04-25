using MediatR;
using Microsoft.AspNetCore.Http;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Commands.Tickets;

public class DeleteTicketMessageCommandHandler : IRequestHandler<DeleteTicketMessageCommand, DeleteTicketMessageResponse>
{
    private readonly ITicketConversationRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public DeleteTicketMessageCommandHandler(
        ITicketConversationRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public async Task<DeleteTicketMessageResponse> Handle(DeleteTicketMessageCommand request, CancellationToken cancellationToken)
    {
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(_httpContextAccessor.HttpContext?.User, _userRepository, cancellationToken);
        if (currentUserIdentifiers.Count == 0)
        {
            throw new UnauthorizedAccessException("Could not determine the current user");
        }

        var conversation = await _repository.GetByTicketIdAsync(request.TicketId, cancellationToken);
        var message = conversation?.Messages.FirstOrDefault(m => string.Equals(m.MessageId, request.MessageId, StringComparison.OrdinalIgnoreCase));
        if (message == null)
        {
            return new DeleteTicketMessageResponse(false);
        }

        if (!ApprovalGateHelper.MatchesCurrentUser(message.SenderId, currentUserIdentifiers))
        {
            throw new UnauthorizedAccessException("You can only delete your own messages");
        }

        var success = await _repository.DeleteMessageAsync(request.TicketId, request.MessageId, cancellationToken);
        return new DeleteTicketMessageResponse(success);
    }
}
