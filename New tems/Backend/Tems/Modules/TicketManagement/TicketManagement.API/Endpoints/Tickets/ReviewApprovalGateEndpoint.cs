using FastEndpoints;
using MediatR;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.API.Endpoints.Tickets;

public class ReviewApprovalGateEndpoint : Endpoint<ReviewApprovalGateCommand, ReviewApprovalGateResponse>
{
    private readonly IMediator _mediator;

    public ReviewApprovalGateEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/tickets/{TicketId}/approval-gates/{ApprovalGateId}/review");
        Policies("CanOpenOrManageTickets");
    }

    public override async Task HandleAsync(ReviewApprovalGateCommand request, CancellationToken ct)
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
