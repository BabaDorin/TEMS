using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class CreateTicketEndpoint : Endpoint<CreateTicketCommand, CreateTicketResponse>
{
    private readonly IMediator _mediator;

    public CreateTicketEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/tickets");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateTicketCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
