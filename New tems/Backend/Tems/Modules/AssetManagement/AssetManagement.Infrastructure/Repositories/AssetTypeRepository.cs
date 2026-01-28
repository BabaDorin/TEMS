using AssetManagement.Application.Interfaces;
using AssetManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Repositories;

public class AssetTypeRepository(IMongoDatabase database) : IAssetTypeRepository
{
    private readonly IMongoCollection<DbEntity.AssetType> _collection = database.GetCollection<DbEntity.AssetType>("asset_types");

    public async Task<DomainEntity.AssetType?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetType>.Filter.Eq(x => x.Id, id);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.AssetType>> GetAllAsync(bool includeArchived = false, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<DbEntity.AssetType>.Filter;
        var filter = includeArchived 
            ? filterBuilder.Empty 
            : filterBuilder.Eq(x => x.IsArchived, false);

        var dbEntities = await _collection.Find(filter)
            .SortBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DomainEntity.AssetType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetType>.Filter.Eq(x => x.Name, name);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<DomainEntity.AssetType?> GetByNameInsensitiveAsync(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetType>.Filter.Eq(x => x.Name, name);
        var collation = new Collation("en", strength: CollationStrength.Secondary);
        var options = new FindOptions
        {
            Collation = collation
        };
        var dbEntity = await _collection.Find(filter, options).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<DomainEntity.AssetType> CreateAsync(DomainEntity.AssetType assetType, CancellationToken cancellationToken = default)
    {
        assetType.CreatedAt = DateTime.UtcNow;
        assetType.UpdatedAt = DateTime.UtcNow;

        var dbEntity = assetType.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);

        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.AssetType assetType, CancellationToken cancellationToken = default)
    {
        assetType.UpdatedAt = DateTime.UtcNow;

        var dbEntity = assetType.ToDatabase();
        var filter = Builders<DbEntity.AssetType>.Filter.Eq(x => x.Id, dbEntity.Id);
        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetType>.Filter.Eq(x => x.Id, id);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);

        return result.DeletedCount > 0;
    }

    public async Task<bool> ArchiveAsync(string id, string archivedBy, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetType>.Filter.Eq(x => x.Id, id);
        var update = Builders<DbEntity.AssetType>.Update
            .Set(x => x.IsArchived, true)
            .Set(x => x.ArchivedAt, DateTime.UtcNow)
            .Set(x => x.ArchivedBy, archivedBy)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetType>.Filter.Eq(x => x.Id, id);
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return count > 0;
    }

    public async Task<List<DomainEntity.AssetType>> GetByParentTypeIdAsync(string parentTypeId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetType>.Filter.Eq(x => x.ParentTypeId, parentTypeId);
        var dbEntities = await _collection.Find(filter).ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }
}
