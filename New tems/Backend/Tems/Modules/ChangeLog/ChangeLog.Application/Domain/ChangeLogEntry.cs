using ChangeLog.Contract.Enums;

namespace ChangeLog.Application.Domain;

public abstract class ChangeLogEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public ChangeLogAction Action { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? PerformedByUserId { get; set; }
    public string? PerformedByUserName { get; set; }

    public abstract ChangeLogEntityType EntityType { get; }
    public abstract string EntityId { get; }
    public abstract Dictionary<string, string?> GetReferences();
    public abstract Dictionary<string, object?>? GetDetails();
}
