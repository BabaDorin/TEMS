using MediatR;
using Microsoft.AspNetCore.Http;
using Tems.Common.Tenant;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Commands.Tickets;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, DeleteTicketResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITicketConversationRepository _conversationRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public DeleteTicketCommandHandler(
        ITicketRepository repository,
        ITicketConversationRepository conversationRepository,
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository)
    {
        _repository = repository;
        _conversationRepository = conversationRepository;
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public async Task<DeleteTicketResponse> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");
        }

        var currentUser = _httpContextAccessor.HttpContext?.User;
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(currentUser, _userRepository, cancellationToken);
        var isManager = ApprovalGateHelper.IsManager(currentUser);

        if (!isManager &&
            !ApprovalGateHelper.MatchesCurrentUser(ticket.Reporter.UserId, currentUserIdentifiers) &&
            !ApprovalGateHelper.MatchesCurrentUser(ticket.AccountableUserId, currentUserIdentifiers))
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this ticket");
        }

        var success = await _repository.DeleteAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);
        if (success)
            await _conversationRepository.DeleteByTicketIdAsync(request.TicketId, cancellationToken);

        return new DeleteTicketResponse(success);
    }
}
