using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UserManagement.Infrastructure.IdentityServer;

/// <summary>
/// Configuration for the Identity Server client.
/// </summary>
public class IdentityServerSettings
{
    public const string SectionName = "IdentityServer";
    public string Authority { get; set; } = "http://localhost:5001";
    public string ApiName { get; set; } = "tems-api";
}

/// <summary>
/// Client for interacting with Duende Identity Server user management API.
/// 
/// WORKAROUND: This client is temporary and will be removed in the future when 
/// user creation is handled exclusively by Keycloak. Currently, TEMS needs to 
/// create users in Identity Server for authentication purposes, but this 
/// responsibility should eventually be moved to Keycloak only.
/// </summary>
public interface IIdentityServerClient
{
    /// <summary>
    /// Creates a user in Identity Server.
    /// </summary>
    Task<IdentityServerCreateUserResponse> CreateUserAsync(
        string username,
        string email,
        string? firstName,
        string? lastName,
        string? temporaryPassword);

    /// <summary>
    /// Deletes a user from Identity Server by username.
    /// </summary>
    Task<bool> DeleteUserAsync(string username);

    /// <summary>
    /// Deletes a user from Identity Server by email.
    /// </summary>
    Task<bool> DeleteUserByEmailAsync(string email);
}

public class IdentityServerClient : IIdentityServerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IdentityServerClient> _logger;
    private readonly IdentityServerSettings _settings;

    public IdentityServerClient(
        HttpClient httpClient,
        IOptions<IdentityServerSettings> settings,
        ILogger<IdentityServerClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(_settings.Authority.TrimEnd('/') + "/");
    }

    public async Task<IdentityServerCreateUserResponse> CreateUserAsync(
        string username,
        string email,
        string? firstName,
        string? lastName,
        string? temporaryPassword)
    {
        try
        {
            _logger.LogInformation("Creating user {Username} in Identity Server at {Authority}", 
                username, _settings.Authority);

            var request = new
            {
                Username = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                TemporaryPassword = temporaryPassword
            };

            var response = await _httpClient.PostAsJsonAsync("api/usermanagement/create", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<IdentityServerApiResponse>();
                _logger.LogInformation("Successfully created user {Username} in Identity Server", username);
                
                return new IdentityServerCreateUserResponse
                {
                    Success = result?.Success ?? true,
                    Message = result?.Message ?? "User created successfully",
                    UserId = result?.UserId
                };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to create user {Username} in Identity Server. Status: {Status}, Response: {Response}", 
                username, response.StatusCode, errorContent);

            // Try to parse error response
            try
            {
                var errorResult = await response.Content.ReadFromJsonAsync<IdentityServerApiResponse>();
                return new IdentityServerCreateUserResponse
                {
                    Success = false,
                    Message = errorResult?.Message ?? $"Failed to create user: {response.StatusCode}"
                };
            }
            catch
            {
                return new IdentityServerCreateUserResponse
                {
                    Success = false,
                    Message = $"Failed to create user: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while creating user {Username} in Identity Server", username);
            return new IdentityServerCreateUserResponse
            {
                Success = false,
                Message = $"Exception while creating user: {ex.Message}"
            };
        }
    }

    public async Task<bool> DeleteUserAsync(string username)
    {
        try
        {
            _logger.LogInformation("Deleting user {Username} from Identity Server", username);

            var response = await _httpClient.DeleteAsync($"api/usermanagement/{Uri.EscapeDataString(username)}");
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully deleted user {Username} from Identity Server", username);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to delete user {Username} from Identity Server. Status: {Status}, Response: {Response}", 
                username, response.StatusCode, errorContent);
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while deleting user {Username} from Identity Server", username);
            return false;
        }
    }

    public async Task<bool> DeleteUserByEmailAsync(string email)
    {
        try
        {
            _logger.LogInformation("Deleting user with email {Email} from Identity Server", email);

            var response = await _httpClient.DeleteAsync($"api/usermanagement/by-email/{Uri.EscapeDataString(email)}");
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully deleted user with email {Email} from Identity Server", email);
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to delete user with email {Email} from Identity Server. Status: {Status}, Response: {Response}", 
                email, response.StatusCode, errorContent);
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while deleting user with email {Email} from Identity Server", email);
            return false;
        }
    }
}

internal class IdentityServerApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public string? UserId { get; set; }
}

public class IdentityServerCreateUserResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public string? UserId { get; set; }
}
