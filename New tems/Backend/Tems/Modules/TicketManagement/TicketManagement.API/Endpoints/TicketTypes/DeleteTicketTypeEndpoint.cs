using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.TicketTypes;

public class DeleteTicketTypeEndpoint : Endpoint<DeleteTicketTypeCommand, DeleteTicketTypeResponse>
{
    private readonly IMediator _mediator;

    public DeleteTicketTypeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/ticket-types/{TicketTypeId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteTicketTypeCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
