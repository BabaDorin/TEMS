using MediatR;
using TicketManagement.Contract.Responses;

namespace TicketManagement.Contract.Commands.Tickets;

public record GetAllTicketsCommand() : IRequest<GetAllTicketsResponse>;
