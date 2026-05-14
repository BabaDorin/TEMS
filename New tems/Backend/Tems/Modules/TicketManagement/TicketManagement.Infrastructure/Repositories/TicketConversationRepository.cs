using TicketManagement.Application.Interfaces;
using TicketManagement.Infrastructure.Mappers;
using MongoDB.Driver;
using MongoDB.Bson;
using DomainEntity = TicketManagement.Application.Domain;
using DbEntity = TicketManagement.Infrastructure.Entities;

namespace TicketManagement.Infrastructure.Repositories;

public class TicketConversationRepository : ITicketConversationRepository
{
    private readonly IMongoCollection<DbEntity.TicketConversation> _collection;

    public TicketConversationRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<DbEntity.TicketConversation>("ticket_conversations");

        var ticketIndex = Builders<DbEntity.TicketConversation>.IndexKeys.Ascending(x => x.TicketId);
        var ticketIndexOptions = new CreateIndexOptions { Unique = true };
        _collection.Indexes.CreateOneAsync(new CreateIndexModel<DbEntity.TicketConversation>(ticketIndex, ticketIndexOptions));
    }

    public async Task<DomainEntity.TicketConversation?> GetByTicketIdAsync(string ticketId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketConversation>.Filter.Eq(x => x.TicketId, ticketId);
        var dbEntity = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (dbEntity == null)
            return null;

        var hasLegacyMessages = dbEntity.Messages.Any(m => string.IsNullOrWhiteSpace(m.MessageId));
        if (hasLegacyMessages)
        {
            foreach (var message in dbEntity.Messages.Where(m => string.IsNullOrWhiteSpace(m.MessageId)))
                message.MessageId = ObjectId.GenerateNewId().ToString();

            var backfillUpdate = Builders<DbEntity.TicketConversation>.Update
                .Set(x => x.Messages, dbEntity.Messages)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);
            await _collection.UpdateOneAsync(filter, backfillUpdate, cancellationToken: cancellationToken);
        }

        return dbEntity.ToDomain();
    }

    public async Task<DomainEntity.TicketConversation> CreateAsync(DomainEntity.TicketConversation conversation, CancellationToken cancellationToken = default)
    {
        conversation.CreatedAt = DateTime.UtcNow;
        conversation.UpdatedAt = DateTime.UtcNow;

        var dbEntity = conversation.ToDatabase();
        await _collection.InsertOneAsync(dbEntity, cancellationToken: cancellationToken);

        return dbEntity.ToDomain();
    }

    public async Task<bool> AddMessageAsync(string ticketId, DomainEntity.TicketMessage message, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketConversation>.Filter.Eq(x => x.TicketId, ticketId);
        var update = Builders<DbEntity.TicketConversation>.Update
            .Push(x => x.Messages, message.ToDatabase())
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task<DomainEntity.TicketMessage?> EditMessageAsync(
        string ticketId,
        string messageId,
        string content,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketConversation>.Filter.And(
            Builders<DbEntity.TicketConversation>.Filter.Eq(x => x.TicketId, ticketId),
            Builders<DbEntity.TicketConversation>.Filter.ElemMatch(x => x.Messages, m => m.MessageId == messageId)
        );

        var editedAt = DateTime.UtcNow;
        var update = Builders<DbEntity.TicketConversation>.Update
            .Set("messages.$.content", content)
            .Set("messages.$.edited_at", editedAt)
            .Set(x => x.UpdatedAt, editedAt);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        if (result.ModifiedCount == 0)
            return null;

        var conversation = await GetByTicketIdAsync(ticketId, cancellationToken);
        return conversation?.Messages.FirstOrDefault(m => m.MessageId == messageId);
    }

    public async Task<bool> DeleteMessageAsync(string ticketId, string messageId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketConversation>.Filter.And(
            Builders<DbEntity.TicketConversation>.Filter.Eq(x => x.TicketId, ticketId),
            Builders<DbEntity.TicketConversation>.Filter.ElemMatch(x => x.Messages, m => m.MessageId == messageId)
        );

        var update = Builders<DbEntity.TicketConversation>.Update
            .PullFilter(x => x.Messages, m => m.MessageId == messageId)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteByTicketIdAsync(string ticketId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketConversation>.Filter.Eq(x => x.TicketId, ticketId);
        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<long> DeleteByTicketIdsAsync(IEnumerable<string> ticketIds, CancellationToken cancellationToken = default)
    {
        var ids = ticketIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (ids.Count == 0)
            return 0;

        var filter = Builders<DbEntity.TicketConversation>.Filter.In(x => x.TicketId, ids);
        var result = await _collection.DeleteManyAsync(filter, cancellationToken);
        return result.DeletedCount;
    }

    public async Task<bool> ExistsAsync(string ticketId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketConversation>.Filter.Eq(x => x.TicketId, ticketId);
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return count > 0;
    }
}
