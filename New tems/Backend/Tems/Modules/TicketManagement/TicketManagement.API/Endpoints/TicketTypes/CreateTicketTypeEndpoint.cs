using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.TicketTypes;

public class CreateTicketTypeEndpoint : Endpoint<CreateTicketTypeCommand, CreateTicketTypeResponse>
{
    private readonly IMediator _mediator;

    public CreateTicketTypeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/ticket-types");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateTicketTypeCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
