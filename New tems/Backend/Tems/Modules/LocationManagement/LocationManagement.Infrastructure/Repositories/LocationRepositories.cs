using LocationManagement.Application.Interfaces;
using LocationManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using DomainEntity = LocationManagement.Application.Domain;
using DbEntity = LocationManagement.Infrastructure.Entities;

namespace LocationManagement.Infrastructure.Repositories;

public class SiteRepository(IMongoDatabase database) : ISiteRepository
{
    private readonly IMongoCollection<DbEntity.Site> _collection = database.GetCollection<DbEntity.Site>("sites");

    public async Task<DomainEntity.Site?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Site>.Filter.And(
            Builders<DbEntity.Site>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.Site>.Filter.Eq(x => x.TenantId, tenantId));

        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.Site>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Site>.Filter.Eq(x => x.TenantId, tenantId);
        var dbEntities = await _collection.Find(filter)
            .SortBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DomainEntity.Site> CreateAsync(DomainEntity.Site site, CancellationToken cancellationToken = default)
    {
        var dbEntity = site.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);
        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.Site site, CancellationToken cancellationToken = default)
    {
        var dbEntity = site.ToDatabase();
        var filter = Builders<DbEntity.Site>.Filter.And(
            Builders<DbEntity.Site>.Filter.Eq(x => x.Id, dbEntity.Id),
            Builders<DbEntity.Site>.Filter.Eq(x => x.TenantId, dbEntity.TenantId));

        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Site>.Filter.And(
            Builders<DbEntity.Site>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.Site>.Filter.Eq(x => x.TenantId, tenantId));

        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Site>.Filter.And(
            Builders<DbEntity.Site>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.Site>.Filter.Eq(x => x.TenantId, tenantId));

        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }
}

public class BuildingRepository(IMongoDatabase database) : IBuildingRepository
{
    private readonly IMongoCollection<DbEntity.Building> _collection = database.GetCollection<DbEntity.Building>("buildings");

    public async Task<DomainEntity.Building?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Building>.Filter.And(
            Builders<DbEntity.Building>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.Building>.Filter.Eq(x => x.TenantId, tenantId));

        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.Building>> GetAllAsync(string tenantId, string? siteId = null, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<DbEntity.Building>.Filter;
        var filter = filterBuilder.Eq(x => x.TenantId, tenantId);

        if (!string.IsNullOrEmpty(siteId))
        {
            filter = filterBuilder.And(filter, filterBuilder.Eq(x => x.SiteId, siteId));
        }

        var dbEntities = await _collection.Find(filter)
            .SortBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DomainEntity.Building> CreateAsync(DomainEntity.Building building, CancellationToken cancellationToken = default)
    {
        var dbEntity = building.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);
        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.Building building, CancellationToken cancellationToken = default)
    {
        var dbEntity = building.ToDatabase();
        var filter = Builders<DbEntity.Building>.Filter.And(
            Builders<DbEntity.Building>.Filter.Eq(x => x.Id, dbEntity.Id),
            Builders<DbEntity.Building>.Filter.Eq(x => x.TenantId, dbEntity.TenantId));

        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Building>.Filter.And(
            Builders<DbEntity.Building>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.Building>.Filter.Eq(x => x.TenantId, tenantId));

        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Building>.Filter.And(
            Builders<DbEntity.Building>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.Building>.Filter.Eq(x => x.TenantId, tenantId));

        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }
}

public class RoomRepository(IMongoDatabase database) : IRoomRepository
{
    private readonly IMongoCollection<DbEntity.Room> _collection = database.GetCollection<DbEntity.Room>("rooms");

    public async Task<DomainEntity.Room?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Room>.Filter.And(
            Builders<DbEntity.Room>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.Room>.Filter.Eq(x => x.TenantId, tenantId));

        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.Room>> GetAllAsync(string tenantId, string? siteId = null, string? buildingId = null, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<DbEntity.Room>.Filter;
        var filter = filterBuilder.Eq(x => x.TenantId, tenantId);

        // If siteId is provided, we need to filter rooms by buildings in that site
        if (!string.IsNullOrEmpty(siteId))
        {
            var buildingsCollection = database.GetCollection<DbEntity.Building>("buildings");
            var buildingFilterBuilder = Builders<DbEntity.Building>.Filter;
            var buildingFilter = buildingFilterBuilder.And(
                buildingFilterBuilder.Eq(x => x.TenantId, tenantId),
                buildingFilterBuilder.Eq(x => x.SiteId, siteId)
            );
            var buildingsInSite = await buildingsCollection.Find(buildingFilter).ToListAsync(cancellationToken);
            var buildingIds = buildingsInSite.Select(b => b.Id).ToList();
            
            if (buildingIds.Any())
            {
                filter = filterBuilder.And(filter, filterBuilder.In(x => x.BuildingId, buildingIds));
            }
            else
            {
                // No buildings in this site, return empty list
                return new List<DomainEntity.Room>();
            }
        }

        if (!string.IsNullOrEmpty(buildingId))
        {
            filter = filterBuilder.And(filter, filterBuilder.Eq(x => x.BuildingId, buildingId));
        }

        var dbEntities = await _collection.Find(filter)
            .SortBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DomainEntity.Room> CreateAsync(DomainEntity.Room room, CancellationToken cancellationToken = default)
    {
        var dbEntity = room.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);
        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.Room room, CancellationToken cancellationToken = default)
    {
        var dbEntity = room.ToDatabase();
        var filter = Builders<DbEntity.Room>.Filter.And(
            Builders<DbEntity.Room>.Filter.Eq(x => x.Id, dbEntity.Id),
            Builders<DbEntity.Room>.Filter.Eq(x => x.TenantId, dbEntity.TenantId));

        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Room>.Filter.And(
            Builders<DbEntity.Room>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.Room>.Filter.Eq(x => x.TenantId, tenantId));

        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(string id, string tenantId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.Room>.Filter.And(
            Builders<DbEntity.Room>.Filter.Eq(x => x.Id, id),
            Builders<DbEntity.Room>.Filter.Eq(x => x.TenantId, tenantId));

        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        return count > 0;
    }
}
