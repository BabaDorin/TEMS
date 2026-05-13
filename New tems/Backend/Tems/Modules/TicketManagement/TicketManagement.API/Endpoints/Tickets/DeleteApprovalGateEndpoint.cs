using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class DeleteApprovalGateEndpoint : Endpoint<DeleteApprovalGateCommand, UpdateApprovalGateResponse>
{
    private readonly IMediator _mediator;

    public DeleteApprovalGateEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/tickets/{TicketId}/approval-gates/{ApprovalGateId}");
        Policies("CanOpenOrManageTickets");
    }

    public override async Task HandleAsync(DeleteApprovalGateCommand request, CancellationToken ct)
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
