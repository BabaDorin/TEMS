using MongoDB.Bson.Serialization.Attributes;
using UserManagement.Infrastructure.Serialization;

namespace UserManagement.Infrastructure.Entities;

[BsonIgnoreExtraElements]
public class User
{
    [BsonId]
    [BsonSerializer(typeof(FlexibleStringObjectIdSerializer))]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("Username")]
    public string? LegacyUsername { get; set; }

    [BsonElement("FullName")]
    public string? LegacyFullName { get; set; }

    [BsonElement("avatar_url")]
    public string? AvatarUrl { get; set; }

    [BsonElement("identity_provider_id")]
    public string IdentityProviderId { get; set; } = string.Empty;

    /// <summary>
    /// List of tenant IDs this user belongs to
    /// </summary>
    [BsonElement("tenant_ids")]
    public List<string> TenantIds { get; set; } = new() { "default" };

    /// <summary>
    /// Keycloak user ID (sub claim from Keycloak)
    /// </summary>
    [BsonElement("keycloak_id")]
    public string? KeycloakId { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string GetDisplayName()
    {
        return FirstNonEmpty(Name, LegacyFullName, LegacyUsername, Email, Id);
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }
        }

        return string.Empty;
    }
}
