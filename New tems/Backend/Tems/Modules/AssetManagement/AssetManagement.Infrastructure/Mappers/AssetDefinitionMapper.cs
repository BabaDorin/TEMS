using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Mappers;

public static class AssetDefinitionMapper
{
    public static DomainEntity.AssetDefinition ToDomain(this DbEntity.AssetDefinition dbEntity)
    {
        return new DomainEntity.AssetDefinition
        {
            Id = dbEntity.Id,
            Name = dbEntity.Name,
            ShortName = dbEntity.ShortName,
            AssetTypeId = dbEntity.AssetTypeId,
            AssetTypeName = dbEntity.AssetTypeName,
            Manufacturer = dbEntity.Manufacturer,
            Model = dbEntity.Model,
            Specifications = dbEntity.Specifications.Select(s => new DomainEntity.AssetSpecification
            {
                PropertyId = s.PropertyId,
                Name = s.Name,
                Value = s.Value,
                DataType = s.DataType,
                Unit = s.Unit
            }).ToList(),
            UsageCount = dbEntity.UsageCount,
            Description = dbEntity.Description,
            Notes = dbEntity.Notes,
            Tags = dbEntity.Tags,
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt,
            CreatedBy = dbEntity.CreatedBy,
            IsArchived = dbEntity.IsArchived,
            ArchivedAt = dbEntity.ArchivedAt,
            ArchivedBy = dbEntity.ArchivedBy
        };
    }

    public static DbEntity.AssetDefinition ToDatabase(this DomainEntity.AssetDefinition domainEntity)
    {
        return new DbEntity.AssetDefinition
        {
            Id = domainEntity.Id,
            Name = domainEntity.Name,
            ShortName = domainEntity.ShortName,
            AssetTypeId = domainEntity.AssetTypeId,
            AssetTypeName = domainEntity.AssetTypeName,
            Manufacturer = domainEntity.Manufacturer,
            Model = domainEntity.Model,
            Specifications = domainEntity.Specifications.Select(s => new DbEntity.AssetSpecification
            {
                PropertyId = s.PropertyId,
                Name = s.Name,
                Value = s.Value,
                DataType = s.DataType,
                Unit = s.Unit
            }).ToList(),
            UsageCount = domainEntity.UsageCount,
            Description = domainEntity.Description,
            Notes = domainEntity.Notes,
            Tags = domainEntity.Tags,
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt,
            CreatedBy = domainEntity.CreatedBy,
            IsArchived = domainEntity.IsArchived,
            ArchivedAt = domainEntity.ArchivedAt,
            ArchivedBy = domainEntity.ArchivedBy
        };
    }
}
