using ChangeLog.Contract.Enums;
using ChangeLog.Contract.Queries;
using FastEndpoints;
using MediatR;

namespace ChangeLog.API.Endpoints;

public class GetEntityTimelineRequest
{
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class GetEntityTimelineEndpoint(IMediator mediator) 
    : Endpoint<GetEntityTimelineRequest, GetEntityTimelineResponse>
{
    public override void Configure()
    {
        Get("/changelog/{EntityType}/{EntityId}");
        Policies("CanManageAssets");
    }

    public override async Task HandleAsync(GetEntityTimelineRequest req, CancellationToken ct)
    {
        if (!Enum.TryParse<ChangeLogEntityType>(req.EntityType, true, out var entityType))
        {
            AddError("Invalid entity type. Must be one of: Asset, User, Location");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var query = new GetEntityTimelineQuery(entityType, req.EntityId, req.PageNumber, req.PageSize);
        var response = await mediator.Send(query, ct);

        await Send.OkAsync(response, cancellation: ct);
    }
}
