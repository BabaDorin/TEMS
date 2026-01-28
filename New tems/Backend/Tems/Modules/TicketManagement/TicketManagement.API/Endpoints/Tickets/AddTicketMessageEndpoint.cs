using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class AddTicketMessageEndpoint(IMediator mediator) : Endpoint<AddTicketMessageCommand, AddTicketMessageResponse>
{
    public override void Configure()
    {
        Post("/tickets/{TicketId}/messages");
        Policies("CanManageTickets");
    }

    public override async Task HandleAsync(AddTicketMessageCommand request, CancellationToken ct)
    {
        var response = await mediator.Send(request, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
