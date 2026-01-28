using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class GetAllTicketsEndpoint : EndpointWithoutRequest<GetAllTicketsResponse>
{
    private readonly IMediator _mediator;

    public GetAllTicketsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/tickets");
        Policies("CanManageTickets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var command = new GetAllTicketsCommand();
        var response = await _mediator.Send(command, ct);
        await Send.OkAsync(response, ct);
    }
}
