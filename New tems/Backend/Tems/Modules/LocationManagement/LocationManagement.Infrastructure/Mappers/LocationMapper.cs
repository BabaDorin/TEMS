using DomainEntity = LocationManagement.Application.Domain;
using DbEntity = LocationManagement.Infrastructure.Entities;

namespace LocationManagement.Infrastructure.Mappers;

public static class LocationMapper
{
    public static DomainEntity.Site ToDomain(this DbEntity.Site dbEntity)
    {
        return new DomainEntity.Site
        {
            Id = dbEntity.Id,
            Name = dbEntity.Name,
            Code = dbEntity.Code,
            Timezone = dbEntity.Timezone,
            IsActive = dbEntity.IsActive,
            TenantId = dbEntity.TenantId,
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt,
            CreatedBy = dbEntity.CreatedBy,
            UpdatedBy = dbEntity.UpdatedBy
        };
    }

    public static DbEntity.Site ToDatabase(this DomainEntity.Site domainEntity)
    {
        return new DbEntity.Site
        {
            Id = domainEntity.Id,
            Name = domainEntity.Name,
            Code = domainEntity.Code,
            Timezone = domainEntity.Timezone,
            IsActive = domainEntity.IsActive,
            TenantId = domainEntity.TenantId,
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt,
            CreatedBy = domainEntity.CreatedBy,
            UpdatedBy = domainEntity.UpdatedBy
        };
    }

    public static DomainEntity.Building ToDomain(this DbEntity.Building dbEntity)
    {
        return new DomainEntity.Building
        {
            Id = dbEntity.Id,
            SiteId = dbEntity.SiteId,
            Name = dbEntity.Name,
            AddressLine = dbEntity.AddressLine,
            ManagerContact = dbEntity.ManagerContact,
            TenantId = dbEntity.TenantId,
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt,
            CreatedBy = dbEntity.CreatedBy,
            UpdatedBy = dbEntity.UpdatedBy
        };
    }

    public static DbEntity.Building ToDatabase(this DomainEntity.Building domainEntity)
    {
        return new DbEntity.Building
        {
            Id = domainEntity.Id,
            SiteId = domainEntity.SiteId,
            Name = domainEntity.Name,
            AddressLine = domainEntity.AddressLine,
            ManagerContact = domainEntity.ManagerContact,
            TenantId = domainEntity.TenantId,
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt,
            CreatedBy = domainEntity.CreatedBy,
            UpdatedBy = domainEntity.UpdatedBy
        };
    }

    public static DomainEntity.Room ToDomain(this DbEntity.Room dbEntity)
    {
        return new DomainEntity.Room
        {
            Id = dbEntity.Id,
            BuildingId = dbEntity.BuildingId,
            Name = dbEntity.Name,
            RoomNumber = dbEntity.RoomNumber,
            FloorLabel = dbEntity.FloorLabel,
            Type = Enum.Parse<DomainEntity.RoomType>(dbEntity.Type, true),
            Capacity = dbEntity.Capacity,
            Area = dbEntity.Area,
            Status = Enum.Parse<DomainEntity.RoomStatus>(dbEntity.Status, true),
            Description = dbEntity.Description,
            TenantId = dbEntity.TenantId,
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt,
            CreatedBy = dbEntity.CreatedBy,
            UpdatedBy = dbEntity.UpdatedBy
        };
    }

    public static DbEntity.Room ToDatabase(this DomainEntity.Room domainEntity)
    {
        return new DbEntity.Room
        {
            Id = domainEntity.Id,
            BuildingId = domainEntity.BuildingId,
            Name = domainEntity.Name,
            RoomNumber = domainEntity.RoomNumber,
            FloorLabel = domainEntity.FloorLabel,
            Type = domainEntity.Type.ToString(),
            Capacity = domainEntity.Capacity,
            Area = domainEntity.Area,
            Status = domainEntity.Status.ToString(),
            Description = domainEntity.Description,
            TenantId = domainEntity.TenantId,
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt,
            CreatedBy = domainEntity.CreatedBy,
            UpdatedBy = domainEntity.UpdatedBy
        };
    }
}
