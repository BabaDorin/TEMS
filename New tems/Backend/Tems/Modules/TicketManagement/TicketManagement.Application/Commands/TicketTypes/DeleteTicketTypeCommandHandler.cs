using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.TicketTypes;

public class DeleteTicketTypeCommandHandler : IRequestHandler<DeleteTicketTypeCommand, DeleteTicketTypeResponse>
{
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketConversationRepository _conversationRepository;
    private readonly ITenantContext _tenantContext;

    public DeleteTicketTypeCommandHandler(
        ITicketTypeRepository ticketTypeRepository,
        ITicketRepository ticketRepository,
        ITicketConversationRepository conversationRepository,
        ITenantContext tenantContext)
    {
        _ticketTypeRepository = ticketTypeRepository;
        _ticketRepository = ticketRepository;
        _conversationRepository = conversationRepository;
        _tenantContext = tenantContext;
    }

    public async Task<DeleteTicketTypeResponse> Handle(DeleteTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var tickets = await _ticketRepository.GetByTicketTypeIdAsync(request.TicketTypeId, _tenantContext.TenantId, cancellationToken);
        var ticketIds = tickets.Select(ticket => ticket.TicketId).ToList();

        if (ticketIds.Count > 0)
        {
            await _conversationRepository.DeleteByTicketIdsAsync(ticketIds, cancellationToken);
            await _ticketRepository.DeleteByTicketTypeIdAsync(request.TicketTypeId, _tenantContext.TenantId, cancellationToken);
        }

        var success = await _ticketTypeRepository.DeleteAsync(request.TicketTypeId, _tenantContext.TenantId, cancellationToken);
        return new DeleteTicketTypeResponse(success);
    }
}
