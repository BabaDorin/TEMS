namespace TicketManagement.Application.Helpers;

public static class PurchaseOrderTicketConstants
{
    public const string TicketTypeId = "ticket_type_purchase_order";
    public const string PoNumberAttributeKey = "po_number";
    public const string VendorAttributeKey = "vendor";
    public const string AmountAttributeKey = "amount";
    public const string CurrencyAttributeKey = "currency";
    public const string AccountablePersonAttributeKey = "accountable_person";
    public const string DescriptionAttributeKey = "description";

    public static bool IsPurchaseOrderTicketType(string? ticketTypeId)
    {
        return string.Equals(ticketTypeId, TicketTypeId, StringComparison.OrdinalIgnoreCase);
    }
}
