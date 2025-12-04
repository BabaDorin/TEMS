using TicketManagement.Application.Domain.Enums;
using DomainEntity = TicketManagement.Application.Domain;
using DbEntity = TicketManagement.Infrastructure.Entities;

namespace TicketManagement.Infrastructure.Mappers;

public static class TicketTypeMapper
{
    public static DomainEntity.TicketType ToDomain(this DbEntity.TicketType dbEntity)
    {
        return new DomainEntity.TicketType
        {
            TicketTypeId = dbEntity.TicketTypeId,
            TenantId = dbEntity.TenantId,
            Name = dbEntity.Name,
            Description = dbEntity.Description,
            ItilCategory = dbEntity.ItilCategory,
            Version = dbEntity.Version,
            WorkflowConfig = dbEntity.WorkflowConfig.ToDomain(),
            AttributeDefinitions = dbEntity.AttributeDefinitions.Select(a => a.ToDomain()).ToList(),
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt
        };
    }

    public static DbEntity.TicketType ToDatabase(this DomainEntity.TicketType domainEntity)
    {
        return new DbEntity.TicketType
        {
            TicketTypeId = domainEntity.TicketTypeId,
            TenantId = domainEntity.TenantId,
            Name = domainEntity.Name,
            Description = domainEntity.Description,
            ItilCategory = domainEntity.ItilCategory,
            Version = domainEntity.Version,
            WorkflowConfig = domainEntity.WorkflowConfig.ToDatabase(),
            AttributeDefinitions = domainEntity.AttributeDefinitions.Select(a => a.ToDatabase()).ToList(),
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt
        };
    }

    public static DomainEntity.WorkflowConfig ToDomain(this DbEntity.WorkflowConfig dbEntity)
    {
        return new DomainEntity.WorkflowConfig
        {
            States = dbEntity.States.Select(s => s.ToDomain()).ToList(),
            InitialStateId = dbEntity.InitialStateId
        };
    }

    public static DbEntity.WorkflowConfig ToDatabase(this DomainEntity.WorkflowConfig domainEntity)
    {
        return new DbEntity.WorkflowConfig
        {
            States = domainEntity.States.Select(s => s.ToDatabase()).ToList(),
            InitialStateId = domainEntity.InitialStateId
        };
    }

    public static DomainEntity.WorkflowState ToDomain(this DbEntity.WorkflowState dbEntity)
    {
        return new DomainEntity.WorkflowState
        {
            Id = dbEntity.Id,
            Label = dbEntity.Label,
            Type = dbEntity.Type,
            AllowedTransitions = dbEntity.AllowedTransitions,
            AutomationHook = dbEntity.AutomationHook
        };
    }

    public static DbEntity.WorkflowState ToDatabase(this DomainEntity.WorkflowState domainEntity)
    {
        return new DbEntity.WorkflowState
        {
            Id = domainEntity.Id,
            Label = domainEntity.Label,
            Type = domainEntity.Type,
            AllowedTransitions = domainEntity.AllowedTransitions,
            AutomationHook = domainEntity.AutomationHook
        };
    }

    public static DomainEntity.AttributeDefinition ToDomain(this DbEntity.AttributeDefinition dbEntity)
    {
        return new DomainEntity.AttributeDefinition
        {
            Key = dbEntity.Key,
            Label = dbEntity.Label,
            DataType = dbEntity.DataType,
            IsRequired = dbEntity.IsRequired,
            IsPredefined = dbEntity.IsPredefined,
            AiExtractionHint = dbEntity.AiExtractionHint,
            ValidationRule = dbEntity.ValidationRule
        };
    }

    public static DbEntity.AttributeDefinition ToDatabase(this DomainEntity.AttributeDefinition domainEntity)
    {
        return new DbEntity.AttributeDefinition
        {
            Key = domainEntity.Key,
            Label = domainEntity.Label,
            DataType = domainEntity.DataType,
            IsRequired = domainEntity.IsRequired,
            IsPredefined = domainEntity.IsPredefined,
            AiExtractionHint = domainEntity.AiExtractionHint,
            ValidationRule = domainEntity.ValidationRule
        };
    }
}
