using ChangeLog.Application.Domain;
using ChangeLog.Contract.Enums;

namespace ChangeLog.Application.Interfaces;

public interface IChangeLogRepository
{
    Task CreateAsync(ChangeLogEntry entry, CancellationToken cancellationToken = default);
    Task<(List<ChangeLogEntry> Entries, int TotalCount)> GetByEntityAsync(
        ChangeLogEntityType entityType,
        string entityId,
        string tenantId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
