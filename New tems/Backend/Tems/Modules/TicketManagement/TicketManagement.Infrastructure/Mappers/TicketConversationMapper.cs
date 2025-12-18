using DomainEntity = TicketManagement.Application.Domain;
using DbEntity = TicketManagement.Infrastructure.Entities;

namespace TicketManagement.Infrastructure.Mappers;

public static class TicketConversationMapper
{
    public static DomainEntity.TicketConversation ToDomain(this DbEntity.TicketConversation dbEntity)
    {
        return new DomainEntity.TicketConversation
        {
            ConversationId = dbEntity.ConversationId,
            TicketId = dbEntity.TicketId,
            Messages = dbEntity.Messages.Select(m => m.ToDomain()).ToList(),
            CreatedAt = dbEntity.CreatedAt,
            UpdatedAt = dbEntity.UpdatedAt
        };
    }

    public static DbEntity.TicketConversation ToDatabase(this DomainEntity.TicketConversation domainEntity)
    {
        return new DbEntity.TicketConversation
        {
            ConversationId = domainEntity.ConversationId,
            TicketId = domainEntity.TicketId,
            Messages = domainEntity.Messages.Select(m => m.ToDatabase()).ToList(),
            CreatedAt = domainEntity.CreatedAt,
            UpdatedAt = domainEntity.UpdatedAt
        };
    }

    public static DomainEntity.TicketMessage ToDomain(this DbEntity.TicketMessage dbEntity)
    {
        return new DomainEntity.TicketMessage
        {
            SenderType = dbEntity.SenderType,
            SenderId = dbEntity.SenderId,
            Timestamp = dbEntity.Timestamp,
            Content = dbEntity.Content,
            ChannelMessageId = dbEntity.ChannelMessageId,
            IsInternalNote = dbEntity.IsInternalNote
        };
    }

    public static DbEntity.TicketMessage ToDatabase(this DomainEntity.TicketMessage domainEntity)
    {
        return new DbEntity.TicketMessage
        {
            SenderType = domainEntity.SenderType,
            SenderId = domainEntity.SenderId,
            Timestamp = domainEntity.Timestamp,
            Content = domainEntity.Content,
            ChannelMessageId = domainEntity.ChannelMessageId,
            IsInternalNote = domainEntity.IsInternalNote
        };
    }
}
