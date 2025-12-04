using MediatR;
using TicketManagement.Application.Domain;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, CreateTicketResponse>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketTypeRepository _ticketTypeRepository;
    private readonly ITicketConversationRepository _conversationRepository;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        ITicketTypeRepository ticketTypeRepository,
        ITicketConversationRepository conversationRepository)
    {
        _ticketRepository = ticketRepository;
        _ticketTypeRepository = ticketTypeRepository;
        _conversationRepository = conversationRepository;
    }

    public async Task<CreateTicketResponse> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketType = await _ticketTypeRepository.GetByIdAsync(request.TicketTypeId, request.TenantId, cancellationToken);
        if (ticketType == null)
            throw new KeyNotFoundException($"TicketType with ID {request.TicketTypeId} not found");

        var prefix = GetPrefixFromItilCategory(ticketType.ItilCategory);
        var nextNumber = await _ticketRepository.GetNextTicketNumberAsync(request.TenantId, prefix, cancellationToken);
        var humanReadableId = $"{prefix}-{nextNumber}";

        var ticket = new Ticket
        {
            TicketId = Guid.NewGuid().ToString(),
            TenantId = request.TenantId,
            TicketTypeId = request.TicketTypeId,
            HumanReadableId = humanReadableId,
            Summary = request.Summary,
            CurrentStateId = ticketType.WorkflowConfig.InitialStateId,
            Priority = request.Priority.ToUpper(),
            Reporter = new Reporter
            {
                UserId = request.Reporter.UserId,
                ChannelSource = request.Reporter.ChannelSource.ToUpper(),
                ChannelThreadId = request.Reporter.ChannelThreadId
            },
            AssigneeId = request.AssigneeId,
            Attributes = request.Attributes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _ticketRepository.CreateAsync(ticket, cancellationToken);

        var conversation = new TicketConversation
        {
            ConversationId = Guid.NewGuid().ToString(),
            TicketId = created.TicketId,
            Messages = new List<TicketMessage>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _conversationRepository.CreateAsync(conversation, cancellationToken);

        return new CreateTicketResponse(created.TicketId, created.HumanReadableId);
    }

    private string GetPrefixFromItilCategory(string category)
    {
        return category.ToUpper() switch
        {
            "INCIDENT" => "INC",
            "PROBLEM" => "PRB",
            "CHANGE" => "CHG",
            "REQUEST" => "REQ",
            "SECURITY_INCIDENT" => "SEC",
            "ALERT" => "ALT",
            _ => "TKT"
        };
    }
}
