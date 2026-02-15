using ChangeLog.Application.Interfaces;
using ChangeLog.Contract.DTOs;
using ChangeLog.Contract.Queries;
using MediatR;
using Tems.Common.Tenant;

namespace ChangeLog.Application.Queries;

public class GetEntityTimelineQueryHandler(
    IChangeLogRepository changeLogRepository,
    ITenantContext tenantContext
) : IRequestHandler<GetEntityTimelineQuery, GetEntityTimelineResponse>
{
    public async Task<GetEntityTimelineResponse> Handle(GetEntityTimelineQuery request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId ?? "default";

        var (entries, totalCount) = await changeLogRepository.GetByEntityAsync(
            request.EntityType,
            request.EntityId,
            tenantId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = entries.Select(e => new ChangeLogEntryDto(
            e.Id,
            e.Action,
            e.Description,
            e.Timestamp,
            e.PerformedByUserId,
            e.PerformedByUserName,
            e.GetReferences(),
            e.GetDetails()
        )).ToList();

        return new GetEntityTimelineResponse(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}
