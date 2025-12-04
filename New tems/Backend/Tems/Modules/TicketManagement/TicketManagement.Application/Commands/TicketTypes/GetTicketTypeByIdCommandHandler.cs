using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.TicketTypes;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.TicketTypes;

public class GetTicketTypeByIdCommandHandler : IRequestHandler<GetTicketTypeByIdCommand, GetTicketTypeResponse>
{
    private readonly ITicketTypeRepository _repository;

    public GetTicketTypeByIdCommandHandler(ITicketTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetTicketTypeResponse> Handle(GetTicketTypeByIdCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await _repository.GetByIdAsync(request.TicketTypeId, request.TenantId, cancellationToken);

        if (ticketType == null)
            throw new KeyNotFoundException($"TicketType with ID {request.TicketTypeId} not found");

        return new GetTicketTypeResponse(
            ticketType.TicketTypeId,
            ticketType.TenantId,
            ticketType.Name,
            ticketType.Description,
            ticketType.ItilCategory,
            ticketType.Version,
            new WorkflowConfigResponse(
                ticketType.WorkflowConfig.States.Select(s => new WorkflowStateResponse(
                    s.Id,
                    s.Label,
                    s.Type,
                    s.AllowedTransitions,
                    s.AutomationHook
                )).ToList(),
                ticketType.WorkflowConfig.InitialStateId
            ),
            ticketType.AttributeDefinitions.Select(a => new AttributeDefinitionResponse(
                a.Key,
                a.Label,
                a.DataType,
                a.IsRequired,
                a.IsPredefined,
                a.AiExtractionHint,
                a.ValidationRule
            )).ToList(),
            ticketType.CreatedAt,
            ticketType.UpdatedAt
        );
    }
}
