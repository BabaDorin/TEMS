using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class GetTicketMessagesEndpoint : Endpoint<GetTicketMessagesCommand, GetTicketMessagesResponse>
{
    private readonly IMediator _mediator;

    public GetTicketMessagesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/tickets/{TicketId}/messages");
        Policies("CanManageTickets");
    }

    public override async Task HandleAsync(GetTicketMessagesCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
