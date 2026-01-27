namespace LocationManagement.Application.Domain;

public class Site
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Timezone { get; set; }
    public bool IsActive { get; set; } = true;
    public required string TenantId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}
