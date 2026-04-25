using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class UpdateApprovalGateEndpoint : Endpoint<UpdateApprovalGateCommand, UpdateApprovalGateResponse>
{
    private readonly IMediator _mediator;

    public UpdateApprovalGateEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/tickets/{TicketId}/approval-gates/{ApprovalGateId}");
        Policies("CanOpenOrManageTickets");
    }

    public override async Task HandleAsync(UpdateApprovalGateCommand request, CancellationToken ct)
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
    }
}
