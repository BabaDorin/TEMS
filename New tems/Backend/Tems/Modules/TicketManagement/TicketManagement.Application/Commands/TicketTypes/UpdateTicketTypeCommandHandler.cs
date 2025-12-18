using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Domain;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.TicketTypes;

public class UpdateTicketTypeCommandHandler : IRequestHandler<UpdateTicketTypeCommand, UpdateTicketTypeResponse>
{
    private readonly ITicketTypeRepository _repository;
    private readonly ITenantContext _tenantContext;

    public UpdateTicketTypeCommandHandler(ITicketTypeRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<UpdateTicketTypeResponse> Handle(UpdateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByIdAsync(request.TicketTypeId, _tenantContext.TenantId, cancellationToken);
        if (existing == null)
            throw new KeyNotFoundException($"TicketType with ID {request.TicketTypeId} not found");

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.ItilCategory = request.ItilCategory.ToUpper();
        existing.Version = request.Version;
        existing.WorkflowConfig = new WorkflowConfig
        {
            States = request.WorkflowConfig.States.Select(s => new WorkflowState
            {
                Id = s.Id,
                Label = s.Label,
                Type = s.Type.ToUpper(),
                AllowedTransitions = s.AllowedTransitions,
                AutomationHook = s.AutomationHook
            }).ToList(),
            InitialStateId = request.WorkflowConfig.InitialStateId
        };
        existing.AttributeDefinitions = request.AttributeDefinitions.Select(a => new AttributeDefinition
        {
            Key = a.Key,
            Label = a.Label,
            DataType = a.DataType.ToUpper(),
            IsRequired = a.IsRequired,
            IsPredefined = a.IsPredefined,
            Options = a.Options,
            AiExtractionHint = a.AiExtractionHint,
            ValidationRule = a.ValidationRule
        }).ToList();

        var success = await _repository.UpdateAsync(existing, cancellationToken);

        return new UpdateTicketTypeResponse(success);
    }
}
