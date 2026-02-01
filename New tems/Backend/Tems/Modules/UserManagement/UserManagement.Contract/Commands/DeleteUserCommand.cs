using MediatR;
using UserManagement.Contract.Responses;

namespace UserManagement.Contract.Commands;

public record DeleteUserCommand(
    string Id
) : IRequest<DeleteUserResponse>;
