using MediatR;
using UserManagement.Contract.Responses;

namespace UserManagement.Contract.Commands;

public record GetAllRolesCommand() : IRequest<GetAllRolesResponse>;
