using ChangeLog.Contract.DTOs;
using ChangeLog.Contract.Enums;
using MediatR;

namespace ChangeLog.Contract.Queries;

public record GetEntityTimelineQuery(
    ChangeLogEntityType EntityType,
    string EntityId,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<GetEntityTimelineResponse>;

public record GetEntityTimelineResponse(
    List<ChangeLogEntryDto> Entries,
    int TotalCount,
    int PageNumber,
    int PageSize
);
