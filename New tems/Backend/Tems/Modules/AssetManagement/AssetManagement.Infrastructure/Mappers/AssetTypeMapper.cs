using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Mappers;

public static class AssetTypeMapper
{
    public static DomainEntity.AssetType ToDomain(this DbEntity.AssetType dbEntity)
    {
        return new DomainEntity.AssetType
        {
            Id = dbEntity.Id,
            Name = dbEntity.Name,
            Description = dbEntity.Description,
            ParentTypeId = dbEntity.ParentTypeId,
            Properties = dbEntity.Properties.Select(p => p.ToDomain()).ToList(),
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt,
            CreatedBy = dbEntity.CreatedBy,
            IsArchived = dbEntity.IsArchived,
            ArchivedAt = dbEntity.ArchivedAt,
            ArchivedBy = dbEntity.ArchivedBy
        };
    }

    public static DbEntity.AssetType ToDatabase(this DomainEntity.AssetType domainEntity)
    {
        return new DbEntity.AssetType
        {
            Id = domainEntity.Id,
            Name = domainEntity.Name,
            Description = domainEntity.Description,
            ParentTypeId = domainEntity.ParentTypeId,
            Properties = domainEntity.Properties.Select(p => p.ToDatabase()).ToList(),
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt,
            CreatedBy = domainEntity.CreatedBy,
            IsArchived = domainEntity.IsArchived,
            ArchivedAt = domainEntity.ArchivedAt,
            ArchivedBy = domainEntity.ArchivedBy
        };
    }

    public static DomainEntity.AssetTypeProperty ToDomain(this DbEntity.AssetTypeProperty dbEntity)
    {
        return new DomainEntity.AssetTypeProperty
        {
            PropertyId = dbEntity.PropertyId,
            Name = dbEntity.Name,
            Description = dbEntity.Description,
            DataType = dbEntity.DataType,
            Required = dbEntity.Required,
            Validation = dbEntity.Validation?.ToDomain(),
            DisplayOrder = dbEntity.DisplayOrder
        };
    }

    public static DbEntity.AssetTypeProperty ToDatabase(this DomainEntity.AssetTypeProperty domainEntity)
    {
        return new DbEntity.AssetTypeProperty
        {
            PropertyId = domainEntity.PropertyId,
            Name = domainEntity.Name,
            Description = domainEntity.Description,
            DataType = domainEntity.DataType,
            Required = domainEntity.Required,
            Validation = domainEntity.Validation?.ToDatabase(),
            DisplayOrder = domainEntity.DisplayOrder
        };
    }
}
