using DomainEntity = LocationManagement.Application.Domain;

namespace LocationManagement.Application.Interfaces;

public interface IBuildingRepository
{
    Task<DomainEntity.Building?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.Building>> GetAllAsync(string tenantId, string? siteId = null, CancellationToken cancellationToken = default);
    Task<DomainEntity.Building> CreateAsync(DomainEntity.Building building, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(DomainEntity.Building building, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, string tenantId, CancellationToken cancellationToken = default);
}
