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

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _users
            .Find(u => u.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _users
            .Find(u => u.Email.ToLower() == email.ToLower())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByKeycloakIdAsync(string keycloakId, CancellationToken cancellationToken = default)
    {
        return await _users
            .Find(u => u.KeycloakId == keycloakId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(List<User> Users, int TotalCount)> GetAllAsync(int page, int pageSize, string? tenantId = null, CancellationToken cancellationToken = default)
    {
        var filter = tenantId != null 
            ? Builders<User>.Filter.AnyEq(u => u.TenantIds, tenantId)
            : Builders<User>.Filter.Empty;

        var totalCount = await _users.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        
        var users = await _users
            .Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .SortByDescending(u => u.CreatedAt)
            .ToListAsync(cancellationToken);

        return (users, (int)totalCount);
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

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _users.DeleteOneAsync(u => u.Id == id, cancellationToken);
    }
}
