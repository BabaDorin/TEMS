using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

public class UpdateUserRolesCommandHandler(
    IUserRepository userRepository,
    IKeycloakClient keycloakClient,
    ILogger<UpdateUserRolesCommandHandler> logger
) : IRequestHandler<UpdateUserRolesCommand, UpdateUserRolesResponse>
{
    public async Task<UpdateUserRolesResponse> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get the user from our database
            var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                return new UpdateUserRolesResponse(
                    Success: false,
                    Message: $"User with ID {request.Id} not found",
                    User: null
                );
            }

            if (string.IsNullOrEmpty(user.KeycloakId))
            {
                return new UpdateUserRolesResponse(
                    Success: false,
                    Message: "User does not have a Keycloak ID, cannot update roles",
                    User: null
                );
            }

            // Get current roles from Keycloak
            var currentRoles = await keycloakClient.GetUserRolesAsync(user.KeycloakId);
            var currentRoleNames = currentRoles.Select(r => r.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList();

            // Calculate roles to add and remove
            var rolesToAdd = request.Roles.Except(currentRoleNames).ToList();
            var rolesToRemove = currentRoleNames.Except(request.Roles).ToList();

            // Remove old roles
            if (rolesToRemove.Any())
            {
                await keycloakClient.RemoveRolesFromUserAsync(user.KeycloakId, rolesToRemove);
                logger.LogInformation("Removed roles {Roles} from user {UserId}", string.Join(", ", rolesToRemove), user.KeycloakId);
            }

            // Add new roles
            if (rolesToAdd.Any())
            {
                await keycloakClient.AssignRolesToUserAsync(user.KeycloakId, rolesToAdd);
                logger.LogInformation("Added roles {Roles} to user {UserId}", string.Join(", ", rolesToAdd), user.KeycloakId);
            }

            // Update the user's UpdatedAt timestamp
            user.UpdatedAt = DateTime.UtcNow;
            await userRepository.UpdateAsync(user, cancellationToken);

            var userDto = new UserDto(
                Id: user.Id,
                Username: user.Name,
                Email: user.Email,
                FirstName: null,
                LastName: null,
                AvatarUrl: user.AvatarUrl,
                TenantIds: user.TenantIds,
                KeycloakId: user.KeycloakId,
                Roles: request.Roles,
                CreatedAt: user.CreatedAt,
                UpdatedAt: user.UpdatedAt
            );

            return new UpdateUserRolesResponse(
                Success: true,
                Message: "User roles updated successfully",
                User: userDto
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update user roles");
            return new UpdateUserRolesResponse(
                Success: false,
                Message: $"Failed to update user roles: {ex.Message}",
                User: null
            );
        }
    }
}
