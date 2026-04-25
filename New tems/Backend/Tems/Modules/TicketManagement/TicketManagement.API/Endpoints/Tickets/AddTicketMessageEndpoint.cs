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
        Policies("Authenticated");
    }

    public override async Task HandleAsync(AddTicketMessageCommand request, CancellationToken ct)
    {
        try
        {
            var response = await mediator.Send(request, ct);
            if (!response.Success)
            {
                ThrowError("Ticket conversation not found.", 404);
                return;
            }

            await Send.CreatedAtAsync<GetTicketMessagesEndpoint>(new { request.TicketId }, response, cancellation: ct);
        }
        catch (UnauthorizedAccessException)
        {
            await Send.ForbiddenAsync(ct);
        }
    }
}
