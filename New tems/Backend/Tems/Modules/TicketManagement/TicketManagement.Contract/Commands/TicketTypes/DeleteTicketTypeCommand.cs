using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.TicketTypes;

public record DeleteTicketTypeCommand(
    string TicketTypeId
) : IRequest<DeleteTicketTypeResponse>;
