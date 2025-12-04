using MediatR;
using TicketManagement.Application.Domain;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.TicketTypes;

public class CreateTicketTypeCommandHandler : IRequestHandler<CreateTicketTypeCommand, CreateTicketTypeResponse>
{
    private readonly ITicketTypeRepository _repository;

    public CreateTicketTypeCommandHandler(ITicketTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateTicketTypeResponse> Handle(CreateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var ticketType = new TicketType
        {
            TicketTypeId = Guid.NewGuid().ToString(),
            TenantId = request.TenantId,
            Name = request.Name,
            Description = request.Description,
            ItilCategory = request.ItilCategory.ToUpper(),
            Version = request.Version,
            WorkflowConfig = new WorkflowConfig
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
            },
            AttributeDefinitions = request.AttributeDefinitions.Select(a => new AttributeDefinition
            {
                Key = a.Key,
                Label = a.Label,
                DataType = a.DataType.ToUpper(),
                IsRequired = a.IsRequired,
                IsPredefined = a.IsPredefined,
                AiExtractionHint = a.AiExtractionHint,
                ValidationRule = a.ValidationRule
            }).ToList(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(ticketType, cancellationToken);

        return new CreateTicketTypeResponse(created.TicketTypeId);
    }
}
