using DomainEntity = AssetManagement.Application.Domain;

namespace AssetManagement.Application.Interfaces;

public interface IAssetRepository
{
    Task<DomainEntity.Asset?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.Asset>> GetAllAsync(bool includeArchived = false, CancellationToken cancellationToken = default);
    Task<(List<DomainEntity.Asset> Assets, int TotalCount)> GetPagedAsync(
        List<string>? assetTypeIds = null,
        bool includeArchived = false,
        int pageNumber = 1,
        int pageSize = 20,
        List<string>? definitionIds = null,
        string? assetTag = null,
        CancellationToken cancellationToken = default);
    Task<DomainEntity.Asset?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<DomainEntity.Asset?> GetByAssetTagAsync(string assetTag, CancellationToken cancellationToken = default);
    Task<DomainEntity.Asset?> GetBySerialNumberOrTagAsync(string serialNumber, string assetTag, CancellationToken cancellationToken = default);
    Task<DomainEntity.Asset> CreateAsync(DomainEntity.Asset asset, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(DomainEntity.Asset asset, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ArchiveAsync(string id, string archivedBy, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.Asset>> GetByDefinitionIdAsync(string definitionId, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.Asset>> GetByAssetTypeIdAsync(string assetTypeId, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.Asset>> GetByAssignedUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.Asset>> GetByParentAssetIdAsync(string parentAssetId, CancellationToken cancellationToken = default);
    Task<List<DomainEntity.Asset>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
}
