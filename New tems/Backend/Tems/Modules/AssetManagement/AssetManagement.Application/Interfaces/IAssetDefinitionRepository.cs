using DomainEntity = AssetManagement.Application.Domain;

namespace AssetManagement.Application.Interfaces;

public interface IAssetDefinitionRepository
{
    Task<DomainEntity.AssetDefinition?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.AssetDefinition>> GetAllAsync(bool includeArchived = false, CancellationToken cancellationToken = default);
    Task<DomainEntity.AssetDefinition?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<DomainEntity.AssetDefinition> CreateAsync(DomainEntity.AssetDefinition assetDefinition, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(DomainEntity.AssetDefinition assetDefinition, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ArchiveAsync(string id, string archivedBy, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.AssetDefinition>> GetByAssetTypeIdAsync(string assetTypeId, CancellationToken cancellationToken = default);
    Task<bool> IncrementUsageCountAsync(string id, CancellationToken cancellationToken = default);
}
