using MediatR;
using Microsoft.Extensions.Logging;
using Tems.Common.Notifications;
using Tems.Common.Tenant;
using UserManagement.Contract.Commands;
using UserManagement.Contract.DTOs;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.Entities;
using UserManagement.Infrastructure.IdentityServer;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IKeycloakClient keycloakClient,
    IIdentityServerClient identityServerClient,
    IPublisher publisher,
    ITenantContext tenantContext,
    ILogger<CreateUserCommandHandler> logger
) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating user {Username} in all 3 systems (Keycloak, Identity Server, TEMS DB)", 
            request.Username);

        try
        {
            // Check if user already exists in TEMS
            var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                return new CreateUserResponse(
                    Success: false,
                    Message: $"User with email {request.Email} already exists",
                    User: null
                );
            }

            // =============================================
            // STEP 1: Create user in Keycloak
            // Keycloak is responsible for roles and access management
            // =============================================
            string? keycloakUserId = null;
            try
            {
                logger.LogInformation("Step 1/3: Creating user {Username} in Keycloak", request.Username);
                
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
                    logger.LogInformation("Assigning {RoleCount} roles to user in Keycloak", request.InitialRoles.Count);
                    await keycloakClient.AssignRolesToUserAsync(keycloakUserId, request.InitialRoles);
                }
                
                logger.LogInformation("Successfully created user {Username} in Keycloak with ID: {KeycloakId}", 
                    request.Username, keycloakUserId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create user in Keycloak - aborting user creation");
                return new CreateUserResponse(
                    Success: false,
                    Message: $"Failed to create user in Keycloak: {ex.Message}",
                    User: null
                );
            }

            // =============================================
            // STEP 2: Create user in Duende Identity Server
            // WORKAROUND: This step will be removed in the future when Identity Server 
            // is no longer used for user management, only for authentication.
            // =============================================
            try
            {
                logger.LogInformation("Step 2/3: Creating user {Username} in Identity Server (WORKAROUND)", 
                    request.Username);
                
                var identityResult = await identityServerClient.CreateUserAsync(
                    request.Username,
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.TemporaryPassword
                );
                
                if (!identityResult.Success)
                {
                    // Rollback Keycloak user creation
                    logger.LogWarning("Failed to create user in Identity Server, rolling back Keycloak user");
                    if (!string.IsNullOrEmpty(keycloakUserId))
                    {
                        try
                        {
                            await keycloakClient.DeleteUserAsync(keycloakUserId);
                            logger.LogInformation("Rolled back Keycloak user creation");
                        }
                        catch (Exception rollbackEx)
                        {
                            logger.LogError(rollbackEx, "Failed to rollback Keycloak user creation");
                        }
                    }
                    
                    return new CreateUserResponse(
                        Success: false,
                        Message: $"Failed to create user in Identity Server: {identityResult.Message}",
                        User: null
                    );
                }
                
                logger.LogInformation("Successfully created user {Username} in Identity Server", request.Username);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception while creating user in Identity Server - rolling back Keycloak");
                
                // Rollback Keycloak user creation
                if (!string.IsNullOrEmpty(keycloakUserId))
                {
                    try
                    {
                        await keycloakClient.DeleteUserAsync(keycloakUserId);
                    }
                    catch (Exception rollbackEx)
                    {
                        logger.LogError(rollbackEx, "Failed to rollback Keycloak user creation");
                    }
                }
                
                return new CreateUserResponse(
                    Success: false,
                    Message: $"Failed to create user in Identity Server: {ex.Message}",
                    User: null
                );
            }

            // =============================================
            // STEP 3: Create user in TEMS Database
            // =============================================
            User createdUser;
            try
            {
                logger.LogInformation("Step 3/3: Creating user {Username} in TEMS Database", request.Username);
                
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

                createdUser = await userRepository.CreateAsync(user, cancellationToken);
                logger.LogInformation("Successfully created user {Username} in TEMS Database with ID: {UserId}", 
                    request.Username, createdUser.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create user in TEMS Database - rolling back Keycloak and Identity Server");
                
                // Rollback Keycloak user creation
                if (!string.IsNullOrEmpty(keycloakUserId))
                {
                    try
                    {
                        await keycloakClient.DeleteUserAsync(keycloakUserId);
                    }
                    catch (Exception rollbackEx)
                    {
                        logger.LogError(rollbackEx, "Failed to rollback Keycloak user creation");
                    }
                }
                
                // Rollback Identity Server user creation
                try
                {
                    await identityServerClient.DeleteUserAsync(request.Username);
                }
                catch (Exception rollbackEx)
                {
                    logger.LogError(rollbackEx, "Failed to rollback Identity Server user creation");
                }
                
                return new CreateUserResponse(
                    Success: false,
                    Message: $"Failed to create user in TEMS Database: {ex.Message}",
                    User: null
                );
            }

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

            logger.LogInformation("User {Username} created successfully in all 3 systems", request.Username);

            await publisher.Publish(new UserCreatedNotification(
                createdUser.Id, request.Username, request.Email, null, null
            ), cancellationToken);

            return new CreateUserResponse(
                Success: true,
                Message: "User created successfully in all systems (Keycloak, Identity Server, TEMS)",
                User: userDto
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating user {Username}", request.Username);
            return new CreateUserResponse(
                Success: false,
                Message: $"Failed to create user: {ex.Message}",
                User: null
            );
        }
    }
}
