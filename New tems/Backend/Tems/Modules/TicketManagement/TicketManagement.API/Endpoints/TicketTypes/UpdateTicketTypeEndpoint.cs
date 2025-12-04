using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.TicketTypes;

public class UpdateTicketTypeEndpoint : Endpoint<UpdateTicketTypeCommand, UpdateTicketTypeResponse>
{
    private readonly IMediator _mediator;

    public UpdateTicketTypeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/ticket-types/{TicketTypeId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateTicketTypeCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await SendAsync(response, 200, ct);
    }
}
