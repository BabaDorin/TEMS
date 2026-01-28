using AssetManagement.Application.Interfaces;
using AssetManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Repositories;

public class AssetDefinitionRepository(IMongoDatabase database) : IAssetDefinitionRepository
{
    private readonly IMongoCollection<DbEntity.AssetDefinition> _collection = database.GetCollection<DbEntity.AssetDefinition>("asset_definitions");

    public async Task<DomainEntity.AssetDefinition?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetDefinition>.Filter.Eq(x => x.Id, id);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.AssetDefinition>> GetAllAsync(bool includeArchived = false, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<DbEntity.AssetDefinition>.Filter;
        var filter = includeArchived 
            ? filterBuilder.Empty 
            : filterBuilder.Eq(x => x.IsArchived, false);

        var dbEntities = await _collection.Find(filter)
            .SortBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DomainEntity.AssetDefinition?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetDefinition>.Filter.Eq(x => x.Name, name);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<DomainEntity.AssetDefinition> CreateAsync(DomainEntity.AssetDefinition assetDefinition, CancellationToken cancellationToken = default)
    {
        assetDefinition.CreatedAt = DateTime.UtcNow;
        assetDefinition.UpdatedAt = DateTime.UtcNow;
        assetDefinition.UsageCount = 0;

        var dbEntity = assetDefinition.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);

        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.AssetDefinition assetDefinition, CancellationToken cancellationToken = default)
    {
        assetDefinition.UpdatedAt = DateTime.UtcNow;

        var dbEntity = assetDefinition.ToDatabase();
        var filter = Builders<DbEntity.AssetDefinition>.Filter.Eq(x => x.Id, dbEntity.Id);
        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetDefinition>.Filter.Eq(x => x.Id, id);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);

        return result.DeletedCount > 0;
    }

    public async Task<bool> ArchiveAsync(string id, string archivedBy, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetDefinition>.Filter.Eq(x => x.Id, id);
        var update = Builders<DbEntity.AssetDefinition>.Update
            .Set(x => x.IsArchived, true)
            .Set(x => x.ArchivedAt, DateTime.UtcNow)
            .Set(x => x.ArchivedBy, archivedBy)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetDefinition>.Filter.Eq(x => x.Id, id);
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return count > 0;
    }

    public async Task<List<DomainEntity.AssetDefinition>> GetByAssetTypeIdAsync(string assetTypeId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetDefinition>.Filter.Eq(x => x.AssetTypeId, assetTypeId);
        var dbEntities = await _collection.Find(filter).ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<bool> IncrementUsageCountAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.AssetDefinition>.Filter.Eq(x => x.Id, id);
        var update = Builders<DbEntity.AssetDefinition>.Update
            .Inc(x => x.UsageCount, 1)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }
}
