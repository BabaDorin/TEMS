using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, UpdateTicketResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly ITenantContext _tenantContext;

    public UpdateTicketCommandHandler(ITicketRepository repository, ITicketTypeRepository ticketTypeRepository, ITenantContext tenantContext)
    {
        _repository = repository;
        _ticketTypeRepository = ticketTypeRepository;
        _tenantContext = tenantContext;
    }

    public async Task<UpdateTicketResponse> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);
        if (existing == null)
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");

        var ticketType = await _ticketTypeRepository.GetByIdAsync(existing.TicketTypeId, _tenantContext.TenantId, cancellationToken);
        if (ticketType == null)
        {
            throw new KeyNotFoundException($"TicketType with ID {existing.TicketTypeId} not found");
        }

        var resolvedStateId = TicketStateHelper.ResolveManagedStatusId(ticketType.WorkflowConfig.States, request.CurrentStateId);
        if (resolvedStateId == null)
        {
            throw new InvalidOperationException($"Status '{request.CurrentStateId}' is not allowed for this ticket type");
        }

        existing.Summary = request.Summary;
        existing.CurrentStateId = resolvedStateId;
        existing.Priority = request.Priority.ToUpper();
        existing.AssigneeId = request.AssigneeId;
        existing.Attributes = request.Attributes;
        existing.UpdatedAt = DateTime.UtcNow;

        var success = await _repository.UpdateAsync(existing, cancellationToken);

        return new UpdateTicketResponse(success);
    }
}
