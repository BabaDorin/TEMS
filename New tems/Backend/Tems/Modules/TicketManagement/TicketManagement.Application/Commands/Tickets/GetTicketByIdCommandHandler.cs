using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class GetTicketByIdCommandHandler : IRequestHandler<GetTicketByIdCommand, GetTicketResponse>
{
    private readonly ITicketRepository _repository;

    public GetTicketByIdCommandHandler(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetTicketResponse> Handle(GetTicketByIdCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _repository.GetByIdAsync(request.TicketId, request.TenantId, cancellationToken);

        if (ticket == null)
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");

        return new GetTicketResponse(
            ticket.TicketId,
            ticket.TenantId,
            ticket.TicketTypeId,
            ticket.HumanReadableId,
            ticket.Summary,
            ticket.CurrentStateId,
            ticket.Priority,
            new ReporterResponse(
                ticket.Reporter.UserId,
                ticket.Reporter.ChannelSource,
                ticket.Reporter.ChannelThreadId
            ),
            ticket.AssigneeId,
            ticket.Attributes,
            ticket.CreatedAt,
            ticket.UpdatedAt,
            ticket.ResolvedAt
        );
    }
}
