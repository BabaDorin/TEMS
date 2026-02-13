using MediatR;
using UserManagement.Contract.DTOs;

namespace UserManagement.Contract.Commands;

public record GetUserByIdCommand(string Id) : IRequest<UserDto?>;
