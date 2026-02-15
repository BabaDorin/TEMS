using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Tems.Common.Identity;

namespace Tems.IdentityServer.Controllers;

/// <summary>
/// API Controller for managing users in Identity Server.
/// WORKAROUND: This is a temporary solution for TEMS to create users directly in Identity Server.
/// In the future, user creation should be handled exclusively by Keycloak, and Identity Server
/// should only be used for authentication (not user management).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserManagementController : ControllerBase
{
    private readonly IMongoDatabase _database;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ILogger<UserManagementController> _logger;

    public UserManagementController(
        IMongoDatabase database,
        IPasswordHasher<User> passwordHasher,
        ILogger<UserManagementController> logger)
    {
        _database = database;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new user in Identity Server.
    /// WORKAROUND: This endpoint is temporary and will be removed when user creation is handled by Keycloak only.
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var users = _database.GetCollection<User>("users");

            // Check if user already exists
            var existingUser = await users.Find(u => 
                u.Username == request.Username || u.Email == request.Email
            ).FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return Conflict(new CreateUserResponse
                {
                    Success = false,
                    Message = existingUser.Username == request.Username 
                        ? "Username already exists" 
                        : "Email already exists"
                });
            }

            // Create the user
            var user = new User
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Username = request.Username,
                Email = request.Email,
                FullName = $"{request.FirstName ?? ""} {request.LastName ?? ""}".Trim(),
                PhoneNumber = "",
                IsActive = true,
                Roles = new List<string>(), // Roles are managed by Keycloak
                Claims = new Dictionary<string, string>(),
                CreatedAt = DateTime.UtcNow
            };

            // Hash password if provided, otherwise generate a random one
            var password = !string.IsNullOrWhiteSpace(request.TemporaryPassword) 
                ? request.TemporaryPassword 
                : GenerateRandomPassword();
            
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            await users.InsertOneAsync(user);

            _logger.LogInformation("User {Username} created successfully in Identity Server", request.Username);

            return Ok(new CreateUserResponse
            {
                Success = true,
                Message = "User created successfully",
                UserId = user.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user {Username} in Identity Server", request.Username);
            return StatusCode(500, new CreateUserResponse
            {
                Success = false,
                Message = $"Failed to create user: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Deletes a user from Identity Server.
    /// WORKAROUND: This endpoint is temporary and will be removed when user management is handled by Keycloak only.
    /// </summary>
    [HttpDelete("{username}")]
    public async Task<IActionResult> DeleteUser(string username)
    {
        try
        {
            var users = _database.GetCollection<User>("users");
            var result = await users.DeleteOneAsync(u => u.Username == username);

            if (result.DeletedCount == 0)
            {
                return NotFound(new DeleteUserResponse
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            _logger.LogInformation("User {Username} deleted successfully from Identity Server", username);

            return Ok(new DeleteUserResponse
            {
                Success = true,
                Message = "User deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user {Username} from Identity Server", username);
            return StatusCode(500, new DeleteUserResponse
            {
                Success = false,
                Message = $"Failed to delete user: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Deletes a user from Identity Server by email.
    /// WORKAROUND: This endpoint is temporary.
    /// </summary>
    [HttpDelete("by-email/{email}")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        try
        {
            var users = _database.GetCollection<User>("users");
            var result = await users.DeleteOneAsync(u => u.Email == email);

            if (result.DeletedCount == 0)
            {
                return NotFound(new DeleteUserResponse
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            _logger.LogInformation("User with email {Email} deleted successfully from Identity Server", email);

            return Ok(new DeleteUserResponse
            {
                Success = true,
                Message = "User deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user with email {Email} from Identity Server", email);
            return StatusCode(500, new DeleteUserResponse
            {
                Success = false,
                Message = $"Failed to delete user: {ex.Message}"
            });
        }
    }

    private static string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 16)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

public record CreateUserRequest
{
    public string Username { get; init; } = "";
    public string Email { get; init; } = "";
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? TemporaryPassword { get; init; }
}

public record CreateUserResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
    public string? UserId { get; init; }
}

public record DeleteUserResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
}
