using EquipmentManagement.Application.Interfaces;
using EquipmentManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using DomainEntity = EquipmentManagement.Application.Domain;
using DbEntity = EquipmentManagement.Infrastructure.Entities;

namespace EquipmentManagement.Infrastructure.Repositories;

public class EquipmentPropertyRepository : IEquipmentPropertyRepository
{
    private readonly IMongoCollection<DbEntity.EquipmentProperty> _collection;

    public EquipmentPropertyRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<DbEntity.EquipmentProperty>("equipment_properties");

        var indexKeys = Builders<DbEntity.EquipmentProperty>.IndexKeys.Ascending(x => x.Name);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<DbEntity.EquipmentProperty>(indexKeys, indexOptions);

        _collection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<DomainEntity.EquipmentProperty?> GetByIdAsync(string propertyId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.EquipmentProperty>.Filter.Eq(x => x.PropertyId, propertyId);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<List<DomainEntity.EquipmentProperty>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var dbEntities = await _collection.Find(_ => true)
            .SortBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);

        return dbEntities.Select(x => x.ToDomain()).ToList();
    }

    public async Task<DomainEntity.EquipmentProperty?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.EquipmentProperty>.Filter.Eq(x => x.Name, name);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return dbEntity?.ToDomain();
    }

    public async Task<DomainEntity.EquipmentProperty> CreateAsync(DomainEntity.EquipmentProperty property, CancellationToken cancellationToken = default)
    {
        property.CreatedAt = DateTime.UtcNow;
        property.UpdatedAt = DateTime.UtcNow;

        var dbEntity = property.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);

        return dbEntity.ToDomain();
    }

    public async Task<bool> UpdateAsync(DomainEntity.EquipmentProperty property, CancellationToken cancellationToken = default)
    {
        property.UpdatedAt = DateTime.UtcNow;

        var dbEntity = property.ToDatabase();
        var filter = Builders<DbEntity.EquipmentProperty>.Filter.Eq(x => x.PropertyId, dbEntity.PropertyId);
        var result = await _collection.ReplaceOneAsync(filter, dbEntity, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string propertyId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.EquipmentProperty>.Filter.Eq(x => x.PropertyId, propertyId);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);

        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(string propertyId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.EquipmentProperty>.Filter.Eq(x => x.PropertyId, propertyId);
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return count > 0;
    }
}
