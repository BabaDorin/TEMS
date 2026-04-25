using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;
using Microsoft.AspNetCore.Http;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Commands.Tickets;

public class GetTicketByIdCommandHandler : IRequestHandler<GetTicketByIdCommand, GetTicketResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public GetTicketByIdCommandHandler(
        ITicketRepository repository,
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public async Task<GetTicketResponse> Handle(GetTicketByIdCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);

        if (ticket == null)
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");

        var currentUser = _httpContextAccessor.HttpContext?.User;
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(currentUser, _userRepository, cancellationToken);
        var isManager = ApprovalGateHelper.IsManager(currentUser);

        if (!ApprovalGateHelper.CanViewTicket(ticket, currentUserIdentifiers, isManager))
        {
            throw new UnauthorizedAccessException("You do not have access to this ticket");
        }

        if (ApprovalGateHelper.EnsureApprovalGateIds(ticket))
        {
            await _repository.UpdateAsync(ticket, cancellationToken);
        }

        return await TicketResponseFactory.ToResponseAsync(ticket, _userRepository, cancellationToken);
    }
}
