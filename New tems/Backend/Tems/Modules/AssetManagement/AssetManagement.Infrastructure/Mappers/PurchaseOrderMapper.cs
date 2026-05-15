using DomainEntity = AssetManagement.Application.Domain;
using DbEntity = AssetManagement.Infrastructure.Entities;

namespace AssetManagement.Infrastructure.Mappers;

public static class PurchaseOrderMapper
{
    public static DomainEntity.PurchaseOrder ToDomain(this DbEntity.PurchaseOrder dbEntity)
    {
        return new DomainEntity.PurchaseOrder
        {
            Id = dbEntity.Id,
            TenantId = dbEntity.TenantId,
            TicketId = dbEntity.TicketId,
            TicketHumanReadableId = dbEntity.TicketHumanReadableId,
            PoNumber = dbEntity.PoNumber,
            Vendor = dbEntity.Vendor,
            Amount = dbEntity.Amount,
            Currency = dbEntity.Currency,
            Description = dbEntity.Description,
            CreatedByUserId = dbEntity.CreatedByUserId,
            AccountableUserId = dbEntity.AccountableUserId,
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt
        };
    }

    public static DbEntity.PurchaseOrder ToDatabase(this DomainEntity.PurchaseOrder domainEntity)
    {
        return new DbEntity.PurchaseOrder
        {
            Id = domainEntity.Id,
            TenantId = domainEntity.TenantId,
            TicketId = domainEntity.TicketId,
            TicketHumanReadableId = domainEntity.TicketHumanReadableId,
            PoNumber = domainEntity.PoNumber,
            Vendor = domainEntity.Vendor,
            Amount = domainEntity.Amount,
            Currency = domainEntity.Currency,
            Description = domainEntity.Description,
            CreatedByUserId = domainEntity.CreatedByUserId,
            AccountableUserId = domainEntity.AccountableUserId,
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt
        };
    }
}
