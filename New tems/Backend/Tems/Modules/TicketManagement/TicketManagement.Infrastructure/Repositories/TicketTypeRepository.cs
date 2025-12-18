using TicketManagement.Application.Interfaces;
using TicketManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using DomainEntity = TicketManagement.Application.Domain;
using DbEntity = TicketManagement.Infrastructure.Entities;

namespace TicketManagement.Infrastructure.Repositories;

public class TicketTypeRepository : ITicketTypeRepository
{
    private readonly IMongoCollection<DbEntity.TicketType> _collection;

    public TicketTypeRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<DbEntity.TicketType>("ticket_types");

        var indexKeys = Builders<DbEntity.TicketType>.IndexKeys
            .Ascending(x => x.TenantId)
            .Ascending(x => x.Name)
            .Ascending(x => x.Version);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<DbEntity.TicketType>(indexKeys, indexOptions);

        _collection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<DomainEntity.TicketType?> GetByIdAsync(string ticketTypeId, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketType>.Filter.And(
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.TicketTypeId, ticketTypeId),
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.TenantId, tenantId)
        );
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.TicketType>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketType>.Filter.Eq(x => x.TenantId, tenantId);
        var dbEntities = await _collection.Find(filter)
            .SortBy(x => x.Name)
            .ThenByDescending(x => x.Version)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DomainEntity.TicketType?> GetByNameAsync(string name, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketType>.Filter.And(
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.Name, name),
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.TenantId, tenantId)
        );
        var dbEntity = await _collection.Find(filter)
            .SortByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<DomainEntity.TicketType> CreateAsync(DomainEntity.TicketType ticketType, CancellationToken cancellationToken = default)
    {
        ticketType.CreatedAt = DateTime.UtcNow;
        ticketType.UpdatedAt = DateTime.UtcNow;

        var dbEntity = ticketType.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);

        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.TicketType ticketType, CancellationToken cancellationToken = default)
    {
        ticketType.UpdatedAt = DateTime.UtcNow;

        var dbEntity = ticketType.ToDatabase();
        var filter = Builders<DbEntity.TicketType>.Filter.And(
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.TicketTypeId, dbEntity.TicketTypeId),
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.TenantId, dbEntity.TenantId)
        );
        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string ticketTypeId, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketType>.Filter.And(
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.TicketTypeId, ticketTypeId),
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.TenantId, tenantId)
        );
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);

        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(string ticketTypeId, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketType>.Filter.And(
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.TicketTypeId, ticketTypeId),
            Builders<DbEntity.TicketType>.Filter.Eq(x => x.TenantId, tenantId)
        );
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return count > 0;
    }
}
