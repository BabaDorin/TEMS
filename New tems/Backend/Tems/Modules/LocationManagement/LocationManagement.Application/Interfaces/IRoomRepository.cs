using DomainEntity = LocationManagement.Application.Domain;

namespace LocationManagement.Application.Interfaces;

public interface IRoomRepository
{
    Task<DomainEntity.Room?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.Room>> GetAllAsync(string tenantId, string? siteId = null, string? buildingId = null, CancellationToken cancellationToken = default);
    Task<DomainEntity.Room> CreateAsync(DomainEntity.Room room, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(DomainEntity.Room room, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, string tenantId, CancellationToken cancellationToken = default);
}
