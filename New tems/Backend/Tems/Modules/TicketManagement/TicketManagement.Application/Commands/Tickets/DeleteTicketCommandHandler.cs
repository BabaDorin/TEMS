using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, DeleteTicketResponse>
{
    private readonly ITicketRepository _repository;

    public DeleteTicketCommandHandler(ITicketRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeleteTicketResponse> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var success = await _repository.DeleteAsync(request.TicketId, request.TenantId, cancellationToken);
        return new DeleteTicketResponse(success);
    }
}
