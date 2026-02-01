namespace UserManagement.Infrastructure.Keycloak;

public interface IKeycloakClient
{
    /// <summary>
    /// Creates a new user in Keycloak
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="email">The email address</param>
    /// <param name="firstName">First name (optional)</param>
    /// <param name="lastName">Last name (optional)</param>
    /// <param name="temporaryPassword">Temporary password that user must change on first login</param>
    /// <returns>The Keycloak user ID</returns>
    Task<string?> CreateUserAsync(string username, string email, string? firstName = null, string? lastName = null, string? temporaryPassword = null);
    
    /// <summary>
    /// Deletes a user from Keycloak
    /// </summary>
    /// <param name="keycloakUserId">The Keycloak user ID</param>
    Task DeleteUserAsync(string keycloakUserId);
    
    /// <summary>
    /// Gets a user by their Keycloak ID
    /// </summary>
    /// <param name="keycloakUserId">The Keycloak user ID</param>
    /// <returns>The user details</returns>
    Task<KeycloakUserDto?> GetUserAsync(string keycloakUserId);
    
    /// <summary>
    /// Gets a user by their email address
    /// </summary>
    /// <param name="email">The email address</param>
    /// <returns>The user details</returns>
    Task<KeycloakUserDto?> GetUserByEmailAsync(string email);
    
    /// <summary>
    /// Gets all realm roles assigned to a user
    /// </summary>
    /// <param name="keycloakUserId">The Keycloak user ID</param>
    /// <returns>List of roles</returns>
    Task<List<KeycloakRoleDto>> GetUserRolesAsync(string keycloakUserId);
    
    /// <summary>
    /// Gets all available realm roles
    /// </summary>
    /// <returns>List of all roles</returns>
    Task<List<KeycloakRoleDto>> GetAllRolesAsync();
    
    /// <summary>
    /// Assigns roles to a user (replaces existing realm role mappings)
    /// </summary>
    /// <param name="keycloakUserId">The Keycloak user ID</param>
    /// <param name="roleNames">The role names to assign</param>
    Task AssignRolesToUserAsync(string keycloakUserId, IEnumerable<string> roleNames);
    
    /// <summary>
    /// Removes roles from a user
    /// </summary>
    /// <param name="keycloakUserId">The Keycloak user ID</param>
    /// <param name="roleNames">The role names to remove</param>
    Task RemoveRolesFromUserAsync(string keycloakUserId, IEnumerable<string> roleNames);
}
