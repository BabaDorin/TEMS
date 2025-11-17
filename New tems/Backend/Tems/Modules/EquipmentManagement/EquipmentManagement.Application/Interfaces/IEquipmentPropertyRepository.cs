using EquipmentManagement.Application.Domain;

namespace EquipmentManagement.Application.Interfaces;

public interface IEquipmentPropertyRepository
{
    Task<EquipmentProperty?> GetByIdAsync(string propertyId, CancellationToken cancellationToken = default);
    Task<List<EquipmentProperty>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<EquipmentProperty?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<EquipmentProperty> CreateAsync(EquipmentProperty property, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(EquipmentProperty property, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string propertyId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string propertyId, CancellationToken cancellationToken = default);
}
