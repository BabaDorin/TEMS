using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class UpdateTicketEndpoint : Endpoint<UpdateTicketCommand, UpdateTicketResponse>
{
    private readonly IMediator _mediator;

    public UpdateTicketEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/tickets/{TicketId}");
        Policies("CanOpenOrManageTickets");
    }

    public override async Task HandleAsync(UpdateTicketCommand request, CancellationToken ct)
    {
        try
        {
            var response = await _mediator.Send(request, ct);
            await Send.OkAsync(response, cancellation: ct);
        }
        catch (UnauthorizedAccessException)
        {
            await Send.ForbiddenAsync(ct);
        }
        catch (KeyNotFoundException)
        {
            await Send.NotFoundAsync(ct);
        }
        catch (InvalidOperationException ex)
        {
            ThrowError(ex.Message, 400);
        }
    }
}
