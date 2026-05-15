namespace AssetManagement.Application.Domain;

public class PurchaseOrder
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string TicketId { get; set; } = string.Empty;
    public string TicketHumanReadableId { get; set; } = string.Empty;
    public string PoNumber { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; } = string.Empty;
    public string AccountableUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
