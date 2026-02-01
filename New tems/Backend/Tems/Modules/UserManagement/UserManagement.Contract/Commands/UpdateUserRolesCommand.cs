using MediatR;
using UserManagement.Contract.Responses;

namespace UserManagement.Contract.Commands;

public record UpdateUserRolesCommand(
    string Id,
    List<string> Roles
) : IRequest<UpdateUserRolesResponse>;
