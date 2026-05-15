using AssetManagement.Application.Domain;

namespace AssetManagement.Application.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetByTicketIdAsync(string ticketId, string tenantId, CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetByPoNumberAsync(string poNumber, string tenantId, CancellationToken cancellationToken = default);
    Task<List<PurchaseOrder>> GetAllAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<PurchaseOrder> CreateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, string tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByPoNumberAsync(string tenantId, string poNumber, string? excludeId = null, CancellationToken cancellationToken = default);
}
