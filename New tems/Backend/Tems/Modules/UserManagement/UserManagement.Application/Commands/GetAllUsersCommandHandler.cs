using MediatR;
using Tems.Common.Tenant;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

public class GetAllUsersCommandHandler(
    IUserRepository userRepository,
    IKeycloakClient keycloakClient,
    ITenantContext tenantContext
) : IRequestHandler<GetAllUsersCommand, GetAllUsersResponse>
{
    public async Task<GetAllUsersResponse> Handle(GetAllUsersCommand request, CancellationToken cancellationToken)
    {
        var tenantId = tenantContext.TenantId;
        
        var (users, totalCount) = await userRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            tenantId,
            cancellationToken
        );

        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = new List<string>();
            
            // Fetch roles from Keycloak if user has a KeycloakId
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

            userDtos.Add(new UserDto(
                Id: user.Id,
                Username: user.Name,
                Email: user.Email,
                FirstName: null, // Not stored in our DB yet
                LastName: null,
                AvatarUrl: user.AvatarUrl,
                TenantIds: user.TenantIds,
                KeycloakId: user.KeycloakId,
                Roles: roles,
                CreatedAt: user.CreatedAt,
                UpdatedAt: user.UpdatedAt
            ));
        }

        return new GetAllUsersResponse(
            Users: userDtos,
            TotalCount: totalCount,
            PageNumber: request.PageNumber,
            PageSize: request.PageSize
        );
    }
}
