using MediatR;
using TicketManagement.Application.Domain;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class AddTicketMessageCommandHandler : IRequestHandler<AddTicketMessageCommand, AddTicketMessageResponse>
{
    private readonly ITicketConversationRepository _repository;

    public AddTicketMessageCommandHandler(ITicketConversationRepository repository)
    {
        _repository = repository;
    }

    public async Task<AddTicketMessageResponse> Handle(AddTicketMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new TicketMessage
        {
            SenderType = request.SenderType.ToUpper(),
            SenderId = request.SenderId,
            Timestamp = DateTime.UtcNow,
            Content = request.Content,
            ChannelMessageId = request.ChannelMessageId,
            IsInternalNote = request.IsInternalNote
        };

        var success = await _repository.AddMessageAsync(request.TicketId, message, cancellationToken);

        return new AddTicketMessageResponse(success);
    }
}
