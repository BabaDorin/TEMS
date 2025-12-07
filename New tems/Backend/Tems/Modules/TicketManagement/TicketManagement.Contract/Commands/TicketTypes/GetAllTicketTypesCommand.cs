using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.TicketTypes;

public record GetAllTicketTypesCommand : IRequest<GetAllTicketTypesResponse>;
