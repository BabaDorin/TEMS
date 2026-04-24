using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tems.Host.Configuration;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Models;

namespace Tems.Host.Services;

public sealed class TicketAiSummaryBackgroundService(
    IServiceScopeFactory scopeFactory,
    DeepSeekAiSupportClient deepSeekClient,
    IOptions<AiSupportOptions> options,
    ILogger<TicketAiSummaryBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<ITicketAiSummaryQueue>();

        await foreach (var workItem in queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessWorkItemAsync(workItem, scopeFactory, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to generate AI summary for ticket {TicketId}", workItem.TicketId);
            }
        }
    }

    private async Task ProcessWorkItemAsync(
        TicketAiSummaryWorkItem workItem,
        IServiceScopeFactory scopeFactory,
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var ticketRepository = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
        var ticketTypeRepository = scope.ServiceProvider.GetRequiredService<ITicketTypeRepository>();

        var ticketType = await ticketTypeRepository.GetByIdAsync(workItem.TicketTypeId, workItem.TenantId, cancellationToken);
        if (ticketType == null || !ShouldGenerateAiSummary(ticketType.Name, ticketType.Description, ticketType.TicketTypeId, ticketType.ItilCategory))
        {
            return;
        }

        var ticket = await ticketRepository.GetByIdAsync(workItem.TicketId, workItem.TenantId, cancellationToken);
        if (ticket == null)
        {
            logger.LogWarning("Ticket {TicketId} no longer exists while generating AI summary.", workItem.TicketId);
            return;
        }

        if (!string.IsNullOrWhiteSpace(ticket.AiSummary))
        {
            return;
        }

        var prompt = BuildPrompt(workItem, ticketType.Name);
        var timeout = TimeSpan.FromMinutes(Math.Max(1, options.Value.TimeoutMinutes));
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(timeout);

        var summary = await deepSeekClient.GenerateCompletionAsync(
            options.Value.TicketSummarySystemPrompt,
            prompt,
            timeoutCts.Token);

        summary = summary.Trim();
        if (string.IsNullOrWhiteSpace(summary))
        {
            logger.LogWarning("DeepSeek returned an empty AI summary for ticket {TicketId}.", workItem.TicketId);
            return;
        }

        ticket.AiSummary = summary;
        var saved = await ticketRepository.UpdateAsync(ticket, timeoutCts.Token);
        if (!saved)
        {
            logger.LogWarning("Ticket AI summary could not be saved for ticket {TicketId}.", workItem.TicketId);
        }
    }

    private static string BuildPrompt(TicketAiSummaryWorkItem workItem, string ticketTypeName)
    {
        var attributes = workItem.Attributes.Count == 0
            ? "None"
            : string.Join(Environment.NewLine, workItem.Attributes.Select(kvp => $"- {kvp.Key}: {kvp.Value}"));

        return $"""
Ticket type: {ticketTypeName}
Ticket description: {workItem.TicketDescription}
Priority: {workItem.Priority}
State: {workItem.CurrentStateId}

Relevant attributes:
{attributes}
""";
    }

    private static bool ShouldGenerateAiSummary(string name, string description, string ticketTypeId, string itilCategory)
    {
        var haystack = $"{name} {description} {ticketTypeId}".ToLowerInvariant();
        var isHardwareIssue = haystack.Contains("hardware issue") || haystack.Contains("hardware_issue") || haystack.Contains("hardware");
        var isNetworkIssue = haystack.Contains("network issue") || haystack.Contains("network_issue") || haystack.Contains("network");
        var isIncident = string.Equals(itilCategory, "incident", StringComparison.OrdinalIgnoreCase);

        return isIncident && (isHardwareIssue || isNetworkIssue);
    }
}
