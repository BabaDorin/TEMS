namespace LocationManagement.Application.Domain;

public class Room
{
    public required string Id { get; set; }
    public required string BuildingId { get; set; }
    public required string Name { get; set; }
    public string? RoomNumber { get; set; }
    public string FloorLabel { get; set; } = string.Empty;
    public required RoomType Type { get; set; }
    public int Capacity { get; set; }
    public double? Area { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Available;
    public string? Description { get; set; }
    public required string TenantId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
}

public enum RoomType
{
    Meeting,
    Desk,
    Workshop,
    ServerRoom
}

public enum RoomStatus
{
    Available,
    Maintenance,
    Decommissioned
}
