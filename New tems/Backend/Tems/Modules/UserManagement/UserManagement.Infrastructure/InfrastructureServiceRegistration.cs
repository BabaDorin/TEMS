using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tems.Common.Database;
using UserManagement.Infrastructure.IdentityServer;
using UserManagement.Infrastructure.Keycloak;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddMongoDb(configuration);
        
        // Configure Keycloak client
        services.Configure<KeycloakSettings>(configuration.GetSection(KeycloakSettings.SectionName));
        services.AddHttpClient<IKeycloakClient, KeycloakClient>();
        
        // Configure Identity Server client
        // WORKAROUND: This client is temporary and will be removed in the future
        // when user creation is handled exclusively by Keycloak
        services.Configure<IdentityServerSettings>(configuration.GetSection(IdentityServerSettings.SectionName));
        services.AddHttpClient<IIdentityServerClient, IdentityServerClient>();

        return services;
    }
}
