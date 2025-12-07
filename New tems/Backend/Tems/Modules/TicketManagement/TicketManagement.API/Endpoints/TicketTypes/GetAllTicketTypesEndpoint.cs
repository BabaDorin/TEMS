using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.TicketTypes;

public class GetAllTicketTypesEndpoint : Endpoint<GetAllTicketTypesCommand, GetAllTicketTypesResponse>
{
    private readonly IMediator _mediator;

    public GetAllTicketTypesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/ticket-types");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAllTicketTypesCommand request, CancellationToken ct)
    {
        var response = await _mediator.Send(request, ct);
        await Send.OkAsync(response, cancellation: ct);
    }
}
