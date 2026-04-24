using TicketManagement.Application.Models;

namespace TicketManagement.Application.Interfaces;

public interface ITicketAiSummaryQueue
{
    ValueTask EnqueueAsync(TicketAiSummaryWorkItem workItem, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TicketAiSummaryWorkItem> ReadAllAsync(CancellationToken cancellationToken);
}
