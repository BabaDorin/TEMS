using MediatR;
using Tems.Common.Tenant;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Services;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Commands.Tickets;

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, UpdateTicketResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly TicketHistoryLogService _ticketHistoryLogService;

    public UpdateTicketCommandHandler(
        ITicketRepository repository,
        ITicketTypeRepository ticketTypeRepository,
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        TicketHistoryLogService ticketHistoryLogService)
    {
        _repository = repository;
        _ticketTypeRepository = ticketTypeRepository;
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _ticketHistoryLogService = ticketHistoryLogService;
    }

    public async Task<UpdateTicketResponse> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);
        if (existing == null)
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");

        var ticketType = await _ticketTypeRepository.GetByIdAsync(existing.TicketTypeId, _tenantContext.TenantId, cancellationToken);
        if (ticketType == null)
        {
            throw new KeyNotFoundException($"TicketType with ID {existing.TicketTypeId} not found");
        }

        var currentUser = _httpContextAccessor.HttpContext?.User;
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(currentUser, _userRepository, cancellationToken);
        var isManager = ApprovalGateHelper.IsManager(currentUser);
        var isAuthor = ApprovalGateHelper.MatchesCurrentUser(existing.Reporter.UserId, currentUserIdentifiers);

        var workflowConfig = TicketStateHelper.NormalizeWorkflowConfig(ticketType.WorkflowConfig);
        var resolvedStateId = TicketStateHelper.ResolveManagedStatusId(workflowConfig.States, request.CurrentStateId);
        if (resolvedStateId == null)
        {
            throw new InvalidOperationException($"Status '{request.CurrentStateId}' is not allowed for this ticket type");
        }

        var original = CloneTicket(existing);
        var previousStatus = existing.CurrentStateId;

        if (!string.Equals(existing.Summary, request.Summary, StringComparison.Ordinal))
        {
            if (!isAuthor)
            {
                throw new UnauthorizedAccessException("Only the ticket author can update the summary");
            }

            existing.Summary = request.Summary;
        }

        if (!string.Equals(existing.CurrentStateId, resolvedStateId, StringComparison.OrdinalIgnoreCase))
        {
            if (!isManager)
            {
                throw new UnauthorizedAccessException("Only users with can_manage_tickets can change the ticket status");
            }

            existing.CurrentStateId = resolvedStateId;
        }

        var existingAttributesJson = JsonSerializer.Serialize(existing.Attributes);
        var requestAttributesJson = JsonSerializer.Serialize(request.Attributes);

        if (!isManager && (!string.Equals(existing.Priority, request.Priority, StringComparison.OrdinalIgnoreCase) ||
                           !string.Equals(existing.AssigneeId ?? string.Empty, request.AssigneeId ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                           !string.Equals(existingAttributesJson, requestAttributesJson, StringComparison.Ordinal)))
        {
            throw new UnauthorizedAccessException("Only users with can_manage_tickets can update ticket metadata");
        }

        existing.Priority = request.Priority.ToUpper();
        existing.AssigneeId = request.AssigneeId;
        existing.Attributes = request.Attributes;
        existing.UpdatedAt = DateTime.UtcNow;

        var success = await _repository.UpdateAsync(existing, cancellationToken);
        if (success)
        {
            var updateChanges = await _ticketHistoryLogService.BuildTicketUpdateChangesAsync(original, existing, cancellationToken);
            if (updateChanges.Count > 0)
            {
                await _ticketHistoryLogService.LogUpdatedAsync(existing, updateChanges, cancellationToken: cancellationToken);
            }

            if (!string.Equals(previousStatus, existing.CurrentStateId, StringComparison.OrdinalIgnoreCase))
            {
                await _ticketHistoryLogService.LogStatusChangedAsync(existing, previousStatus, existing.CurrentStateId, cancellationToken);
            }
        }

        return new UpdateTicketResponse(success);
    }

    private static Domain.Ticket CloneTicket(Domain.Ticket ticket)
    {
        return new Domain.Ticket
        {
            TicketId = ticket.TicketId,
            TenantId = ticket.TenantId,
            TicketTypeId = ticket.TicketTypeId,
            HumanReadableId = ticket.HumanReadableId,
            Title = ticket.Title,
            Summary = ticket.Summary,
            AiSummary = ticket.AiSummary,
            CurrentStateId = ticket.CurrentStateId,
            Priority = ticket.Priority,
            Reporter = new Domain.Reporter
            {
                UserId = ticket.Reporter.UserId,
                ChannelSource = ticket.Reporter.ChannelSource,
                ChannelThreadId = ticket.Reporter.ChannelThreadId
            },
            AssigneeId = ticket.AssigneeId,
            Attributes = new Dictionary<string, object>(ticket.Attributes),
            ApprovalGates = ticket.ApprovalGates
                .Select(gate => new Domain.ApprovalGate
                {
                    ApprovalGateId = gate.ApprovalGateId,
                    Title = gate.Title,
                    Justification = gate.Justification,
                    State = gate.State,
                    AllApproversRequired = gate.AllApproversRequired,
                    Approvers = gate.Approvers.Select(approver => new Domain.ApprovalGateApprover
                    {
                        UserId = approver.UserId,
                        Status = approver.Status,
                        ReviewedAt = approver.ReviewedAt
                    }).ToList(),
                    CreatedAt = gate.CreatedAt,
                    UpdatedAt = gate.UpdatedAt
                }).ToList(),
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            ResolvedAt = ticket.ResolvedAt
        };
    }
}
