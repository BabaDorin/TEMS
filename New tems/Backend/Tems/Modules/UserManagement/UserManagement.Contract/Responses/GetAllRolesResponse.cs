using UserManagement.Contract.DTOs;

namespace UserManagement.Contract.Responses;

public record GetAllRolesResponse(
    List<RoleDto> Roles
);
