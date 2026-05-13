using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Tems.Host.Services;

public interface IAiSupportConversationRepository
{
    Task<AiSupportConversation?> GetByIdAsync(string conversationId, string tenantId, string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AiSupportConversationSummary>> GetSummariesAsync(string tenantId, string userId, CancellationToken cancellationToken = default);
    Task<AiSupportConversation> CreateAsync(AiSupportConversation conversation, CancellationToken cancellationToken = default);
    Task<bool> AddMessageAsync(string conversationId, string tenantId, string userId, AiSupportConversationMessage message, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string conversationId, string tenantId, string userId, CancellationToken cancellationToken = default);
}

public sealed class AiSupportConversationRepository : IAiSupportConversationRepository
{
    private static int _indexesInitialized;
    private readonly IMongoCollection<AiSupportConversationDocument> _collection;

    public AiSupportConversationRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<AiSupportConversationDocument>("ai_support_conversations");

        if (Interlocked.Exchange(ref _indexesInitialized, 1) == 0)
        {
            var ownerUpdatedIndex = Builders<AiSupportConversationDocument>.IndexKeys
                .Ascending(x => x.TenantId)
                .Ascending(x => x.UserId)
                .Descending(x => x.UpdatedAt);

            _collection.Indexes.CreateOne(new CreateIndexModel<AiSupportConversationDocument>(ownerUpdatedIndex));
        }
    }

    public async Task<AiSupportConversation?> GetByIdAsync(
        string conversationId,
        string tenantId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<AiSupportConversationDocument>.Filter.And(
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.ConversationId, conversationId),
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.TenantId, tenantId),
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.UserId, userId));

        var document = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return document?.ToDomain();
    }

    public async Task<IReadOnlyList<AiSupportConversationSummary>> GetSummariesAsync(
        string tenantId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<AiSupportConversationDocument>.Filter.And(
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.TenantId, tenantId),
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.UserId, userId));

        var documents = await _collection
            .Find(filter)
            .SortByDescending(x => x.UpdatedAt)
            .ToListAsync(cancellationToken);

        return documents.Select(x => x.ToSummary()).ToList();
    }

    public async Task<AiSupportConversation> CreateAsync(
        AiSupportConversation conversation,
        CancellationToken cancellationToken = default)
    {
        conversation.CreatedAt = DateTime.UtcNow;
        conversation.UpdatedAt = conversation.CreatedAt;

        var document = conversation.ToDocument();
        await _collection.InsertOneAsync(document, cancellationToken: cancellationToken);

        return document.ToDomain();
    }

    public async Task<bool> AddMessageAsync(
        string conversationId,
        string tenantId,
        string userId,
        AiSupportConversationMessage message,
        CancellationToken cancellationToken = default)
    {
        message.CreatedAt = message.CreatedAt == default ? DateTime.UtcNow : message.CreatedAt;

        var filter = Builders<AiSupportConversationDocument>.Filter.And(
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.ConversationId, conversationId),
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.TenantId, tenantId),
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.UserId, userId));

        var now = DateTime.UtcNow;
        var update = Builders<AiSupportConversationDocument>.Update
            .Push(x => x.Messages, message.ToDocument())
            .Set(x => x.UpdatedAt, now);

        var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(
        string conversationId,
        string tenantId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<AiSupportConversationDocument>.Filter.And(
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.ConversationId, conversationId),
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.TenantId, tenantId),
            Builders<AiSupportConversationDocument>.Filter.Eq(x => x.UserId, userId));

        var result = await _collection.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }
}

public sealed class AiSupportConversation
{
    public string ConversationId { get; set; } = ObjectId.GenerateNewId().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<AiSupportConversationMessage> Messages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public sealed class AiSupportConversationMessage
{
    public string MessageId { get; set; } = ObjectId.GenerateNewId().ToString();
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public sealed record AiSupportConversationSummary(
    string ConversationId,
    string Title,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int MessageCount);

[BsonIgnoreExtraElements]
internal sealed class AiSupportConversationDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string ConversationId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("tenant_id")]
    public string TenantId { get; set; } = string.Empty;

    [BsonElement("user_id")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("messages")]
    public List<AiSupportConversationMessageDocument> Messages { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

[BsonIgnoreExtraElements]
internal sealed class AiSupportConversationMessageDocument
{
    [BsonElement("message_id")]
    [BsonRepresentation(BsonType.String)]
    public string MessageId { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("role")]
    public string Role { get; set; } = string.Empty;

    [BsonElement("content")]
    public string Content { get; set; } = string.Empty;

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

internal static class AiSupportConversationMappingExtensions
{
    public static AiSupportConversation ToDomain(this AiSupportConversationDocument document)
    {
        return new AiSupportConversation
        {
            ConversationId = document.ConversationId,
            TenantId = document.TenantId,
            UserId = document.UserId,
            Title = document.Title,
            Messages = document.Messages.Select(x => x.ToDomain()).ToList(),
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }

    public static AiSupportConversationSummary ToSummary(this AiSupportConversationDocument document)
    {
        return new AiSupportConversationSummary(
            document.ConversationId,
            document.Title,
            document.CreatedAt,
            document.UpdatedAt,
            document.Messages.Count);
    }

    public static AiSupportConversationDocument ToDocument(this AiSupportConversation conversation)
    {
        return new AiSupportConversationDocument
        {
            ConversationId = conversation.ConversationId,
            TenantId = conversation.TenantId,
            UserId = conversation.UserId,
            Title = conversation.Title,
            Messages = conversation.Messages.Select(x => x.ToDocument()).ToList(),
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt
        };
    }

    public static AiSupportConversationMessage ToDomain(this AiSupportConversationMessageDocument document)
    {
        return new AiSupportConversationMessage
        {
            MessageId = document.MessageId,
            Role = document.Role,
            Content = document.Content,
            CreatedAt = document.CreatedAt
        };
    }

    public static AiSupportConversationMessageDocument ToDocument(this AiSupportConversationMessage message)
    {
        return new AiSupportConversationMessageDocument
        {
            MessageId = message.MessageId,
            Role = message.Role,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        };
    }
}
