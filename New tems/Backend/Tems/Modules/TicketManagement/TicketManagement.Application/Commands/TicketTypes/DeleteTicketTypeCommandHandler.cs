using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.TicketTypes;

public class DeleteTicketTypeCommandHandler : IRequestHandler<DeleteTicketTypeCommand, DeleteTicketTypeResponse>
{
    private readonly ITicketTypeRepository _repository;

    public DeleteTicketTypeCommandHandler(ITicketTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeleteTicketTypeResponse> Handle(DeleteTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var success = await _repository.DeleteAsync(request.TicketTypeId, request.TenantId, cancellationToken);
        return new DeleteTicketTypeResponse(success);
    }
}
