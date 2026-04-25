using System.Threading.Channels;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Models;

namespace Tems.Host.Services;

public sealed class TicketAiSummaryQueue : ITicketAiSummaryQueue
{
    private readonly Channel<TicketAiSummaryWorkItem> _channel = Channel.CreateUnbounded<TicketAiSummaryWorkItem>(
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask EnqueueAsync(TicketAiSummaryWorkItem workItem, CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(workItem, cancellationToken);

    public IAsyncEnumerable<TicketAiSummaryWorkItem> ReadAllAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}
