using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.Keycloak;

namespace UserManagement.Application.Commands;

public class GetAllRolesCommandHandler(
    IKeycloakClient keycloakClient,
    ILogger<GetAllRolesCommandHandler> logger
) : IRequestHandler<GetAllRolesCommand, GetAllRolesResponse>
{
    public async Task<GetAllRolesResponse> Handle(GetAllRolesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var keycloakRoles = await keycloakClient.GetAllRolesAsync();
            
            var roles = keycloakRoles
                .Where(r => !string.IsNullOrEmpty(r.Name))
                .Select(r => new RoleDto(
                    Id: r.Id ?? "",
                    Name: r.Name!,
                    Description: r.Description
                ))
                .ToList();

            return new GetAllRolesResponse(Roles: roles);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get roles from Keycloak");
            return new GetAllRolesResponse(Roles: new List<RoleDto>());
        }
    }
}
