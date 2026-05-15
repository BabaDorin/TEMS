namespace AssetManagement.Contract.Responses;

public record GetAllPurchaseOrdersResponse(
    List<PurchaseOrderDto> PurchaseOrders
);

public record GetPurchaseOrderByIdResponse(
    PurchaseOrderDto? PurchaseOrder
);

public record DeletePurchaseOrderResponse(
    bool Success
);

public record PurchaseOrderDto(
    string Id,
    string TicketId,
    string TicketHumanReadableId,
    string PoNumber,
    string Vendor,
    decimal Amount,
    string Currency,
    string Description,
    string CreatedByUserId,
    string CreatedByDisplayName,
    string AccountableUserId,
    string AccountableDisplayName,
    decimal UsedAmount,
    decimal AvailableAmount,
    int ItemCount,
    List<PurchaseOrderItemDto> Items,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record PurchaseOrderItemDto(
    string AssetId,
    string AssetTag,
    string SerialNumber,
    decimal Price,
    DateTime CreatedAt
);
