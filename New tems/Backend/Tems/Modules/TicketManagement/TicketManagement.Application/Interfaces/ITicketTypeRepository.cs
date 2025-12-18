using TicketManagement.Application.Domain;

namespace TicketManagement.Application.Interfaces;

public interface ITicketTypeRepository
{
    Task<TicketType?> GetByIdAsync(string ticketTypeId, string tenantId, CancellationToken cancellationToken = default);
    Task<List<TicketType>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<TicketType?> GetByNameAsync(string name, string tenantId, CancellationToken cancellationToken = default);
    Task<TicketType> CreateAsync(TicketType ticketType, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TicketType ticketType, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string ticketTypeId, string tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string ticketTypeId, string tenantId, CancellationToken cancellationToken = default);
}
