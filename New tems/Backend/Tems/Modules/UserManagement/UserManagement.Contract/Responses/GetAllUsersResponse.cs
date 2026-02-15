using UserManagement.Contract.DTOs;

namespace UserManagement.Contract.Responses;

public record GetAllUsersResponse(
    List<UserDto> Users,
    int TotalCount,
    int PageNumber,
    int PageSize
);
