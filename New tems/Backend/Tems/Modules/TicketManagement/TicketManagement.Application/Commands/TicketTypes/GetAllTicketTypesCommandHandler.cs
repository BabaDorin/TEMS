using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.TicketTypes;

public class GetAllTicketTypesCommandHandler : IRequestHandler<GetAllTicketTypesCommand, GetAllTicketTypesResponse>
{
    private readonly ITicketTypeRepository _repository;

    public GetAllTicketTypesCommandHandler(ITicketTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllTicketTypesResponse> Handle(GetAllTicketTypesCommand request, CancellationToken cancellationToken)
    {
        var ticketTypes = await _repository.GetAllAsync(request.TenantId, cancellationToken);

        var response = ticketTypes.Select(tt => new GetTicketTypeResponse(
            tt.TicketTypeId,
            tt.TenantId,
            tt.Name,
            tt.Description,
            tt.ItilCategory,
            tt.Version,
            new WorkflowConfigResponse(
                tt.WorkflowConfig.States.Select(s => new WorkflowStateResponse(
                    s.Id,
                    s.Label,
                    s.Type,
                    s.AllowedTransitions,
                    s.AutomationHook
                )).ToList(),
                tt.WorkflowConfig.InitialStateId
            ),
            tt.AttributeDefinitions.Select(a => new AttributeDefinitionResponse(
                a.Key,
                a.Label,
                a.DataType,
                a.IsRequired,
                a.IsPredefined,
                a.AiExtractionHint,
                a.ValidationRule
            )).ToList(),
            tt.CreatedAt,
            tt.UpdatedAt
        )).ToList();

        return new GetAllTicketTypesResponse(response);
    }
}
