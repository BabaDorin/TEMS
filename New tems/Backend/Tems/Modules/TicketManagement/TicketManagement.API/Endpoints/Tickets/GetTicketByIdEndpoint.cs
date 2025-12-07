using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class GetTicketByIdEndpoint : Endpoint<GetTicketByIdCommand, GetTicketResponse>
{
    private readonly IMediator _mediator;

    public GetTicketByIdEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/tickets/{TicketId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetTicketByIdCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
