using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Notifications;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

public class UpdateUserRolesCommandHandler(
    IUserRepository userRepository,
    IKeycloakClient keycloakClient,
    IPublisher publisher,
    ILogger<UpdateUserRolesCommandHandler> logger
) : IRequestHandler<UpdateUserRolesCommand, UpdateUserRolesResponse>
{
    public async Task<UpdateUserRolesResponse> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
        user ??= await userRepository.GetByKeycloakIdAsync(request.Id, cancellationToken);

        if (user == null)
        {
            return new UpdateUserRolesResponse(
                Success: false,
                Message: $"User with ID {request.Id} not found in database",
                User: null
            );
        }

        if (string.IsNullOrEmpty(user.KeycloakId))
        {
            return new UpdateUserRolesResponse(
                Success: false,
                Message: "User does not have a Keycloak ID. Please log in first to sync your identity.",
                User: null
            );
        }

        try
        {
            var currentRoles = await keycloakClient.GetUserRolesAsync(user.KeycloakId);
            var currentRoleNames = currentRoles.Select(r => r.Name ?? "").Where(n => !string.IsNullOrEmpty(n)).ToList();

            var rolesToAdd = request.Roles.Except(currentRoleNames).ToList();
            var rolesToRemove = currentRoleNames.Except(request.Roles).ToList();

            if (rolesToRemove.Count > 0)
            {
                await keycloakClient.RemoveRolesFromUserAsync(user.KeycloakId, rolesToRemove);
                logger.LogInformation("Removed roles {Roles} from user {UserId}", string.Join(", ", rolesToRemove), user.KeycloakId);
            }

            if (rolesToAdd.Count > 0)
            {
                await keycloakClient.AssignRolesToUserAsync(user.KeycloakId, rolesToAdd);
                logger.LogInformation("Added roles {Roles} to user {UserId}", string.Join(", ", rolesToAdd), user.KeycloakId);
            }

            user.UpdatedAt = DateTime.UtcNow;
            await userRepository.UpdateAsync(user, cancellationToken);

            if (rolesToAdd.Count > 0 || rolesToRemove.Count > 0)
            {
                await publisher.Publish(new UserRolesUpdatedNotification(
                    user.Id, user.Name, rolesToAdd, rolesToRemove, null, null
                ), cancellationToken);
            }

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
            logger.LogError(ex, "Keycloak error while updating roles for user {UserId} (KeycloakId: {KeycloakId})",
                user.Id, user.KeycloakId);
            return new UpdateUserRolesResponse(
                Success: false,
                Message: $"Failed to communicate with Keycloak: {ex.Message}",
                User: null
            );
        }
    }
}
