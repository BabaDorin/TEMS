using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class DeleteTicketMessageEndpoint(IMediator mediator) : Endpoint<DeleteTicketMessageCommand, DeleteTicketMessageResponse>
{
    public override void Configure()
    {
        Delete("/tickets/{TicketId}/messages/{MessageId}");
        Policies("Authenticated");
    }

    public override async Task HandleAsync(DeleteTicketMessageCommand request, CancellationToken ct)
    {
        try
        {
            var response = await mediator.Send(request, ct);
            if (!response.Success)
            {
                ThrowError("Ticket message not found.", 404);
                return;
            }

            await Send.NoContentAsync(ct);
        }
        catch (UnauthorizedAccessException)
        {
            await Send.ForbiddenAsync(ct);
        }
        catch (KeyNotFoundException)
        {
            ThrowError("Ticket message not found.", 404);
            return;
        }
    }
}
