namespace UserManagement.Infrastructure.Keycloak;

public class KeycloakSettings
{
    public const string SectionName = "Keycloak";
    
    public string BaseUrl { get; set; } = "http://localhost:8080";
    public string Realm { get; set; } = "tems";
    public string AdminUsername { get; set; } = "admin";
    public string AdminPassword { get; set; } = "admin";
}
