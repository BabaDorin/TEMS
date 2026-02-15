using MediatR;
using UserManagement.Contract.Responses;

namespace UserManagement.Contract.Commands;

public record DeleteUserCommand(
    string Id,
    string? CallerKeycloakId = null
) : IRequest<DeleteUserResponse>;
