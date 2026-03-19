using TicketManagement.Application.Domain;

namespace TicketManagement.Application.Interfaces;

public interface ITicketConversationRepository
{
    Task<TicketConversation?> GetByTicketIdAsync(string ticketId, CancellationToken cancellationToken = default);
    Task<TicketConversation> CreateAsync(TicketConversation conversation, CancellationToken cancellationToken = default);
    Task<bool> AddMessageAsync(string ticketId, TicketMessage message, CancellationToken cancellationToken = default);
    Task<TicketMessage?> EditMessageAsync(string ticketId, string messageId, string content, CancellationToken cancellationToken = default);
    Task<bool> DeleteMessageAsync(string ticketId, string messageId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string ticketId, CancellationToken cancellationToken = default);
}
