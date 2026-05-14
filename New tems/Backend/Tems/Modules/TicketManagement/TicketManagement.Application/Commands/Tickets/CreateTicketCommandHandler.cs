using MediatR;
using System.Text.Json;
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

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        ITicketTypeRepository ticketTypeRepository,
        ITicketConversationRepository conversationRepository,
        ITicketAiSummaryQueue ticketAiSummaryQueue,
        ITenantContext tenantContext,
        TicketHistoryLogService ticketHistoryLogService)
    {
        _ticketRepository = ticketRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _conversationRepository = conversationRepository;
        _ticketAiSummaryQueue = ticketAiSummaryQueue;
        _tenantContext = tenantContext;
        _ticketHistoryLogService = ticketHistoryLogService;
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
            AssigneeId = request.AssigneeId,
            Attributes = ConvertAttributes(request.Attributes),
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

    private Dictionary<string, object> ConvertAttributes(Dictionary<string, object> attributes)
    {
        var converted = new Dictionary<string, object>();
        
        foreach (var kvp in attributes)
        {
            if (kvp.Value is JsonElement jsonElement)
            {
                converted[kvp.Key] = jsonElement.ValueKind switch
                {
                    JsonValueKind.String => jsonElement.GetString() ?? string.Empty,
                    JsonValueKind.Number => jsonElement.TryGetInt64(out var longValue) ? longValue : jsonElement.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Null => null!,
                    _ => kvp.Value
                };
            }
            else
            {
                converted[kvp.Key] = kvp.Value;
            }
        }
        
        return converted;
    }

    private static bool ShouldGenerateAiSummary(TicketType ticketType)
    {
        var name = $"{ticketType.Name} {ticketType.Description} {ticketType.TicketTypeId}".ToLowerInvariant();
        var isHardwareIssue = name.Contains("hardware issue") || name.Contains("hardware_issue") || name.Contains("hardware");
        var isNetworkIssue = name.Contains("network issue") || name.Contains("network_issue") || name.Contains("network");
        var isIncident = string.Equals(ticketType.ItilCategory, "incident", StringComparison.OrdinalIgnoreCase);

        return isIncident && (isHardwareIssue || isNetworkIssue);
    }
}
