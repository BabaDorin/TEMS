using MediatR;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

public class GetUserByIdCommandHandler(
    IUserRepository userRepository,
    IKeycloakClient keycloakClient
) : IRequestHandler<GetUserByIdCommand, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
        {
            return null;
        }

        var roles = new List<string>();
        
        if (!string.IsNullOrEmpty(user.KeycloakId))
        {
            try
            {
                var keycloakRoles = await keycloakClient.GetUserRolesAsync(user.KeycloakId);
                roles = keycloakRoles.Select(r => r.Name ?? "").Where(r => !string.IsNullOrEmpty(r)).ToList();
            }
            catch
            {
                // If we can't fetch roles, continue with empty list
            }
        }

        return new UserDto(
            Id: user.Id,
            Username: user.Name,
            Email: user.Email,
            FirstName: null,
            LastName: null,
            AvatarUrl: user.AvatarUrl,
            TenantIds: user.TenantIds,
            KeycloakId: user.KeycloakId,
            Roles: roles,
            CreatedAt: user.CreatedAt,
            UpdatedAt: user.UpdatedAt
        );
    }
}
