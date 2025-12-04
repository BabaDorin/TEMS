using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.TicketTypes;

public record GetTicketTypeByIdCommand(
    string TicketTypeId,
    string TenantId
) : IRequest<GetTicketTypeResponse>;
