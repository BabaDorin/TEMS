namespace TicketManagement.Application.Domain;

public class TicketMessage
{
    public string SenderType { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ChannelMessageId { get; set; }
    public bool IsInternalNote { get; set; }
}
