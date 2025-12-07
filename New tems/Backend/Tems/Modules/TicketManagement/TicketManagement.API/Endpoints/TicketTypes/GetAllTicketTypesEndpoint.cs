using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.TicketTypes;

public class GetAllTicketTypesEndpoint : EndpointWithoutRequest<GetAllTicketTypesResponse>
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

    public override async Task HandleAsync(CancellationToken ct)
    {
        var command = new GetAllTicketTypesCommand();
        var response = await _mediator.Send(command, ct);
        await Send.OkAsync(response, ct);
    }
}
