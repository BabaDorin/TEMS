using UserManagement.Infrastructure.Entities;

namespace UserManagement.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdentityProviderIdAsync(string identityProviderId, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByKeycloakIdAsync(string keycloakId, CancellationToken cancellationToken = default);
    Task<(List<User> Users, int TotalCount)> GetAllAsync(int page, int pageSize, string? tenantId = null, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
