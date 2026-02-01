using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Tenant;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.Entities;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IKeycloakClient keycloakClient,
    ITenantContext tenantContext,
    ILogger<CreateUserCommandHandler> logger
) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already exists
            var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                return new CreateUserResponse(
                    Success: false,
                    Message: $"User with email {request.Email} already exists",
                    User: null
                );
            }

            // Create user in Keycloak
            string? keycloakUserId;
            try
            {
                keycloakUserId = await keycloakClient.CreateUserAsync(
                    request.Username,
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.TemporaryPassword
                );
                
                // Assign initial roles if provided
                if (keycloakUserId != null && request.InitialRoles?.Any() == true)
                {
                    await keycloakClient.AssignRolesToUserAsync(keycloakUserId, request.InitialRoles);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create user in Keycloak");
                return new CreateUserResponse(
                    Success: false,
                    Message: $"Failed to create user in Keycloak: {ex.Message}",
                    User: null
                );
            }

            // TODO: Create user in Duende Identity Server (remove this in the future)
            // For now, users will be created on first login via the GetOrCreateProfile endpoint
            // which handles Identity Provider user creation

            // Create user in TEMS database
            var tenantId = tenantContext.TenantId;
            var user = new User
            {
                Name = request.Username,
                Email = request.Email,
                AvatarUrl = null,
                IdentityProviderId = keycloakUserId ?? "",
                KeycloakId = keycloakUserId,
                TenantIds = new List<string> { tenantId },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdUser = await userRepository.CreateAsync(user, cancellationToken);

            // Fetch the roles we just assigned
            var roles = new List<string>();
            if (!string.IsNullOrEmpty(keycloakUserId))
            {
                try
                {
                    var keycloakRoles = await keycloakClient.GetUserRolesAsync(keycloakUserId);
                    roles = keycloakRoles.Select(r => r.Name ?? "").Where(r => !string.IsNullOrEmpty(r)).ToList();
                }
                catch
                {
                    roles = request.InitialRoles?.ToList() ?? new List<string>();
                }
            }

            var userDto = new UserDto(
                Id: createdUser.Id,
                Username: createdUser.Name,
                Email: createdUser.Email,
                FirstName: request.FirstName,
                LastName: request.LastName,
                AvatarUrl: createdUser.AvatarUrl,
                TenantIds: createdUser.TenantIds,
                KeycloakId: createdUser.KeycloakId,
                Roles: roles,
                CreatedAt: createdUser.CreatedAt,
                UpdatedAt: createdUser.UpdatedAt
            );

            return new CreateUserResponse(
                Success: true,
                Message: "User created successfully",
                User: userDto
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create user");
            return new CreateUserResponse(
                Success: false,
                Message: $"Failed to create user: {ex.Message}",
                User: null
            );
        }
    }
}
