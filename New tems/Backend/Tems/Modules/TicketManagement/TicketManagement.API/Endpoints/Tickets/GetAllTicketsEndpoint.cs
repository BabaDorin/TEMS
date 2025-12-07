using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class GetAllTicketsEndpoint : Endpoint<GetAllTicketsCommand, GetAllTicketsResponse>
{
    private readonly IMediator _mediator;

    public GetAllTicketsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/tickets");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllTicketsCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
