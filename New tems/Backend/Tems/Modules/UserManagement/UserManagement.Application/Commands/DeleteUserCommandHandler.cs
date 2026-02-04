using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Contract.Commands;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.IdentityServer;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

/// <summary>
/// Handles user deletion across all 3 systems:
/// 1. Keycloak - for roles and access management
/// 2. Duende Identity Server - for authentication (WORKAROUND: will be removed in future)
/// 3. TEMS Database - for user management table
/// </summary>
public class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IKeycloakClient keycloakClient,
    IIdentityServerClient identityServerClient,
    ILogger<DeleteUserCommandHandler> logger
) : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting user {UserId} from all 3 systems", request.Id);
        
        try
        {
            // Get the user from our database
            var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null)
            {
                return new DeleteUserResponse(
                    Success: false,
                    Message: $"User with ID {request.Id} not found"
                );
            }

            var errors = new List<string>();

            // =============================================
            // STEP 1: Delete from Keycloak
            // =============================================
            if (!string.IsNullOrEmpty(user.KeycloakId))
            {
                try
                {
                    logger.LogInformation("Step 1/3: Deleting user {Username} from Keycloak", user.Name);
                    await keycloakClient.DeleteUserAsync(user.KeycloakId);
                    logger.LogInformation("Successfully deleted user from Keycloak");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to delete user from Keycloak - continuing with other deletions");
                    errors.Add($"Keycloak: {ex.Message}");
                }
            }
            else
            {
                logger.LogWarning("User does not have a Keycloak ID - skipping Keycloak deletion");
            }

            // =============================================
            // STEP 2: Delete from Duende Identity Server
            // WORKAROUND: This step will be removed in the future
            // =============================================
            try
            {
                logger.LogInformation("Step 2/3: Deleting user {Username} from Identity Server (WORKAROUND)", user.Name);
                
                // Try to delete by username first
                var deleted = await identityServerClient.DeleteUserAsync(user.Name);
                
                // If not found by username, try by email
                if (!deleted && !string.IsNullOrEmpty(user.Email))
                {
                    deleted = await identityServerClient.DeleteUserByEmailAsync(user.Email);
                }
                
                if (deleted)
                {
                    logger.LogInformation("Successfully deleted user from Identity Server");
                }
                else
                {
                    logger.LogWarning("User not found in Identity Server or deletion failed");
                    errors.Add("Identity Server: User not found or deletion failed");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to delete user from Identity Server - continuing with database deletion");
                errors.Add($"Identity Server: {ex.Message}");
            }

            // =============================================
            // STEP 3: Delete from TEMS Database
            // =============================================
            try
            {
                logger.LogInformation("Step 3/3: Deleting user {UserId} from TEMS Database", request.Id);
                await userRepository.DeleteAsync(request.Id, cancellationToken);
                logger.LogInformation("Successfully deleted user from TEMS Database");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete user from TEMS Database");
                return new DeleteUserResponse(
                    Success: false,
                    Message: $"Failed to delete user from database: {ex.Message}"
                );
            }

            // Report final status
            if (errors.Any())
            {
                logger.LogWarning("User deleted from TEMS database but some systems had issues: {Errors}", 
                    string.Join(", ", errors));
                
                return new DeleteUserResponse(
                    Success: true,
                    Message: $"User deleted from TEMS. Some systems had issues: {string.Join(", ", errors)}"
                );
            }

            logger.LogInformation("User {UserId} deleted successfully from all 3 systems", request.Id);

            return new DeleteUserResponse(
                Success: true,
                Message: "User deleted successfully from all systems"
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while deleting user");
            return new DeleteUserResponse(
                Success: false,
                Message: $"Failed to delete user: {ex.Message}"
            );
        }
    }
}
