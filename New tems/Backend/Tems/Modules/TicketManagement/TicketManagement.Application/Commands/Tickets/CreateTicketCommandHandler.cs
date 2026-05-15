using AssetManagement.Application.Interfaces;
using MediatR;
using Tems.Common.Notifications;
using Tems.Common.Tenant;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Domain;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Models;
using TicketManagement.Application.Services;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, CreateTicketResponse>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly ITicketConversationRepository _conversationRepository;
    private readonly ITicketAiSummaryQueue _ticketAiSummaryQueue;
    private readonly ITenantContext _tenantContext;
    private readonly TicketHistoryLogService _ticketHistoryLogService;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IPublisher _publisher;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        ITicketTypeRepository ticketTypeRepository,
        ITicketConversationRepository conversationRepository,
        ITicketAiSummaryQueue ticketAiSummaryQueue,
        ITenantContext tenantContext,
        TicketHistoryLogService ticketHistoryLogService,
        IPurchaseOrderRepository purchaseOrderRepository,
        IAssetRepository assetRepository,
        IPublisher publisher)
    {
        _ticketRepository = ticketRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _conversationRepository = conversationRepository;
        _ticketAiSummaryQueue = ticketAiSummaryQueue;
        _tenantContext = tenantContext;
        _ticketHistoryLogService = ticketHistoryLogService;
        _purchaseOrderRepository = purchaseOrderRepository;
        _assetRepository = assetRepository;
        _publisher = publisher;
    }

    public async Task<CreateTicketResponse> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await _ticketTypeRepository.GetByIdAsync(request.TicketTypeId, _tenantContext.TenantId, cancellationToken);
        if (ticketType == null)
            throw new KeyNotFoundException($"TicketType with ID {request.TicketTypeId} not found");

        var prefix = GetPrefixFromItilCategory(ticketType.ItilCategory);
        var nextNumber = await _ticketRepository.GetNextTicketNumberAsync(_tenantContext.TenantId, prefix, cancellationToken);
        var humanReadableId = $"{prefix}-{nextNumber}";
        var workflowConfig = TicketStateHelper.NormalizeWorkflowConfig(ticketType.WorkflowConfig);
        var normalizedAttributes = TicketAttributeHelper.NormalizeAttributes(request.Attributes);
        var normalizedAssetIds = TicketAssetLinkingHelper.NormalizeAssetIds(request.AssetIds);
        var linkedAssets = await TicketAssetLinkingHelper.ResolveLinkedAssetsAsync(ticketType, normalizedAssetIds, _assetRepository, cancellationToken);
        TicketAttributeHelper.ValidateRequiredAttributes(ticketType, normalizedAttributes);

        if (PurchaseOrderTicketConstants.IsPurchaseOrderTicketType(ticketType.TicketTypeId))
        {
            var poNumber = TicketAttributeHelper.GetRequiredString(
                normalizedAttributes,
                PurchaseOrderTicketConstants.PoNumberAttributeKey,
                "PO Number");

            if (await _ticketRepository.ExistsByAttributeValueAsync(
                    _tenantContext.TenantId,
                    PurchaseOrderTicketConstants.PoNumberAttributeKey,
                    poNumber,
                    cancellationToken: cancellationToken))
            {
                throw new InvalidOperationException("A purchase order ticket with this PO Number already exists.");
            }

            if (await _purchaseOrderRepository.ExistsByPoNumberAsync(_tenantContext.TenantId, poNumber, cancellationToken: cancellationToken))
            {
                throw new InvalidOperationException("A purchase order with this PO Number already exists.");
            }
        }

        var accountableUserId = string.IsNullOrWhiteSpace(request.AccountableUserId)
            ? request.Reporter.UserId
            : request.AccountableUserId.Trim();

        var ticket = new Ticket
        {
            TicketId = Guid.NewGuid().ToString(),
            TenantId = _tenantContext.TenantId,
            TicketTypeId = request.TicketTypeId,
            HumanReadableId = humanReadableId,
            Title = request.Title.Trim(),
            Summary = request.Summary.Trim(),
            CurrentStateId = workflowConfig.InitialStateId,
            Priority = request.Priority.ToUpper(),
            Reporter = new Reporter
            {
                UserId = request.Reporter.UserId,
                ChannelSource = request.Reporter.ChannelSource.ToUpper(),
                ChannelThreadId = request.Reporter.ChannelThreadId
            },
            AccountableUserId = accountableUserId,
            AssigneeId = request.AssigneeId,
            Attributes = normalizedAttributes,
            AssetIds = normalizedAssetIds,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _ticketRepository.CreateAsync(ticket, cancellationToken);

        var conversation = new TicketConversation
        {
            ConversationId = Guid.NewGuid().ToString(),
            TicketId = created.TicketId,
            Messages = new List<TicketMessage>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _conversationRepository.CreateAsync(conversation, cancellationToken);
        await _ticketHistoryLogService.LogCreatedAsync(created, cancellationToken);
        await PublishAssetMentionsAsync(created, linkedAssets, cancellationToken);

        if (ShouldGenerateAiSummary(ticketType))
        {
            await _ticketAiSummaryQueue.EnqueueAsync(
                new TicketAiSummaryWorkItem(
                    created.TicketId,
                    created.TenantId,
                    created.TicketTypeId,
                    ticketType.Name,
                    created.Summary,
                    created.Priority,
                    created.CurrentStateId,
                    new Dictionary<string, object>(created.Attributes)),
                CancellationToken.None);
        }

        return new CreateTicketResponse(created.TicketId, created.HumanReadableId);
    }

    private string GetPrefixFromItilCategory(string category)
    {
        return category.ToUpper() switch
        {
            "INCIDENT" => "INC",
            "PROBLEM" => "PRB",
            "CHANGE" => "CHG",
            "REQUEST" => "REQ",
            "SECURITY_INCIDENT" => "SEC",
            "ALERT" => "ALT",
            _ => "TKT"
        };
    }

    private static bool ShouldGenerateAiSummary(TicketType ticketType)
    {
        var name = $"{ticketType.Name} {ticketType.Description} {ticketType.TicketTypeId}".ToLowerInvariant();
        var isHardwareIssue = name.Contains("hardware issue") || name.Contains("hardware_issue") || name.Contains("hardware");
        var isNetworkIssue = name.Contains("network issue") || name.Contains("network_issue") || name.Contains("network");
        var isIncident = string.Equals(ticketType.ItilCategory, "incident", StringComparison.OrdinalIgnoreCase);

        return isIncident && (isHardwareIssue || isNetworkIssue);
    }

    private async Task PublishAssetMentionsAsync(Ticket ticket, IEnumerable<AssetManagement.Application.Domain.Asset> assets, CancellationToken cancellationToken)
    {
        foreach (var asset in assets)
        {
            await _publisher.Publish(new AssetMentionedInTicketNotification(
                asset.Id,
                asset.AssetTag,
                ticket.TicketId,
                ticket.HumanReadableId,
                string.IsNullOrWhiteSpace(ticket.Title) ? ticket.Summary : ticket.Title,
                null,
                null
            ), cancellationToken);
        }
    }
}
