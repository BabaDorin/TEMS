using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Mappers;

public static class AssetMapper
{
    public static DomainEntity.Asset ToDomain(this DbEntity.Asset dbEntity)
    {
        return new DomainEntity.Asset
        {
            Id = dbEntity.Id,
            SerialNumber = dbEntity.SerialNumber,
            AssetTag = dbEntity.AssetTag,
            Status = dbEntity.Status,
            Definition = dbEntity.Definition.ToDomain(),
            PurchaseInfo = dbEntity.PurchaseInfo?.ToDomain(),
            LocationId = dbEntity.LocationId,
            Location = dbEntity.Location?.ToDomain(),
            Assignment = dbEntity.Assignment?.ToDomain(),
            ParentAssetId = dbEntity.ParentAssetId,
            ChildAssetIds = dbEntity.ChildAssetIds,
            Notes = dbEntity.Notes,
            MaintenanceHistory = dbEntity.MaintenanceHistory.Select(m => m.ToDomain()).ToList(),
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt,
            CreatedBy = dbEntity.CreatedBy,
            IsArchived = dbEntity.IsArchived,
            ArchivedAt = dbEntity.ArchivedAt,
            ArchivedBy = dbEntity.ArchivedBy
        };
    }

    public static DbEntity.Asset ToDatabase(this DomainEntity.Asset domainEntity)
    {
        return new DbEntity.Asset
        {
            Id = domainEntity.Id,
            SerialNumber = domainEntity.SerialNumber,
            AssetTag = domainEntity.AssetTag,
            Status = domainEntity.Status,
            Definition = domainEntity.Definition.ToDatabase(),
            PurchaseInfo = domainEntity.PurchaseInfo?.ToDatabase(),
            LocationId = domainEntity.LocationId,
            Location = domainEntity.Location?.ToDatabase(),
            Assignment = domainEntity.Assignment?.ToDatabase(),
            ParentAssetId = domainEntity.ParentAssetId,
            ChildAssetIds = domainEntity.ChildAssetIds,
            Notes = domainEntity.Notes,
            MaintenanceHistory = domainEntity.MaintenanceHistory.Select(m => m.ToDatabase()).ToList(),
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt,
            CreatedBy = domainEntity.CreatedBy,
            IsArchived = domainEntity.IsArchived,
            ArchivedAt = domainEntity.ArchivedAt,
            ArchivedBy = domainEntity.ArchivedBy
        };
    }

    public static DomainEntity.AssetDefinitionSnapshot ToDomain(this DbEntity.AssetDefinitionSnapshot dbEntity)
    {
        return new DomainEntity.AssetDefinitionSnapshot
        {
            DefinitionId = dbEntity.DefinitionId,
            IsCustomized = dbEntity.IsCustomized,
            SnapshotAt = dbEntity.SnapshotAt,
            Name = dbEntity.Name,
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
            }).ToList()
        };
    }

    public static DbEntity.AssetDefinitionSnapshot ToDatabase(this DomainEntity.AssetDefinitionSnapshot domainEntity)
    {
        return new DbEntity.AssetDefinitionSnapshot
        {
            DefinitionId = domainEntity.DefinitionId,
            IsCustomized = domainEntity.IsCustomized,
            SnapshotAt = domainEntity.SnapshotAt,
            Name = domainEntity.Name,
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
            }).ToList()
        };
    }

    public static DomainEntity.PurchaseInfo ToDomain(this DbEntity.PurchaseInfo dbEntity)
    {
        return new DomainEntity.PurchaseInfo
        {
            PurchaseDate = dbEntity.PurchaseDate,
            PurchasePrice = dbEntity.PurchasePrice,
            Currency = dbEntity.Currency,
            Vendor = dbEntity.Vendor,
            WarrantyExpiry = dbEntity.WarrantyExpiry
        };
    }

    public static DbEntity.PurchaseInfo ToDatabase(this DomainEntity.PurchaseInfo domainEntity)
    {
        return new DbEntity.PurchaseInfo
        {
            PurchaseDate = domainEntity.PurchaseDate,
            PurchasePrice = domainEntity.PurchasePrice,
            Currency = domainEntity.Currency,
            Vendor = domainEntity.Vendor,
            WarrantyExpiry = domainEntity.WarrantyExpiry
        };
    }

    public static DomainEntity.AssetLocation ToDomain(this DbEntity.AssetLocation dbEntity)
    {
        return new DomainEntity.AssetLocation
        {
            Building = dbEntity.Building,
            Room = dbEntity.Room,
            Desk = dbEntity.Desk
        };
    }

    public static DbEntity.AssetLocation ToDatabase(this DomainEntity.AssetLocation domainEntity)
    {
        return new DbEntity.AssetLocation
        {
            Building = domainEntity.Building,
            Room = domainEntity.Room,
            Desk = domainEntity.Desk
        };
    }

    public static DomainEntity.AssetAssignment ToDomain(this DbEntity.AssetAssignment dbEntity)
    {
        return new DomainEntity.AssetAssignment
        {
            AssignedToUserId = dbEntity.AssignedToUserId,
            AssignedToName = dbEntity.AssignedToName,
            AssignedAt = dbEntity.AssignedAt,
            AssignmentType = dbEntity.AssignmentType
        };
    }

    public static DbEntity.AssetAssignment ToDatabase(this DomainEntity.AssetAssignment domainEntity)
    {
        return new DbEntity.AssetAssignment
        {
            AssignedToUserId = domainEntity.AssignedToUserId,
            AssignedToName = domainEntity.AssignedToName,
            AssignedAt = domainEntity.AssignedAt,
            AssignmentType = domainEntity.AssignmentType
        };
    }

    public static DomainEntity.MaintenanceRecord ToDomain(this DbEntity.MaintenanceRecord dbEntity)
    {
        return new DomainEntity.MaintenanceRecord
        {
            Date = dbEntity.Date,
            Type = dbEntity.Type,
            Description = dbEntity.Description,
            PerformedBy = dbEntity.PerformedBy,
            Cost = dbEntity.Cost
        };
    }

    public static DbEntity.MaintenanceRecord ToDatabase(this DomainEntity.MaintenanceRecord domainEntity)
    {
        return new DbEntity.MaintenanceRecord
        {
            Date = domainEntity.Date,
            Type = domainEntity.Type,
            Description = domainEntity.Description,
            PerformedBy = domainEntity.PerformedBy,
            Cost = domainEntity.Cost
        };
    }
}
