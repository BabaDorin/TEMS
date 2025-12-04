using TicketManagement.Application.Interfaces;
using TicketManagement.Infrastructure.Mappers;
using MongoDB.Driver;
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
        return dbEntity?.ToDomain();
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

    public async Task<bool> ExistsAsync(string ticketId, CancellationToken cancellationToken = default)
    {
        var filter = Builders<DbEntity.TicketConversation>.Filter.Eq(x => x.TicketId, ticketId);
        var count = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        return count > 0;
    }
}
