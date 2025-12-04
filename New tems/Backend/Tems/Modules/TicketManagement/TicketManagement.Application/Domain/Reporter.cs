namespace TicketManagement.Application.Domain;

public class Reporter
{
    public string UserId { get; set; } = string.Empty;
    public string ChannelSource { get; set; } = string.Empty;
    public string? ChannelThreadId { get; set; }
}
