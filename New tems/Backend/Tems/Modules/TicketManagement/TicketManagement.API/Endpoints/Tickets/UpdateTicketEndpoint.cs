using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class UpdateTicketEndpoint : Endpoint<UpdateTicketCommand, UpdateTicketResponse>
{
    private readonly IMediator _mediator;

    public UpdateTicketEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/tickets/{TicketId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateTicketCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await SendAsync(response, 200, ct);
    }
}
