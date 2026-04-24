using MediatR;
using UserManagement.Contract.Responses;

namespace UserManagement.Contract.Commands;

public record GetAllUsersCommand(
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<GetAllUsersResponse>;
