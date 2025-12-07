using MediatR;
using Tems.Common.Tenant;
using TicketManagement.Application.Domain;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.TicketTypes;

public class CreateTicketTypeCommandHandler : IRequestHandler<CreateTicketTypeCommand, CreateTicketTypeResponse>
{
    private readonly ITicketTypeRepository _repository;
    private readonly ITenantContext _tenantContext;

    public CreateTicketTypeCommandHandler(ITicketTypeRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<CreateTicketTypeResponse> Handle(CreateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var ticketType = new TicketType
        {
            TicketTypeId = Guid.NewGuid().ToString(),
            TenantId = _tenantContext.TenantId,
            Name = request.Name,
            Description = request.Description,
            ItilCategory = request.ItilCategory?.ToUpper() ?? "INCIDENT",
            Version = request.Version > 0 ? request.Version : 1,
            WorkflowConfig = request.WorkflowConfig != null ? new WorkflowConfig
            {
                States = request.WorkflowConfig.States?.Select(s => new WorkflowState
                {
                    Id = s.Id,
                    Label = s.Label,
                    Type = s.Type?.ToUpper() ?? "ACTIVE",
                    AllowedTransitions = s.AllowedTransitions ?? new List<string>(),
                    AutomationHook = s.AutomationHook
                }).ToList() ?? new List<WorkflowState>(),
                InitialStateId = request.WorkflowConfig.InitialStateId ?? "open"
            } : new WorkflowConfig
            {
                States = new List<WorkflowState>
                {
                    new WorkflowState { Id = "open", Label = "Open", Type = "ACTIVE", AllowedTransitions = new List<string> { "in-progress", "closed" } },
                    new WorkflowState { Id = "in-progress", Label = "In Progress", Type = "ACTIVE", AllowedTransitions = new List<string> { "open", "closed" } },
                    new WorkflowState { Id = "closed", Label = "Closed", Type = "CLOSED", AllowedTransitions = new List<string>() }
                },
                InitialStateId = "open"
            },
            AttributeDefinitions = request.AttributeDefinitions?.Select(a => new AttributeDefinition
            {
                Key = a.Key,
                Label = a.Label,
                DataType = a.DataType?.ToUpper() ?? "TEXT",
                IsRequired = a.IsRequired,
                IsPredefined = a.IsPredefined,
                Options = a.Options,
                AiExtractionHint = a.AiExtractionHint,
                ValidationRule = a.ValidationRule
            }).ToList() ?? new List<AttributeDefinition>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(ticketType, cancellationToken);

        return new CreateTicketTypeResponse(created.TicketTypeId);
    }
}
