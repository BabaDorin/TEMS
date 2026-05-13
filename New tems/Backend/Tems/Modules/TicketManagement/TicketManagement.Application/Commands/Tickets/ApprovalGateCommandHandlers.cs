using Microsoft.AspNetCore.Http;
using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Domain;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Services;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Commands.Tickets;

public class CreateApprovalGateCommandHandler : IRequestHandler<CreateApprovalGateCommand, CreateApprovalGateResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly TicketHistoryLogService _ticketHistoryLogService;

    public CreateApprovalGateCommandHandler(
        ITicketRepository repository,
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        TicketHistoryLogService ticketHistoryLogService)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _ticketHistoryLogService = ticketHistoryLogService;
    }

    public async Task<CreateApprovalGateResponse> Handle(CreateApprovalGateCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");
        }

        var currentUser = _httpContextAccessor.HttpContext?.User;
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(currentUser, _userRepository, cancellationToken);
        var isManager = ApprovalGateHelper.IsManager(currentUser);

        if (!ApprovalGateHelper.CanConfigureApprovalGates(ticket, currentUserIdentifiers, isManager))
        {
            throw new UnauthorizedAccessException("You cannot configure approval gates for this ticket");
        }

        var gate = BuildGate(request.Title, request.Justification, request.AllApproversRequired, request.ApproverUserIds);
        ticket.ApprovalGates.Add(gate);
        ticket.UpdatedAt = DateTime.UtcNow;

        var success = await _repository.UpdateAsync(ticket, cancellationToken);
        if (success)
        {
            await _ticketHistoryLogService.LogApprovalGateAddedAsync(ticket, gate, cancellationToken);
        }
        return new CreateApprovalGateResponse(success, gate.ToResponse());
    }

    private static ApprovalGate BuildGate(string title, string justification, bool allApproversRequired, IEnumerable<string> approverUserIds)
    {
        var now = DateTime.UtcNow;
        var uniqueApprovers = approverUserIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(id => new ApprovalGateApprover
            {
                UserId = id,
                Status = "pending",
                ReviewedAt = null
            })
            .ToList();

        return new ApprovalGate
        {
            ApprovalGateId = Guid.NewGuid().ToString(),
            Title = title.Trim(),
            Justification = justification.Trim(),
            AllApproversRequired = allApproversRequired,
            Approvers = uniqueApprovers,
            State = "pending",
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}

public class UpdateApprovalGateCommandHandler : IRequestHandler<UpdateApprovalGateCommand, UpdateApprovalGateResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly TicketHistoryLogService _ticketHistoryLogService;

    public UpdateApprovalGateCommandHandler(
        ITicketRepository repository,
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        TicketHistoryLogService ticketHistoryLogService)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _ticketHistoryLogService = ticketHistoryLogService;
    }

    public async Task<UpdateApprovalGateResponse> Handle(UpdateApprovalGateCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");
        }

        var currentUser = _httpContextAccessor.HttpContext?.User;
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(currentUser, _userRepository, cancellationToken);
        var isManager = ApprovalGateHelper.IsManager(currentUser);

        if (!ApprovalGateHelper.CanConfigureApprovalGates(ticket, currentUserIdentifiers, isManager))
        {
            throw new UnauthorizedAccessException("You cannot configure approval gates for this ticket");
        }

        var gate = ticket.ApprovalGates.FirstOrDefault(x => string.Equals(x.ApprovalGateId, request.ApprovalGateId, StringComparison.OrdinalIgnoreCase));
        if (gate == null)
        {
            throw new KeyNotFoundException($"Approval gate with ID {request.ApprovalGateId} not found");
        }

        if (string.IsNullOrWhiteSpace(gate.ApprovalGateId))
        {
            gate.ApprovalGateId = Guid.NewGuid().ToString();
        }

        var previousGate = ApprovalGateCommandExtensions.CloneGate(gate);
        var now = DateTime.UtcNow;
        var previousStatuses = gate.Approvers.ToDictionary(x => x.UserId, x => x, StringComparer.OrdinalIgnoreCase);
        var nextApprovers = request.ApproverUserIdsNormalized()
            .Select(userId =>
            {
                if (previousStatuses.TryGetValue(userId, out var existing))
                {
                    return new ApprovalGateApprover
                    {
                        UserId = existing.UserId,
                        Status = existing.Status,
                        ReviewedAt = existing.ReviewedAt
                    };
                }

                return new ApprovalGateApprover
                {
                    UserId = userId,
                    Status = "pending",
                    ReviewedAt = null
                };
            })
            .ToList();

        gate.Title = request.Title.Trim();
        gate.Justification = request.Justification.Trim();
        gate.AllApproversRequired = request.AllApproversRequired;
        gate.Approvers = nextApprovers;
        gate.State = ApprovalGateHelper.ResolveGateState(gate);
        gate.UpdatedAt = now;
        ticket.UpdatedAt = now;

        var success = await _repository.UpdateAsync(ticket, cancellationToken);
        if (success)
        {
            var approverNames = await _ticketHistoryLogService.ResolveApproverNamesAsync(
                previousGate.Approvers.Select(x => x.UserId).Union(gate.Approvers.Select(x => x.UserId), StringComparer.OrdinalIgnoreCase),
                cancellationToken);
            var changes = _ticketHistoryLogService.BuildApprovalGateUpdateChanges(previousGate, gate, approverNames);
            if (changes.Count > 0)
            {
                await _ticketHistoryLogService.LogUpdatedAsync(
                    ticket,
                    changes,
                    $"Updated approval gate \"{gate.Title}\".",
                    cancellationToken);
            }
        }
        return new UpdateApprovalGateResponse(success, gate.ToResponse());
    }
}

public class ReviewApprovalGateCommandHandler : IRequestHandler<ReviewApprovalGateCommand, ReviewApprovalGateResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly TicketHistoryLogService _ticketHistoryLogService;

    public ReviewApprovalGateCommandHandler(
        ITicketRepository repository,
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        TicketHistoryLogService ticketHistoryLogService)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _ticketHistoryLogService = ticketHistoryLogService;
    }

    public async Task<ReviewApprovalGateResponse> Handle(ReviewApprovalGateCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");
        }

        var gate = ticket.ApprovalGates.FirstOrDefault(x => string.Equals(x.ApprovalGateId, request.ApprovalGateId, StringComparison.OrdinalIgnoreCase));
        if (gate == null)
        {
            throw new KeyNotFoundException($"Approval gate with ID {request.ApprovalGateId} not found");
        }

        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(_httpContextAccessor.HttpContext?.User, _userRepository, cancellationToken);
        if (currentUserIdentifiers.Count == 0)
        {
            throw new UnauthorizedAccessException("Could not determine the current user");
        }

        if (!ApprovalGateHelper.CanReviewGate(gate, currentUserIdentifiers))
        {
            throw new UnauthorizedAccessException("You are not an approver for this gate");
        }

        var approver = gate.Approvers.First(x => ApprovalGateHelper.MatchesCurrentUser(x.UserId, currentUserIdentifiers));

        var normalizedStatus = ApprovalGateHelper.NormalizeState(request.Status);
        if (normalizedStatus is not "approved" and not "rejected")
        {
            throw new InvalidOperationException("Approval status must be approved or rejected");
        }

        approver.Status = normalizedStatus == "approved" ? "approved" : "rejected";
        approver.ReviewedAt = DateTime.UtcNow;
        gate.State = ApprovalGateHelper.ResolveGateState(gate);
        gate.UpdatedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;

        var success = await _repository.UpdateAsync(ticket, cancellationToken);
        if (success)
        {
            await _ticketHistoryLogService.LogApprovalGateReviewedAsync(ticket, gate, approver.UserId, approver.Status, cancellationToken);
        }
        return new ReviewApprovalGateResponse(success, gate.ToResponse());
    }
}

public class DeleteApprovalGateCommandHandler : IRequestHandler<DeleteApprovalGateCommand, UpdateApprovalGateResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly TicketHistoryLogService _ticketHistoryLogService;

    public DeleteApprovalGateCommandHandler(
        ITicketRepository repository,
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        TicketHistoryLogService ticketHistoryLogService)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _ticketHistoryLogService = ticketHistoryLogService;
    }

    public async Task<UpdateApprovalGateResponse> Handle(DeleteApprovalGateCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);
        if (ticket == null)
        {
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");
        }

        var currentUser = _httpContextAccessor.HttpContext?.User;
        var currentUserIdentifiers = await ApprovalGateHelper.ResolveCurrentUserIdentifiersAsync(currentUser, _userRepository, cancellationToken);
        var isManager = ApprovalGateHelper.IsManager(currentUser);

        if (!ApprovalGateHelper.CanConfigureApprovalGates(ticket, currentUserIdentifiers, isManager))
        {
            throw new UnauthorizedAccessException("You cannot configure approval gates for this ticket");
        }

        var gate = ticket.ApprovalGates.FirstOrDefault(x => string.Equals(x.ApprovalGateId, request.ApprovalGateId, StringComparison.OrdinalIgnoreCase));
        if (gate == null)
        {
            throw new KeyNotFoundException($"Approval gate with ID {request.ApprovalGateId} not found");
        }

        var removedGate = ApprovalGateCommandExtensions.CloneGate(gate);
        ticket.ApprovalGates.Remove(gate);
        ticket.UpdatedAt = DateTime.UtcNow;

        var success = await _repository.UpdateAsync(ticket, cancellationToken);
        if (success)
        {
            await _ticketHistoryLogService.LogApprovalGateRemovedAsync(ticket, removedGate, cancellationToken);
        }

        return new UpdateApprovalGateResponse(success, null);
    }
}

internal static class ApprovalGateCommandExtensions
{
    public static List<string> ApproverUserIdsNormalized(this UpdateApprovalGateCommand request)
    {
        return request.ApproverUserIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public static ApprovalGate CloneGate(ApprovalGate gate)
    {
        return new ApprovalGate
        {
            ApprovalGateId = gate.ApprovalGateId,
            Title = gate.Title,
            Justification = gate.Justification,
            State = gate.State,
            AllApproversRequired = gate.AllApproversRequired,
            Approvers = gate.Approvers.Select(approver => new ApprovalGateApprover
            {
                UserId = approver.UserId,
                Status = approver.Status,
                ReviewedAt = approver.ReviewedAt
            }).ToList(),
            CreatedAt = gate.CreatedAt,
            UpdatedAt = gate.UpdatedAt
        };
    }
}
