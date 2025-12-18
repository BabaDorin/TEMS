using TicketManagement.Application.Domain;

namespace TicketManagement.Application.Interfaces;

public interface ITicketConversationRepository
{
    Task<TicketConversation?> GetByTicketIdAsync(string ticketId, CancellationToken cancellationToken = default);
    Task<TicketConversation> CreateAsync(TicketConversation conversation, CancellationToken cancellationToken = default);
    Task<bool> AddMessageAsync(string ticketId, TicketMessage message, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string ticketId, CancellationToken cancellationToken = default);
}
