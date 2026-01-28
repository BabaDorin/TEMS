using DomainEntity = LocationManagement.Application.Domain;

namespace LocationManagement.Application.Interfaces;

public interface ISiteRepository
{
    Task<DomainEntity.Site?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.Site>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<DomainEntity.Site> CreateAsync(DomainEntity.Site site, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(DomainEntity.Site site, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, string tenantId, CancellationToken cancellationToken = default);
}
