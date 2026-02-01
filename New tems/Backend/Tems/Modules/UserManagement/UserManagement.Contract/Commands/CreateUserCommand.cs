using MediatR;
using UserManagement.Contract.Responses;

namespace UserManagement.Contract.Commands;

public record CreateUserCommand(
    string Username,
    string Email,
    string? FirstName,
    string? LastName,
    string? TemporaryPassword,
    List<string>? InitialRoles
) : IRequest<CreateUserResponse>;
