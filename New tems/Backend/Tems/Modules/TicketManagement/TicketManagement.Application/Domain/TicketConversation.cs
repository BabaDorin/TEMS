namespace TicketManagement.Application.Domain;

public class TicketConversation
{
    public string ConversationId { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
    public List<TicketMessage> Messages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
