using AssetManagement.Application.Domain;

namespace AssetManagement.Application.Interfaces;

public interface IAssetPropertyRepository
{
    Task<AssetProperty?> GetByIdAsync(string propertyId, CancellationToken cancellationToken = default);
    Task<List<AssetProperty>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AssetProperty?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<AssetProperty> CreateAsync(AssetProperty property, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(AssetProperty property, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string propertyId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string propertyId, CancellationToken cancellationToken = default);
}
