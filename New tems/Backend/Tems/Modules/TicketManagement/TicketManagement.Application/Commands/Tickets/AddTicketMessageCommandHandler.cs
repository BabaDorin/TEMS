using MediatR;
using MongoDB.Bson;
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
            MessageId = ObjectId.GenerateNewId().ToString(),
            SenderType = request.SenderType.ToUpper(),
            SenderId = request.SenderId,
            Timestamp = DateTime.UtcNow,
            Content = request.Content,
            ChannelMessageId = request.ChannelMessageId,
            IsInternalNote = request.IsInternalNote,
            EditedAt = null
        };

        var success = await _repository.AddMessageAsync(request.TicketId, message, cancellationToken);

        if (!success)
            return new AddTicketMessageResponse(false, null);

        var messageResponse = new TicketMessageResponse(
            message.MessageId,
            message.SenderType,
            message.SenderId,
            message.Timestamp,
            message.Content,
            message.ChannelMessageId,
            message.IsInternalNote,
            message.EditedAt
        );

        return new AddTicketMessageResponse(true, messageResponse);
    }
}
