using MediatR;
using UserManagement.Contract.DTOs;

namespace UserManagement.Contract.Commands;

public record SearchUsersCommand(
    string? Name = null,
    int Take = 10
) : IRequest<List<UserLookupDto>>;
