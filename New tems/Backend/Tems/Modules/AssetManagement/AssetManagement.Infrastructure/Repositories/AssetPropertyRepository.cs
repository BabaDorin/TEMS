using AssetManagement.Application.Interfaces;
using AssetManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Repositories;

public class AssetPropertyRepository : IAssetPropertyRepository
{
    private readonly IMongoCollection<DbEntity.AssetProperty> _collection;

    public AssetPropertyRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<DbEntity.AssetProperty>("asset_properties");

        var indexKeys = Builders<DbEntity.AssetProperty>.IndexKeys.Ascending(x => x.Name);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<DbEntity.AssetProperty>(indexKeys, indexOptions);

        _collection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<DomainEntity.AssetProperty?> GetByIdAsync(string propertyId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetProperty>.Filter.Eq(x => x.PropertyId, propertyId);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.AssetProperty>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dbEntities = await _collection.Find(_ => true)
            .SortBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DomainEntity.AssetProperty?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetProperty>.Filter.Eq(x => x.Name, name);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<DomainEntity.AssetProperty> CreateAsync(DomainEntity.AssetProperty property, CancellationToken cancellationToken = default)
    {
        property.CreatedAt = DateTime.UtcNow;
        property.UpdatedAt = DateTime.UtcNow;

        var dbEntity = property.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);

        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.AssetProperty property, CancellationToken cancellationToken = default)
    {
        property.UpdatedAt = DateTime.UtcNow;

        var dbEntity = property.ToDatabase();
        var filter = Builders<DbEntity.AssetProperty>.Filter.Eq(x => x.PropertyId, dbEntity.PropertyId);
        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string propertyId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetProperty>.Filter.Eq(x => x.PropertyId, propertyId);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);

        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(string propertyId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetProperty>.Filter.Eq(x => x.PropertyId, propertyId);
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return count > 0;
    }
}
