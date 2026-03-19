using MediatR;
using TicketManagement.Application.Interfaces;
using TicketManagement.Contract.Commands.Tickets;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Application.Commands.Tickets;

public class DeleteTicketMessageCommandHandler : IRequestHandler<DeleteTicketMessageCommand, DeleteTicketMessageResponse>
{
    private readonly ITicketConversationRepository _repository;

    public DeleteTicketMessageCommandHandler(ITicketConversationRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeleteTicketMessageResponse> Handle(DeleteTicketMessageCommand request, CancellationToken cancellationToken)
    {
        var success = await _repository.DeleteMessageAsync(request.TicketId, request.MessageId, cancellationToken);
        return new DeleteTicketMessageResponse(success);
    }
}
