using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.TicketTypes;

public class DeleteTicketTypeCommandHandler : IRequestHandler<DeleteTicketTypeCommand, DeleteTicketTypeResponse>
{
    private readonly ITicketTypeRepository _repository;
    private readonly ITenantContext _tenantContext;

    public DeleteTicketTypeCommandHandler(ITicketTypeRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<DeleteTicketTypeResponse> Handle(DeleteTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var success = await _repository.DeleteAsync(request.TicketTypeId, _tenantContext.TenantId, cancellationToken);
        return new DeleteTicketTypeResponse(success);
    }
}
