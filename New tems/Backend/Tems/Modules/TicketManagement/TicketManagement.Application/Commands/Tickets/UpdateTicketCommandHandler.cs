using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, UpdateTicketResponse>
{
    private readonly ITicketRepository _repository;

    public UpdateTicketCommandHandler(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateTicketResponse> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.TicketId, request.TenantId, cancellationToken);
        if (existing == null)
            throw new KeyNotFoundException($"Ticket with ID {request.TicketId} not found");

        existing.Summary = request.Summary;
        existing.CurrentStateId = request.CurrentStateId;
        existing.Priority = request.Priority.ToUpper();
        existing.AssigneeId = request.AssigneeId;
        existing.Attributes = request.Attributes;

        var success = await _repository.UpdateAsync(existing, cancellationToken);

        return new UpdateTicketResponse(success);
    }
}
