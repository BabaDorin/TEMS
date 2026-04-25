using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class GetTicketsForApprovalEndpoint : EndpointWithoutRequest<GetAllTicketsResponse>
{
    private readonly IMediator _mediator;

    public GetTicketsForApprovalEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/tickets/for-approval");
        Policies("CanOpenOrManageTickets");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var response = await _mediator.Send(new GetTicketsForApprovalCommand(), ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
