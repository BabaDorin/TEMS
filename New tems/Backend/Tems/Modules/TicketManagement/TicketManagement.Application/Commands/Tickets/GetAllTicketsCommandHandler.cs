using MediatR;
using Microsoft.AspNetCore.Http;
using Tems.Common.Tenant;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;
using TicketManagement.Application.Helpers;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Commands.Tickets;

public class GetAllTicketsCommandHandler : IRequestHandler<GetAllTicketsCommand, GetAllTicketsResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public GetAllTicketsCommandHandler(
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

    public async Task<GetAllTicketsResponse> Handle(GetAllTicketsCommand request, CancellationToken cancellationToken)
    {
        var tickets = await _repository.GetAllAsync(_tenantContext.TenantId, cancellationToken);
        var currentUser = _httpContextAccessor.HttpContext?.User;
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(currentUser, _userRepository, cancellationToken);
        var isManager = ApprovalGateHelper.IsManager(currentUser);

        if (!isManager)
        {
            tickets = tickets
                .Where(ticket => ApprovalGateHelper.CanViewTicket(ticket, currentUserIdentifiers, isManager))
                .ToList();
        }

        var response = new List<GetTicketResponse>(tickets.Count);
        foreach (var ticket in tickets)
        {
            if (ApprovalGateHelper.EnsureApprovalGateIds(ticket))
            {
                await _repository.UpdateAsync(ticket, cancellationToken);
            }

            response.Add(await TicketResponseFactory.ToResponseAsync(ticket, _userRepository, cancellationToken));
        }

        return new GetAllTicketsResponse(response);
    }
}
