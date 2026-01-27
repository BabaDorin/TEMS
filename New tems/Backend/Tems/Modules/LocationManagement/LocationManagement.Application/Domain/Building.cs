namespace LocationManagement.Application.Domain;

public class Building
{
    public required string Id { get; set; }
    public required string SiteId { get; set; }
    public required string Name { get; set; }
    public string AddressLine { get; set; } = string.Empty;
    public string ManagerContact { get; set; } = string.Empty;
    public required string TenantId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}
