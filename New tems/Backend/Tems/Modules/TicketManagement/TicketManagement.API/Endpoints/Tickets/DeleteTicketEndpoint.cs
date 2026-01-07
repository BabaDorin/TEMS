using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class DeleteTicketEndpoint : Endpoint<DeleteTicketCommand, DeleteTicketResponse>
{
    private readonly IMediator _mediator;

    public DeleteTicketEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/tickets/{TicketId}");
        Policies("CanManageTickets");
    }

    public override async Task HandleAsync(DeleteTicketCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
