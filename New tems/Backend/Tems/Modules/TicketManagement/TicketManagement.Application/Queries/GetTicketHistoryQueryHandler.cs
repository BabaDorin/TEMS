using ChangeLog.Application.Interfaces;
using ChangeLog.Contract.DTOs;
using ChangeLog.Contract.Enums;
using ChangeLog.Contract.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Tems.Common.Tenant;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Queries;

public class GetTicketHistoryQueryHandler(
    ITicketRepository ticketRepository,
    IChangeLogRepository changeLogRepository,
    ITenantContext tenantContext,
    IHttpContextAccessor httpContextAccessor,
    IUserRepository userRepository) : IRequestHandler<GetTicketHistoryQuery, GetEntityTimelineResponse>
{
    public async Task<GetEntityTimelineResponse> Handle(GetTicketHistoryQuery request, CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(request.TicketId, tenantContext.TenantId, cancellationToken);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");
        }

        var currentUser = httpContextAccessor.HttpContext?.User;
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(currentUser, userRepository, cancellationToken);
        var isManager = ApprovalGateHelper.IsManager(currentUser);
        if (!ApprovalGateHelper.CanViewTicket(ticket, currentUserIdentifiers, isManager))
        {
            throw new UnauthorizedAccessException("You do not have access to this ticket");
        }

        var (entries, totalCount) = await changeLogRepository.GetByEntityAsync(
            ChangeLogEntityType.Ticket,
            request.TicketId,
            tenantContext.TenantId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return new GetEntityTimelineResponse(
            entries.Select(entry => new ChangeLogEntryDto(
                entry.Id,
                entry.Action,
                entry.Description,
                entry.Timestamp,
                entry.PerformedByUserId,
                entry.PerformedByUserName,
                entry.GetReferences(),
                entry.GetDetails())).ToList(),
            totalCount,
            request.PageNumber,
            request.PageSize);
    }
}
