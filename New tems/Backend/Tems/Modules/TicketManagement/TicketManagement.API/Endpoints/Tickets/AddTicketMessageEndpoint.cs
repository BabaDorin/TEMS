using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class AddTicketMessageEndpoint : Endpoint<AddTicketMessageCommand, AddTicketMessageResponse>
{
    private readonly IMediator _mediator;

    public AddTicketMessageEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/tickets/{TicketId}/messages");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddTicketMessageCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await SendAsync(response, 201, ct);
    }
}
