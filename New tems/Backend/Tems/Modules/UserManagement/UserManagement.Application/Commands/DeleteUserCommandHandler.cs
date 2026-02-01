using MediatR;
using Microsoft.Extensions.Logging;
using UserManagement.Contract.Commands;
using UserManagement.Contract.Responses;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Application.Commands;

public class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IKeycloakClient keycloakClient,
    ILogger<DeleteUserCommandHandler> logger
) : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
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

            // Delete from Keycloak only (not from Duende Identity Server as per requirements)
            if (!string.IsNullOrEmpty(user.KeycloakId))
            {
                try
                {
                    await keycloakClient.DeleteUserAsync(user.KeycloakId);
                    logger.LogInformation("Deleted user {UserId} from Keycloak", user.KeycloakId);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to delete user {UserId} from Keycloak, continuing with database deletion", user.KeycloakId);
                    // Continue with database deletion even if Keycloak deletion fails
                }
            }

            // Delete from TEMS database
            await userRepository.DeleteAsync(request.Id, cancellationToken);
            logger.LogInformation("Deleted user {UserId} from database", request.Id);

            return new DeleteUserResponse(
                Success: true,
                Message: "User deleted successfully"
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete user");
            return new DeleteUserResponse(
                Success: false,
                Message: $"Failed to delete user: {ex.Message}"
            );
        }
    }
}
