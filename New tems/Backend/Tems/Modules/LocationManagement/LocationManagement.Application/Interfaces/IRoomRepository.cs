using DomainEntity = LocationManagement.Application.Domain;

namespace LocationManagement.Application.Interfaces;

public interface IRoomRepository
{
    Task<DomainEntity.Room?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<(List<DomainEntity.Room> Rooms, int TotalCount)> GetAllAsync(
        string tenantId,
        string? siteId = null,
        string? buildingId = null,
        int pageNumber = 1,
        int pageSize = 20,
        string? searchText = null,
        CancellationToken cancellationToken = default);
    Task<DomainEntity.Room> CreateAsync(DomainEntity.Room room, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(DomainEntity.Room room, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, string tenantId, CancellationToken cancellationToken = default);
}
