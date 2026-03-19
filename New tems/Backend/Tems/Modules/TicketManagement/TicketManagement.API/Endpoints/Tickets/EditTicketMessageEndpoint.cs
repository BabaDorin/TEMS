using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class EditTicketMessageEndpoint(IMediator mediator) : Endpoint<EditTicketMessageCommand, EditTicketMessageResponse>
{
    public override void Configure()
    {
        Patch("/tickets/{TicketId}/messages/{MessageId}");
        Policies("CanManageTickets");
    }

    public override async Task HandleAsync(EditTicketMessageCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            ThrowError("Message content cannot be empty.", 400);
            return;
        }

        var response = await mediator.Send(request, ct);
        if (!response.Success)
        {
            ThrowError("Ticket message not found.", 404);
            return;
        }

        await Send.OkAsync(response, cancellation: ct);
    }
}
