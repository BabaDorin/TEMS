using AssetManagement.Application.Interfaces;
using AssetManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Repositories;

public class AssetRepository(IMongoDatabase database) : IAssetRepository
{
    private readonly IMongoCollection<DbEntity.Asset> _collection = database.GetCollection<DbEntity.Asset>("assets");

    public async Task<DomainEntity.Asset?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq(x => x.Id, id);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.Asset>> GetAllAsync(bool includeArchived = false, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<DbEntity.Asset>.Filter;
        var filter = includeArchived 
            ? filterBuilder.Empty 
            : filterBuilder.Eq(x => x.IsArchived, false);

        var dbEntities = await _collection.Find(filter)
            .SortBy(x => x.AssetTag)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<(List<DomainEntity.Asset> Assets, int TotalCount)> GetPagedAsync(
        List<string>? assetTypeIds = null,
        bool includeArchived = false,
        int pageNumber = 1,
        int pageSize = 20,
        List<string>? definitionIds = null,
        string? assetTag = null,
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<DbEntity.Asset>.Filter;
        var filters = new List<FilterDefinition<DbEntity.Asset>>();

        if (!includeArchived)
        {
            filters.Add(filterBuilder.Eq(x => x.IsArchived, false));
        }

        if (assetTypeIds?.Count > 0)
        {
            filters.Add(filterBuilder.In("definition.asset_type_id", assetTypeIds));
        }

        if (definitionIds?.Count > 0)
        {
            filters.Add(filterBuilder.In("definition.definition_id", definitionIds));
        }

        if (!string.IsNullOrWhiteSpace(assetTag))
        {
            filters.Add(filterBuilder.Regex(x => x.AssetTag, new MongoDB.Bson.BsonRegularExpression(assetTag, "i")));
        }

        var filter = filters.Count > 0 
            ? filterBuilder.And(filters) 
            : filterBuilder.Empty;

        var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var skip = (pageNumber - 1) * pageSize;
        var dbEntities = await _collection.Find(filter)
            .SortBy(x => x.AssetTag)
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var assets = dbEntities.Select(x => x.ToDomain()).ToList();
        return (assets, (int)totalCount);
    }

    public async Task<DomainEntity.Asset?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq(x => x.SerialNumber, serialNumber);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<DomainEntity.Asset?> GetByAssetTagAsync(string assetTag, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq(x => x.AssetTag, assetTag);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<DomainEntity.Asset> CreateAsync(DomainEntity.Asset asset, CancellationToken cancellationToken = default)
    {
        asset.CreatedAt = DateTime.UtcNow;
        asset.UpdatedAt = DateTime.UtcNow;

        var dbEntity = asset.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);

        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.Asset asset, CancellationToken cancellationToken = default)
    {
        asset.UpdatedAt = DateTime.UtcNow;

        var dbEntity = asset.ToDatabase();
        var filter = Builders<DbEntity.Asset>.Filter.Eq(x => x.Id, dbEntity.Id);
        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq(x => x.Id, id);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);

        return result.DeletedCount > 0;
    }

    public async Task<bool> ArchiveAsync(string id, string archivedBy, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq(x => x.Id, id);
        var update = Builders<DbEntity.Asset>.Update
            .Set(x => x.IsArchived, true)
            .Set(x => x.ArchivedAt, DateTime.UtcNow)
            .Set(x => x.ArchivedBy, archivedBy)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq(x => x.Id, id);
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return count > 0;
    }

    public async Task<List<DomainEntity.Asset>> GetByDefinitionIdAsync(string definitionId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq("definition.definition_id", definitionId);
        var dbEntities = await _collection.Find(filter).ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<List<DomainEntity.Asset>> GetByAssetTypeIdAsync(string assetTypeId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq("definition.asset_type_id", assetTypeId);
        var dbEntities = await _collection.Find(filter).ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<List<DomainEntity.Asset>> GetByAssignedUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq("assignment.assigned_to_user_id", userId);
        var dbEntities = await _collection.Find(filter).ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<List<DomainEntity.Asset>> GetByParentAssetIdAsync(string parentAssetId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq(x => x.ParentAssetId, parentAssetId);
        var dbEntities = await _collection.Find(filter).ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<List<DomainEntity.Asset>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Asset>.Filter.Eq(x => x.Status, status);
        var dbEntities = await _collection.Find(filter).ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }
}
