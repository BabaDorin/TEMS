using ChangeLog.Contract.Queries;
using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;

namespace TicketManagement.API.Endpoints.Tickets;

public class GetTicketHistoryEndpoint : Endpoint<GetTicketHistoryQuery, GetEntityTimelineResponse>
{
    private readonly IMediator _mediator;

    public GetTicketHistoryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/tickets/{TicketId}/history");
        Policies("CanOpenOrManageTickets");
    }

    public override async Task HandleAsync(GetTicketHistoryQuery request, CancellationToken ct)
    {
        try
        {
            var response = await _mediator.Send(request, ct);
            await Send.OkAsync(response, cancellation: ct);
        }
        catch (UnauthorizedAccessException)
        {
            await Send.ForbiddenAsync(ct);
        }
        catch (KeyNotFoundException)
        {
            await Send.NotFoundAsync(ct);
        }
    }
}
