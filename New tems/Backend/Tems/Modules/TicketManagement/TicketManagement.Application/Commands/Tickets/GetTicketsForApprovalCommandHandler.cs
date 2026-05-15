using Microsoft.AspNetCore.Http;
using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;
using AssetManagement.Application.Interfaces;

namespace TicketManagement.Application.Commands.Tickets;

public class GetTicketsForApprovalCommandHandler : IRequestHandler<GetTicketsForApprovalCommand, GetAllTicketsResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IAssetRepository _assetRepository;

    public GetTicketsForApprovalCommandHandler(
        ITicketRepository repository,
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        IAssetRepository assetRepository)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _assetRepository = assetRepository;
    }

    public async Task<GetAllTicketsResponse> Handle(GetTicketsForApprovalCommand request, CancellationToken cancellationToken)
    {
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(_httpContextAccessor.HttpContext?.User, _userRepository, cancellationToken);
        if (currentUserIdentifiers.Count == 0)
        {
            return new GetAllTicketsResponse(new List<GetTicketResponse>());
        }

        var tickets = await _repository.GetAllAsync(_tenantContext.TenantId, cancellationToken);

        var approvedTickets = tickets
            .Where(ticket => !IsTerminalState(ticket.CurrentStateId))
            .Where(ticket => ticket.ApprovalGates.Any(gate =>
                gate.Approvers.Any(approver => ApprovalGateHelper.MatchesCurrentUser(approver.UserId, currentUserIdentifiers))))
            .ToList();

        var response = new List<GetTicketResponse>(approvedTickets.Count);
        foreach (var ticket in approvedTickets)
        {
            if (ApprovalGateHelper.EnsureApprovalGateIds(ticket))
            {
                await _repository.UpdateAsync(ticket, cancellationToken);
            }

            response.Add(await TicketResponseFactory.ToResponseAsync(ticket, _assetRepository, _userRepository, cancellationToken));
        }

        return new GetAllTicketsResponse(response);
    }

    private static bool IsTerminalState(string? stateId)
    {
        var normalized = (stateId ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Replace("_", "-")
            .Replace(" ", "-");

        return normalized is "closed" or "state-closed" or "approved" or "state-approved";
    }
}
