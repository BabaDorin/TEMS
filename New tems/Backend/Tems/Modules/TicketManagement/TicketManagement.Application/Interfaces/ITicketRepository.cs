using TicketManagement.Application.Domain;

namespace TicketManagement.Application.Interfaces;

public interface ITicketRepository
{
    Task<Ticket?> GetByIdAsync(string ticketId, string tenantId, CancellationToken cancellationToken = default);
    Task<List<Ticket>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<List<Ticket>> GetByTicketTypeIdAsync(string ticketTypeId, string tenantId, CancellationToken cancellationToken = default);
    Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Ticket ticket, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string ticketId, string tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string ticketId, string tenantId, CancellationToken cancellationToken = default);
    Task<int> GetNextTicketNumberAsync(string tenantId, string prefix, CancellationToken cancellationToken = default);
}
