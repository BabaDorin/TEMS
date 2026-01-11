using DomainEntity = AssetManagement.Application.Domain;

namespace AssetManagement.Application.Interfaces;

public interface IAssetTypeRepository
{
    Task<DomainEntity.AssetType?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.AssetType>> GetAllAsync(bool includeArchived = false, CancellationToken cancellationToken = default);
    Task<DomainEntity.AssetType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<DomainEntity.AssetType?> GetByNameInsensitiveAsync(string name, CancellationToken cancellationToken = default);
    Task<DomainEntity.AssetType> CreateAsync(DomainEntity.AssetType assetType, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(DomainEntity.AssetType assetType, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ArchiveAsync(string id, string archivedBy, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.AssetType>> GetByParentTypeIdAsync(string parentTypeId, CancellationToken cancellationToken = default);
}
