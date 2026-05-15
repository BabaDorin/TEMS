using DomainEntity = TicketManagement.Application.Domain;
using DbEntity = TicketManagement.Infrastructure.Entities;
using TicketManagement.Contract.Responses;

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
            Title = dbEntity.Title,
            Summary = dbEntity.Summary,
            AiSummary = dbEntity.AiSummary,
            CurrentStateId = dbEntity.CurrentStateId,
            Priority = dbEntity.Priority,
            Reporter = dbEntity.Reporter.ToDomain(),
            AccountableUserId = dbEntity.AccountableUserId,
            AssigneeId = dbEntity.AssigneeId,
            Attributes = new Dictionary<string, object>(dbEntity.Attributes),
            AssetIds = (dbEntity.AssetIds ?? []).ToList(),
            ApprovalGates = (dbEntity.ApprovalGates ?? new List<DbEntity.ApprovalGate>()).Select(x => x.ToDomain()).ToList(),
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
            Title = domainEntity.Title,
            Summary = domainEntity.Summary,
            AiSummary = domainEntity.AiSummary,
            CurrentStateId = domainEntity.CurrentStateId,
            Priority = domainEntity.Priority,
            Reporter = domainEntity.Reporter.ToDatabase(),
            AccountableUserId = domainEntity.AccountableUserId,
            AssigneeId = domainEntity.AssigneeId,
            Attributes = new Dictionary<string, object>(domainEntity.Attributes),
            AssetIds = (domainEntity.AssetIds ?? []).ToList(),
            ApprovalGates = (domainEntity.ApprovalGates ?? new List<DomainEntity.ApprovalGate>()).Select(x => x.ToDatabase()).ToList(),
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

    public static DomainEntity.ApprovalGate ToDomain(this DbEntity.ApprovalGate dbEntity)
    {
        return new DomainEntity.ApprovalGate
        {
            ApprovalGateId = dbEntity.ApprovalGateId,
            Title = dbEntity.Title,
            Justification = dbEntity.Justification,
            State = dbEntity.State,
            AllApproversRequired = dbEntity.AllApproversRequired,
            Approvers = dbEntity.Approvers.Select(x => x.ToDomain()).ToList(),
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt
        };
    }

    public static DbEntity.ApprovalGate ToDatabase(this DomainEntity.ApprovalGate domainEntity)
    {
        return new DbEntity.ApprovalGate
        {
            ApprovalGateId = domainEntity.ApprovalGateId,
            Title = domainEntity.Title,
            Justification = domainEntity.Justification,
            State = domainEntity.State,
            AllApproversRequired = domainEntity.AllApproversRequired,
            Approvers = domainEntity.Approvers.Select(x => x.ToDatabase()).ToList(),
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt
        };
    }

    public static DomainEntity.ApprovalGateApprover ToDomain(this DbEntity.ApprovalGateApprover dbEntity)
    {
        return new DomainEntity.ApprovalGateApprover
        {
            UserId = dbEntity.UserId,
            Status = dbEntity.Status,
            ReviewedAt = dbEntity.ReviewedAt
        };
    }

    public static DbEntity.ApprovalGateApprover ToDatabase(this DomainEntity.ApprovalGateApprover domainEntity)
    {
        return new DbEntity.ApprovalGateApprover
        {
            UserId = domainEntity.UserId,
            Status = domainEntity.Status,
            ReviewedAt = domainEntity.ReviewedAt
        };
    }

    public static ApprovalGateResponse ToResponse(this DomainEntity.ApprovalGate domainEntity)
    {
        return new ApprovalGateResponse(
            domainEntity.ApprovalGateId,
            domainEntity.Title,
            domainEntity.Justification,
            domainEntity.State,
            domainEntity.AllApproversRequired,
            domainEntity.Approvers.Select(x => x.ToResponse()).ToList(),
            domainEntity.CreatedAt,
            domainEntity.UpdatedAt
        );
    }

    public static ApprovalGateApproverResponse ToResponse(this DomainEntity.ApprovalGateApprover domainEntity)
    {
        return new ApprovalGateApproverResponse(
            domainEntity.UserId,
            domainEntity.Status,
            domainEntity.ReviewedAt
        );
    }
}
