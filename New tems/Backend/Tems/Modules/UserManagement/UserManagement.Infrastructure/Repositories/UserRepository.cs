using MongoDB.Driver;
using UserManagement.Infrastructure.Entities;

namespace UserManagement.Infrastructure.Repositories;

public class UserRepository(IMongoDatabase database) : IUserRepository
{
    private readonly IMongoCollection<User> _users = database.GetCollection<User>("users");

    public async Task<User?> GetByIdentityProviderIdAsync(string identityProviderId, CancellationToken cancellationToken = default)
    {
        return await _users
            .Find(u => u.IdentityProviderId == identityProviderId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _users.InsertOneAsync(user, cancellationToken: cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        await _users.ReplaceOneAsync(
            u => u.Id == user.Id,
            user,
            cancellationToken: cancellationToken
        );
        return user;
    }
}
