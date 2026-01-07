using MongoDB.Bson;
using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Mappers;

public static class AssetPropertyMapper
{
    public static DomainEntity.AssetProperty ToDomain(this DbEntity.AssetProperty dbEntity)
    {
        return new DomainEntity.AssetProperty
        {
            PropertyId = dbEntity.PropertyId,
            Name = dbEntity.Name,
            Description = dbEntity.Description,
            DataType = dbEntity.DataType,
            Category = dbEntity.Category,
            DefaultValidation = dbEntity.DefaultValidation?.ToDomain(),
            EnumValues = dbEntity.EnumValues,
            Unit = dbEntity.Unit,
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt,
            CreatedBy = dbEntity.CreatedBy
        };
    }

    public static DbEntity.AssetProperty ToDatabase(this DomainEntity.AssetProperty domainEntity)
    {
        return new DbEntity.AssetProperty
        {
            PropertyId = domainEntity.PropertyId,
            Name = domainEntity.Name,
            Description = domainEntity.Description,
            DataType = domainEntity.DataType,
            Category = domainEntity.Category,
            DefaultValidation = domainEntity.DefaultValidation?.ToDatabase(),
            EnumValues = domainEntity.EnumValues,
            Unit = domainEntity.Unit,
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt,
            CreatedBy = domainEntity.CreatedBy
        };
    }

    public static DomainEntity.PropertyValidation ToDomain(this DbEntity.PropertyValidation dbEntity)
    {
        return new DomainEntity.PropertyValidation
        {
            Type = dbEntity.Type,
            MaxLength = dbEntity.MaxLength,
            Pattern = dbEntity.Pattern,
            Min = dbEntity.Min,
            Max = dbEntity.Max,
            Unit = dbEntity.Unit,
            EnumValues = dbEntity.EnumValues
        };
    }

    public static DbEntity.PropertyValidation ToDatabase(this DomainEntity.PropertyValidation domainEntity)
    {
        return new DbEntity.PropertyValidation
        {
            Type = domainEntity.Type,
            MaxLength = domainEntity.MaxLength,
            Pattern = domainEntity.Pattern,
            Min = domainEntity.Min,
            Max = domainEntity.Max,
            Unit = domainEntity.Unit,
            EnumValues = domainEntity.EnumValues
        };
    }
}
