using MongoDB.Bson;
using DomainEntity = EquipmentManagement.Application.Domain;
using DbEntity = EquipmentManagement.Infrastructure.Entities;

namespace EquipmentManagement.Infrastructure.Mappers;

public static class EquipmentPropertyMapper
{
    public static DomainEntity.EquipmentProperty ToDomain(this DbEntity.EquipmentProperty dbEntity)
    {
        return new DomainEntity.EquipmentProperty
        {
            PropertyId = dbEntity.PropertyId,
            Name = dbEntity.Name,
            Description = dbEntity.Description,
            DataType = dbEntity.DataType,
            Required = dbEntity.Required,
            Validation = dbEntity.Validation?.ToDomain(),
            DisplayOrder = dbEntity.DisplayOrder,
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt
        };
    }

    public static DbEntity.EquipmentProperty ToDatabase(this DomainEntity.EquipmentProperty domainEntity)
    {
        return new DbEntity.EquipmentProperty
        {
            PropertyId = domainEntity.PropertyId,
            Name = domainEntity.Name,
            Description = domainEntity.Description,
            DataType = domainEntity.DataType,
            Required = domainEntity.Required,
            Validation = domainEntity.Validation?.ToDatabase(),
            DisplayOrder = domainEntity.DisplayOrder,
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt
        };
    }

    public static DomainEntity.PropertyValidation ToDomain(this DbEntity.PropertyValidation dbEntity)
    {
        return new DomainEntity.PropertyValidation
        {
            Type = dbEntity.Type,
            MaxLength = dbEntity.MaxLength,
            Pattern = dbEntity.Pattern,
            MinValue = dbEntity.MinValue,
            MaxValue = dbEntity.MaxValue,
            AllowedValues = dbEntity.AllowedValues
        };
    }

    public static DbEntity.PropertyValidation ToDatabase(this DomainEntity.PropertyValidation domainEntity)
    {
        return new DbEntity.PropertyValidation
        {
            Type = domainEntity.Type,
            MaxLength = domainEntity.MaxLength,
            Pattern = domainEntity.Pattern,
            MinValue = domainEntity.MinValue,
            MaxValue = domainEntity.MaxValue,
            AllowedValues = domainEntity.AllowedValues
        };
    }
}
