using MediatR;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using TicketManagement.Application.Domain;
using TicketManagement.Application.Helpers;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;
using UserManagement.Infrastructure.Repositories;

namespace TicketManagement.Application.Commands.Tickets;

public class AddTicketMessageCommandHandler : IRequestHandler<AddTicketMessageCommand, AddTicketMessageResponse>
{
    private readonly ITicketConversationRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    public AddTicketMessageCommandHandler(
        ITicketConversationRepository repository,
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public async Task<AddTicketMessageResponse> Handle(AddTicketMessageCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _httpContextAccessor.HttpContext?.User;
        var currentUserId = await ApprovalGateHelper.ResolveCurrentUserIdAsync(currentUser, _userRepository, cancellationToken);
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            throw new UnauthorizedAccessException("Could not determine the current user");
        }

        if (request.IsInternalNote && !ApprovalGateHelper.IsManager(currentUser))
        {
            throw new UnauthorizedAccessException("Internal notes are available only to ticket managers");
        }

        var message = new TicketMessage
        {
            MessageId = ObjectId.GenerateNewId().ToString(),
            SenderType = request.SenderType.ToUpper(),
            SenderId = currentUserId,
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
