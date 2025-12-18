using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class GetAllTicketsCommandHandler : IRequestHandler<GetAllTicketsCommand, GetAllTicketsResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITenantContext _tenantContext;

    public GetAllTicketsCommandHandler(ITicketRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<GetAllTicketsResponse> Handle(GetAllTicketsCommand request, CancellationToken cancellationToken)
    {
        var tickets = await _repository.GetAllAsync(_tenantContext.TenantId, cancellationToken);

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
