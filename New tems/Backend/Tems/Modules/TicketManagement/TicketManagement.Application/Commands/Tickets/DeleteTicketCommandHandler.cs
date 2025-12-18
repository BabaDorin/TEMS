using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, DeleteTicketResponse>
{
    private readonly ITicketRepository _repository;
    private readonly ITenantContext _tenantContext;

    public DeleteTicketCommandHandler(ITicketRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<DeleteTicketResponse> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
    {
        var success = await _repository.DeleteAsync(request.TicketId, _tenantContext.TenantId, cancellationToken);
        return new DeleteTicketResponse(success);
    }
}
