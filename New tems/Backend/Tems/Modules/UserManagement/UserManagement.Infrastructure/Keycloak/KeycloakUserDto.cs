using System.Text.Json.Serialization;

namespace UserManagement.Infrastructure.Keycloak;

public class KeycloakUserDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;
    
    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; } = true;
    
    [JsonPropertyName("credentials")]
    public List<KeycloakCredentialDto>? Credentials { get; set; }
}

public class KeycloakCredentialDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "password";
    
    [JsonPropertyName("value")]
    public string? Value { get; set; }
    
    [JsonPropertyName("temporary")]
    public bool Temporary { get; set; } = true;
}

public class KeycloakRoleDto
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("composite")]
    public bool Composite { get; set; }
    
    [JsonPropertyName("clientRole")]
    public bool ClientRole { get; set; }
}

public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
}
