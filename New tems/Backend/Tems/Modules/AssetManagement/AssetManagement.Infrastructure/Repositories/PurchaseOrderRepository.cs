using AssetManagement.Application.Interfaces;
using AssetManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly IMongoCollection<DbEntity.PurchaseOrder> _collection;

    public PurchaseOrderRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<DbEntity.PurchaseOrder>("purchase_orders");

        _collection.Indexes.CreateOne(new CreateIndexModel<DbEntity.PurchaseOrder>(
            Builders<DbEntity.PurchaseOrder>.IndexKeys
                .Ascending(x => x.TenantId)
                .Ascending(x => x.PoNumber),
            new CreateIndexOptions { Unique = true }));

        _collection.Indexes.CreateOne(new CreateIndexModel<DbEntity.PurchaseOrder>(
            Builders<DbEntity.PurchaseOrder>.IndexKeys
                .Ascending(x => x.TenantId)
                .Ascending(x => x.TicketId),
            new CreateIndexOptions { Unique = true }));
    }

    public async Task<DomainEntity.PurchaseOrder?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.PurchaseOrder>.Filter.And(
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.TenantId, tenantId));

        var entity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return entity?.ToDomain();
    }

    public async Task<DomainEntity.PurchaseOrder?> GetByTicketIdAsync(string ticketId, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.PurchaseOrder>.Filter.And(
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.TicketId, ticketId),
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.TenantId, tenantId));

        var entity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return entity?.ToDomain();
    }

    public async Task<DomainEntity.PurchaseOrder?> GetByPoNumberAsync(string poNumber, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.PurchaseOrder>.Filter.And(
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.PoNumber, poNumber),
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.TenantId, tenantId));

        var entity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return entity?.ToDomain();
    }

    public async Task<List<DomainEntity.PurchaseOrder>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.TenantId, tenantId);
        var entities = await _collection.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities.Select(entity => entity.ToDomain()).ToList();
    }

    public async Task<DomainEntity.PurchaseOrder> CreateAsync(DomainEntity.PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
    {
        purchaseOrder.CreatedAt = DateTime.UtcNow;
        purchaseOrder.UpdatedAt = DateTime.UtcNow;

        var entity = purchaseOrder.ToDatabase();
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity.ToDomain();
    }

    public async Task<bool> DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.PurchaseOrder>.Filter.And(
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.TenantId, tenantId));

        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsByPoNumberAsync(string tenantId, string poNumber, string? excludeId = null, CancellationToken cancellationToken = default)
    {
        var filters = new List<FilterDefinition<DbEntity.PurchaseOrder>>
        {
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.TenantId, tenantId),
            Builders<DbEntity.PurchaseOrder>.Filter.Eq(x => x.PoNumber, poNumber)
        };

        if (!string.IsNullOrWhiteSpace(excludeId))
        {
            filters.Add(Builders<DbEntity.PurchaseOrder>.Filter.Ne(x => x.Id, excludeId));
        }

        var count = await _collection.CountDocumentsAsync(
            Builders<DbEntity.PurchaseOrder>.Filter.And(filters),
            cancellationToken: cancellationToken);

        return count > 0;
    }
}
