using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tems.Common.Identity;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string FullName { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
    
    public List<string> Roles { get; set; } = new();
    
    public Dictionary<string, string> Claims { get; set; } = new();
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
