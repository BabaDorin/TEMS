using MongoDB.Driver;
using UserManagement.Infrastructure.Entities;
using System.Text.RegularExpressions;

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

    public async Task<List<User>> SearchByNameAsync(string? searchText, int take, string? tenantId = null, CancellationToken cancellationToken = default)
    {
        var normalizedSearchText = searchText?.Trim() ?? string.Empty;
        var safeTake = Math.Clamp(take, 1, 25);

        var filters = new List<FilterDefinition<User>>();

        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            filters.Add(Builders<User>.Filter.AnyEq(u => u.TenantIds, tenantId));
        }

        if (!string.IsNullOrWhiteSpace(normalizedSearchText))
        {
            var escaped = Regex.Escape(normalizedSearchText);
            filters.Add(Builders<User>.Filter.Regex(u => u.Name, new MongoDB.Bson.BsonRegularExpression(escaped, "i")));
        }

        var filter = filters.Count > 0
            ? Builders<User>.Filter.And(filters)
            : Builders<User>.Filter.Empty;

        return await _users
            .Find(filter)
            .SortBy(u => u.Name)
            .Limit(safeTake)
            .ToListAsync(cancellationToken);
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
