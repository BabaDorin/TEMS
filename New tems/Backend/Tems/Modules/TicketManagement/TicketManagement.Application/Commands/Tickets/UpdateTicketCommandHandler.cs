using AssetManagement.Application.Interfaces;
using MediatR;
using Tems.Common.Tenant;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Tems.Common.Notifications;
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
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IPublisher _publisher;

    public UpdateTicketCommandHandler(
        ITicketRepository repository,
        ITicketTypeRepository ticketTypeRepository,
        ITenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        TicketHistoryLogService ticketHistoryLogService,
        IPurchaseOrderRepository purchaseOrderRepository,
        IAssetRepository assetRepository,
        IPublisher publisher)
    {
        _repository = repository;
        _ticketTypeRepository = ticketTypeRepository;
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _ticketHistoryLogService = ticketHistoryLogService;
        _purchaseOrderRepository = purchaseOrderRepository;
        _assetRepository = assetRepository;
        _publisher = publisher;
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
        var isAccountable = ApprovalGateHelper.MatchesCurrentUser(existing.AccountableUserId, currentUserIdentifiers);
        var isResponsibleUser = isAuthor || isAccountable;

        var workflowConfig = TicketStateHelper.NormalizeWorkflowConfig(ticketType.WorkflowConfig);
        var resolvedStateId = TicketStateHelper.ResolveManagedStatusId(workflowConfig.States, request.CurrentStateId);
        if (resolvedStateId == null)
        {
            resolvedStateId = ResolveWorkflowStateId(ticketType, request.CurrentStateId);
            if (resolvedStateId == null)
            {
                throw new InvalidOperationException($"Status '{request.CurrentStateId}' is not allowed for this ticket type");
            }
        }

        var original = CloneTicket(existing);
        var previousStatus = existing.CurrentStateId;
        var normalizedRequestAttributes = TicketAttributeHelper.NormalizeAttributes(request.Attributes);
        var normalizedAssetIds = TicketAssetLinkingHelper.NormalizeAssetIds(request.AssetIds);
        var currentLinkedAssets = await TicketAssetLinkingHelper.ResolveLinkedAssetsAsync(ticketType, existing.AssetIds, _assetRepository, cancellationToken);
        var updatedLinkedAssets = await TicketAssetLinkingHelper.ResolveLinkedAssetsAsync(ticketType, normalizedAssetIds, _assetRepository, cancellationToken);
        TicketAttributeHelper.ValidateRequiredAttributes(ticketType, normalizedRequestAttributes);

        if (!string.Equals(existing.Summary, request.Summary, StringComparison.Ordinal))
        {
            if (!isResponsibleUser)
            {
                throw new UnauthorizedAccessException("Only the ticket author or accountable user can update the summary");
            }

            existing.Summary = request.Summary;
        }

        if (!string.Equals(existing.CurrentStateId, resolvedStateId, StringComparison.OrdinalIgnoreCase))
        {
            if (!isManager)
            {
                throw new UnauthorizedAccessException("Only users with can_manage_tickets can change the ticket status");
            }

            if (PurchaseOrderTicketConstants.IsPurchaseOrderTicketType(ticketType.TicketTypeId)
                && IsApprovedState(ticketType, resolvedStateId)
                && !HasAllApprovalGatesApproved(existing))
            {
                throw new InvalidOperationException("All approval gates must be approved before a purchase order can be marked as Approved");
            }

            existing.CurrentStateId = resolvedStateId;
        }

        if (!TicketAssetLinkingHelper.AreSameSelections(existing.AssetIds, normalizedAssetIds) && !isResponsibleUser && !isManager)
        {
            throw new UnauthorizedAccessException("Only the ticket author, accountable user, or ticket managers can update linked assets");
        }

        var existingAttributesJson = JsonSerializer.Serialize(existing.Attributes);
        var requestAttributesJson = JsonSerializer.Serialize(normalizedRequestAttributes);

        if (!isManager && (!string.Equals(existing.Priority, request.Priority, StringComparison.OrdinalIgnoreCase) ||
                           !string.Equals(existing.AssigneeId ?? string.Empty, request.AssigneeId ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                           !string.Equals(existingAttributesJson, requestAttributesJson, StringComparison.Ordinal)))
        {
            throw new UnauthorizedAccessException("Only users with can_manage_tickets can update ticket metadata");
        }

        existing.Priority = request.Priority.ToUpper();
        existing.AssigneeId = request.AssigneeId;
        existing.Attributes = normalizedRequestAttributes;
        existing.AssetIds = normalizedAssetIds;
        existing.UpdatedAt = DateTime.UtcNow;

        var success = await _repository.UpdateAsync(existing, cancellationToken);
        if (success)
        {
            if (PurchaseOrderTicketConstants.IsPurchaseOrderTicketType(ticketType.TicketTypeId)
                && !IsApprovedState(ticketType, previousStatus)
                && IsApprovedState(ticketType, existing.CurrentStateId))
            {
                await EnsurePurchaseOrderInitializedAsync(existing, cancellationToken);
            }

            var updateChanges = await _ticketHistoryLogService.BuildTicketUpdateChangesAsync(original, existing, cancellationToken);
            if (!TicketAssetLinkingHelper.AreSameSelections(original.AssetIds, existing.AssetIds))
            {
                updateChanges.Add(new ChangeLog.Application.Domain.FieldChange
                {
                    FieldName = "Linked Assets",
                    OldValue = TicketAssetLinkingHelper.FormatAssetTags(currentLinkedAssets),
                    NewValue = TicketAssetLinkingHelper.FormatAssetTags(updatedLinkedAssets)
                });
            }
            if (updateChanges.Count > 0)
            {
                await _ticketHistoryLogService.LogUpdatedAsync(existing, updateChanges, cancellationToken: cancellationToken);
            }

            if (!string.Equals(previousStatus, existing.CurrentStateId, StringComparison.OrdinalIgnoreCase))
            {
                await _ticketHistoryLogService.LogStatusChangedAsync(existing, previousStatus, existing.CurrentStateId, cancellationToken);
            }

            var existingAssetIdSet = (original.AssetIds ?? []).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var newlyAddedAssets = updatedLinkedAssets
                .Where(asset => !existingAssetIdSet.Contains(asset.Id))
                .ToList();

            foreach (var asset in newlyAddedAssets)
            {
                await _publisher.Publish(new AssetMentionedInTicketNotification(
                    asset.Id,
                    asset.AssetTag,
                    existing.TicketId,
                    existing.HumanReadableId,
                    string.IsNullOrWhiteSpace(existing.Title) ? existing.Summary : existing.Title,
                    null,
                    null
                ), cancellationToken);
            }
        }

        return new UpdateTicketResponse(success);
    }

    private async Task EnsurePurchaseOrderInitializedAsync(Domain.Ticket ticket, CancellationToken cancellationToken)
    {
        if (await _purchaseOrderRepository.GetByTicketIdAsync(ticket.TicketId, _tenantContext.TenantId, cancellationToken) != null)
        {
            return;
        }

        var poNumber = TicketAttributeHelper.GetRequiredString(ticket.Attributes, PurchaseOrderTicketConstants.PoNumberAttributeKey, "PO Number");
        if (await _purchaseOrderRepository.ExistsByPoNumberAsync(_tenantContext.TenantId, poNumber, cancellationToken: cancellationToken))
        {
            throw new InvalidOperationException("A purchase order with this PO Number already exists.");
        }

        var purchaseOrder = new AssetManagement.Application.Domain.PurchaseOrder
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = _tenantContext.TenantId,
            TicketId = ticket.TicketId,
            TicketHumanReadableId = ticket.HumanReadableId,
            PoNumber = poNumber,
            Vendor = TicketAttributeHelper.GetRequiredString(ticket.Attributes, PurchaseOrderTicketConstants.VendorAttributeKey, "Vendor"),
            Amount = TicketAttributeHelper.GetRequiredDecimal(ticket.Attributes, PurchaseOrderTicketConstants.AmountAttributeKey, "Amount"),
            Currency = TicketAttributeHelper.GetRequiredString(ticket.Attributes, PurchaseOrderTicketConstants.CurrencyAttributeKey, "Currency"),
            Description = TicketAttributeHelper.GetRequiredString(ticket.Attributes, PurchaseOrderTicketConstants.DescriptionAttributeKey, "Description"),
            CreatedByUserId = ticket.Reporter.UserId,
            AccountableUserId = string.IsNullOrWhiteSpace(ticket.AccountableUserId) ? ticket.Reporter.UserId : ticket.AccountableUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _purchaseOrderRepository.CreateAsync(purchaseOrder, cancellationToken);
    }

    private static string? ResolveWorkflowStateId(Application.Domain.TicketType ticketType, string requestedStateId)
    {
        var normalizedRequested = NormalizeState(requestedStateId);
        return ticketType.WorkflowConfig.States
            .FirstOrDefault(state =>
                string.Equals(NormalizeState(state.Id), normalizedRequested, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(NormalizeState(state.Label), normalizedRequested, StringComparison.OrdinalIgnoreCase))
            ?.Id;
    }

    private static bool IsApprovedState(Application.Domain.TicketType ticketType, string? stateId)
    {
        var normalized = NormalizeState(stateId);
        if (normalized == "approved" || normalized == "state-approved")
        {
            return true;
        }

        return ticketType.WorkflowConfig.States.Any(state =>
            string.Equals(state.Id, stateId, StringComparison.OrdinalIgnoreCase) &&
            (NormalizeState(state.Label) == "approved" || NormalizeState(state.Id) == "approved"));
    }

    private static bool HasAllApprovalGatesApproved(Domain.Ticket ticket)
    {
        return (ticket.ApprovalGates ?? [])
            .All(gate => string.Equals(ApprovalGateHelper.ResolveGateState(gate), "approved", StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeState(string? value)
    {
        return (value ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Replace('_', '-')
            .Replace(' ', '-');
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
            AccountableUserId = ticket.AccountableUserId,
            AssigneeId = ticket.AssigneeId,
            Attributes = new Dictionary<string, object>(ticket.Attributes),
            AssetIds = (ticket.AssetIds ?? []).ToList(),
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
