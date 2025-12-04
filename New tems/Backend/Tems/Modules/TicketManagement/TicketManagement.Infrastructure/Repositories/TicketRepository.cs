using TicketManagement.Application.Interfaces;
using TicketManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using DomainEntity = TicketManagement.Application.Domain;
using DbEntity = TicketManagement.Infrastructure.Entities;

namespace TicketManagement.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly IMongoCollection<DbEntity.Ticket> _collection;

    public TicketRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<DbEntity.Ticket>("tickets");

        var tenantIndex = Builders<DbEntity.Ticket>.IndexKeys.Ascending(x => x.TenantId);
        _collection.Indexes.CreateOneAsync(new CreateIndexModel<DbEntity.Ticket>(tenantIndex));

        var humanReadableIndex = Builders<DbEntity.Ticket>.IndexKeys
            .Ascending(x => x.TenantId)
            .Ascending(x => x.HumanReadableId);
        var humanReadableOptions = new CreateIndexOptions { Unique = true };
        _collection.Indexes.CreateOneAsync(new CreateIndexModel<DbEntity.Ticket>(humanReadableIndex, humanReadableOptions));

        var ticketTypeIndex = Builders<DbEntity.Ticket>.IndexKeys
            .Ascending(x => x.TenantId)
            .Ascending(x => x.TicketTypeId);
        _collection.Indexes.CreateOneAsync(new CreateIndexModel<DbEntity.Ticket>(ticketTypeIndex));
    }

    public async Task<DomainEntity.Ticket?> GetByIdAsync(string ticketId, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Ticket>.Filter.And(
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TicketId, ticketId),
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TenantId, tenantId)
        );
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.Ticket>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Ticket>.Filter.Eq(x => x.TenantId, tenantId);
        var dbEntities = await _collection.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<List<DomainEntity.Ticket>> GetByTicketTypeIdAsync(string ticketTypeId, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Ticket>.Filter.And(
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TicketTypeId, ticketTypeId),
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TenantId, tenantId)
        );
        var dbEntities = await _collection.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DomainEntity.Ticket> CreateAsync(DomainEntity.Ticket ticket, CancellationToken cancellationToken = default)
    {
        ticket.CreatedAt = DateTime.UtcNow;
        ticket.UpdatedAt = DateTime.UtcNow;

        var dbEntity = ticket.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);

        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.Ticket ticket, CancellationToken cancellationToken = default)
    {
        ticket.UpdatedAt = DateTime.UtcNow;

        var dbEntity = ticket.ToDatabase();
        var filter = Builders<DbEntity.Ticket>.Filter.And(
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TicketId, dbEntity.TicketId),
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TenantId, dbEntity.TenantId)
        );
        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string ticketId, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Ticket>.Filter.And(
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TicketId, ticketId),
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TenantId, tenantId)
        );
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);

        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(string ticketId, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Ticket>.Filter.And(
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TicketId, ticketId),
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TenantId, tenantId)
        );
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return count > 0;
    }

    public async Task<int> GetNextTicketNumberAsync(string tenantId, string prefix, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Ticket>.Filter.And(
            Builders<DbEntity.Ticket>.Filter.Eq(x => x.TenantId, tenantId),
            Builders<DbEntity.Ticket>.Filter.Regex(x => x.HumanReadableId, $"^{prefix}-")
        );

        var lastTicket = await _collection.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastTicket == null)
            return 1;

        var parts = lastTicket.HumanReadableId.Split('-');
        if (parts.Length >= 2 && int.TryParse(parts[1], out int lastNumber))
        {
            return lastNumber + 1;
        }

        return 1;
    }
}
