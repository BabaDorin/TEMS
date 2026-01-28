using UserManagement.Infrastructure.Entities;

namespace UserManagement.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdentityProviderIdAsync(string identityProviderId, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
}
