using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UserManagement.Infrastructure.Keycloak;

public class KeycloakClient : IKeycloakClient
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakSettings _settings;
    private readonly ILogger<KeycloakClient> _logger;
    
    private string? _accessToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public KeycloakClient(HttpClient httpClient, IOptions<KeycloakSettings> settings, ILogger<KeycloakClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
    }

    public async Task<string?> CreateUserAsync(string username, string email, string? firstName = null, string? lastName = null, string? temporaryPassword = null)
    {
        await EnsureAuthenticatedAsync();
        
        var user = new KeycloakUserDto
        {
            Username = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Enabled = true,
            EmailVerified = true
        };

        if (!string.IsNullOrEmpty(temporaryPassword))
        {
            user.Credentials = new List<KeycloakCredentialDto>
            {
                new()
                {
                    Type = "password",
                    Value = temporaryPassword,
                    Temporary = true
                }
            };
        }

        var response = await _httpClient.PostAsJsonAsync($"/admin/realms/{_settings.Realm}/users", user);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to create Keycloak user. Status: {Status}, Error: {Error}", response.StatusCode, error);
            throw new Exception($"Failed to create Keycloak user: {error}");
        }

        // Get the user ID from the Location header
        var locationHeader = response.Headers.Location?.ToString();
        if (string.IsNullOrEmpty(locationHeader))
        {
            // Fallback: fetch user by email
            var createdUser = await GetUserByEmailAsync(email);
            return createdUser?.Id;
        }
        
        // Extract ID from location header (format: .../users/{id})
        var keycloakUserId = locationHeader.Split('/').LastOrDefault();
        _logger.LogInformation("Created Keycloak user with ID: {UserId}", keycloakUserId);
        
        return keycloakUserId;
    }

    public async Task DeleteUserAsync(string keycloakUserId)
    {
        await EnsureAuthenticatedAsync();
        
        var response = await _httpClient.DeleteAsync($"/admin/realms/{_settings.Realm}/users/{keycloakUserId}");
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to delete Keycloak user {UserId}. Status: {Status}, Error: {Error}", keycloakUserId, response.StatusCode, error);
            throw new Exception($"Failed to delete Keycloak user: {error}");
        }
        
        _logger.LogInformation("Deleted Keycloak user with ID: {UserId}", keycloakUserId);
    }

    public async Task<KeycloakUserDto?> GetUserAsync(string keycloakUserId)
    {
        await EnsureAuthenticatedAsync();
        
        var response = await _httpClient.GetAsync($"/admin/realms/{_settings.Realm}/users/{keycloakUserId}");
        
        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get Keycloak user {UserId}. Status: {Status}, Error: {Error}", keycloakUserId, response.StatusCode, error);
            throw new Exception($"Failed to get Keycloak user: {error}");
        }
        
        return await response.Content.ReadFromJsonAsync<KeycloakUserDto>();
    }

    public async Task<KeycloakUserDto?> GetUserByEmailAsync(string email)
    {
        await EnsureAuthenticatedAsync();
        
        var response = await _httpClient.GetAsync($"/admin/realms/{_settings.Realm}/users?email={Uri.EscapeDataString(email)}&exact=true");
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get Keycloak user by email. Status: {Status}, Error: {Error}", response.StatusCode, error);
            throw new Exception($"Failed to get Keycloak user: {error}");
        }
        
        var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserDto>>();
        return users?.FirstOrDefault();
    }

    public async Task<List<KeycloakRoleDto>> GetUserRolesAsync(string keycloakUserId)
    {
        await EnsureAuthenticatedAsync();
        
        var response = await _httpClient.GetAsync($"/admin/realms/{_settings.Realm}/users/{keycloakUserId}/role-mappings/realm");
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get user roles. Status: {Status}, Error: {Error}", response.StatusCode, error);
            throw new Exception($"Failed to get user roles: {error}");
        }
        
        var roles = await response.Content.ReadFromJsonAsync<List<KeycloakRoleDto>>();
        return roles ?? new List<KeycloakRoleDto>();
    }

    public async Task<List<KeycloakRoleDto>> GetAllRolesAsync()
    {
        await EnsureAuthenticatedAsync();
        
        var response = await _httpClient.GetAsync($"/admin/realms/{_settings.Realm}/roles");
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to get all roles. Status: {Status}, Error: {Error}", response.StatusCode, error);
            throw new Exception($"Failed to get all roles: {error}");
        }
        
        var roles = await response.Content.ReadFromJsonAsync<List<KeycloakRoleDto>>();
        
        // Filter out default Keycloak roles that shouldn't be assigned manually
        var excludedRoles = new[] { "offline_access", "uma_authorization", "default-roles-tems" };
        return roles?.Where(r => !excludedRoles.Contains(r.Name)).ToList() ?? new List<KeycloakRoleDto>();
    }

    public async Task AssignRolesToUserAsync(string keycloakUserId, IEnumerable<string> roleNames)
    {
        await EnsureAuthenticatedAsync();
        
        // First get all available roles to get their IDs
        var allRoles = await GetAllRolesAsync();
        var rolesToAssign = allRoles
            .Where(r => roleNames.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (!rolesToAssign.Any())
        {
            _logger.LogWarning("No matching roles found to assign");
            return;
        }

        var response = await _httpClient.PostAsJsonAsync(
            $"/admin/realms/{_settings.Realm}/users/{keycloakUserId}/role-mappings/realm",
            rolesToAssign);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to assign roles. Status: {Status}, Error: {Error}", response.StatusCode, error);
            throw new Exception($"Failed to assign roles: {error}");
        }
        
        _logger.LogInformation("Assigned roles {Roles} to user {UserId}", string.Join(", ", roleNames), keycloakUserId);
    }

    public async Task RemoveRolesFromUserAsync(string keycloakUserId, IEnumerable<string> roleNames)
    {
        await EnsureAuthenticatedAsync();
        
        // First get all available roles to get their IDs
        var allRoles = await GetAllRolesAsync();
        var rolesToRemove = allRoles
            .Where(r => roleNames.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (!rolesToRemove.Any())
        {
            _logger.LogWarning("No matching roles found to remove");
            return;
        }

        var request = new HttpRequestMessage(HttpMethod.Delete, 
            $"/admin/realms/{_settings.Realm}/users/{keycloakUserId}/role-mappings/realm")
        {
            Content = JsonContent.Create(rolesToRemove)
        };
        
        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to remove roles. Status: {Status}, Error: {Error}", response.StatusCode, error);
            throw new Exception($"Failed to remove roles: {error}");
        }
        
        _logger.LogInformation("Removed roles {Roles} from user {UserId}", string.Join(", ", roleNames), keycloakUserId);
    }

    private async Task EnsureAuthenticatedAsync()
    {
        if (_accessToken != null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-1))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            return;
        }

        var tokenUrl = $"/realms/master/protocol/openid-connect/token";
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = _settings.AdminUsername,
            ["password"] = _settings.AdminPassword
        });

        var response = await _httpClient.PostAsync(tokenUrl, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to authenticate with Keycloak. Status: {Status}, Error: {Error}", response.StatusCode, error);
            throw new Exception($"Failed to authenticate with Keycloak: {error}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
        
        if (tokenResponse?.AccessToken == null)
        {
            throw new Exception("No access token received from Keycloak");
        }

        _accessToken = tokenResponse.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        
        _logger.LogDebug("Authenticated with Keycloak, token expires in {ExpiresIn} seconds", tokenResponse.ExpiresIn);
    }
}
