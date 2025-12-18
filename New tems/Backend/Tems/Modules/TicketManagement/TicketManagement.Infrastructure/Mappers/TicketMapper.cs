using DomainEntity = TicketManagement.Application.Domain;
using DbEntity = TicketManagement.Infrastructure.Entities;

namespace TicketManagement.Infrastructure.Mappers;

public static class TicketMapper
{
    public static DomainEntity.Ticket ToDomain(this DbEntity.Ticket dbEntity)
    {
        return new DomainEntity.Ticket
        {
            TicketId = dbEntity.TicketId,
            TenantId = dbEntity.TenantId,
            TicketTypeId = dbEntity.TicketTypeId,
            HumanReadableId = dbEntity.HumanReadableId,
            Summary = dbEntity.Summary,
            CurrentStateId = dbEntity.CurrentStateId,
            Priority = dbEntity.Priority,
            Reporter = dbEntity.Reporter.ToDomain(),
            AssigneeId = dbEntity.AssigneeId,
            Attributes = new Dictionary<string, object>(dbEntity.Attributes),
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt,
            ResolvedAt = dbEntity.ResolvedAt
        };
    }

    public static DbEntity.Ticket ToDatabase(this DomainEntity.Ticket domainEntity)
    {
        return new DbEntity.Ticket
        {
            TicketId = domainEntity.TicketId,
            TenantId = domainEntity.TenantId,
            TicketTypeId = domainEntity.TicketTypeId,
            HumanReadableId = domainEntity.HumanReadableId,
            Summary = domainEntity.Summary,
            CurrentStateId = domainEntity.CurrentStateId,
            Priority = domainEntity.Priority,
            Reporter = domainEntity.Reporter.ToDatabase(),
            AssigneeId = domainEntity.AssigneeId,
            Attributes = new Dictionary<string, object>(domainEntity.Attributes),
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt,
            ResolvedAt = domainEntity.ResolvedAt
        };
    }

    public static DomainEntity.Reporter ToDomain(this DbEntity.Reporter dbEntity)
    {
        return new DomainEntity.Reporter
        {
            UserId = dbEntity.UserId,
            ChannelSource = dbEntity.ChannelSource,
            ChannelThreadId = dbEntity.ChannelThreadId
        };
    }

    public static DbEntity.Reporter ToDatabase(this DomainEntity.Reporter domainEntity)
    {
        return new DbEntity.Reporter
        {
            UserId = domainEntity.UserId,
            ChannelSource = domainEntity.ChannelSource,
            ChannelThreadId = domainEntity.ChannelThreadId
        };
    }
}
