using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class GetAllTicketsCommandHandler : IRequestHandler<GetAllTicketsCommand, GetAllTicketsResponse>
{
    private readonly ITicketRepository _repository;

    public GetAllTicketsCommandHandler(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllTicketsResponse> Handle(GetAllTicketsCommand request, CancellationToken cancellationToken)
    {
        var tickets = await _repository.GetAllAsync(request.TenantId, cancellationToken);

        var response = tickets.Select(t => new GetTicketResponse(
            t.TicketId,
            t.TenantId,
            t.TicketTypeId,
            t.HumanReadableId,
            t.Summary,
            t.CurrentStateId,
            t.Priority,
            new ReporterResponse(
                t.Reporter.UserId,
                t.Reporter.ChannelSource,
                t.Reporter.ChannelThreadId
            ),
            t.AssigneeId,
            t.Attributes,
            t.CreatedAt,
            t.UpdatedAt,
            t.ResolvedAt
        )).ToList();

        return new GetAllTicketsResponse(response);
    }
}
