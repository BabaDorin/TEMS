using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Tems.IdentityServer.Config;

public static class IdentityConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
            // Roles are managed by Keycloak, not Duende
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            // API scope without role claims - roles managed by Keycloak
            new ApiScope("tems-api", "TEMS API")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // Angular SPA Client (Direct - not used when Keycloak is active)
            new Client
            {
                ClientId = "tems-angular-spa",
                ClientName = "TEMS Angular SPA",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                
                RedirectUris = 
                {
                    "http://localhost:4200/callback",
                    "http://localhost:4200/silent-refresh.html"
                },
                PostLogoutRedirectUris = { "http://localhost:4200" },
                AllowedCorsOrigins = { "http://localhost:4200" },
                
                AllowedScopes = 
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "tems-api"
                },
                
                AccessTokenLifetime = 900, // 15 minutes
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                SlidingRefreshTokenLifetime = 2592000, // 30 days
                
                AllowOfflineAccess = true,
                
                // Allow access token via browser (for development)
                AllowAccessTokensViaBrowser = true
            },
            
            // Keycloak Broker Client
            new Client
            {
                ClientId = "keycloak-broker",
                ClientName = "Keycloak Identity Broker",
                ClientSecrets = { new Secret("keycloak-secret".Sha256()) },
                
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = false, // Keycloak doesn't send PKCE by default
                RequireClientSecret = true,
                
                RedirectUris = 
                {
                    "http://localhost:8080/realms/tems/broker/duende-idp/endpoint"
                },
                PostLogoutRedirectUris = { "http://localhost:8080/realms/tems" },
                AllowedCorsOrigins = { "http://localhost:8080" },
                
                AllowedScopes = 
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email
                    // Roles managed by Keycloak only
                },
                
                AccessTokenLifetime = 900,
                AllowOfflineAccess = true,
                AllowAccessTokensViaBrowser = true
            }
        };
}
