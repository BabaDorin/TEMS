using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Application.Handlers;
using UserManagement.Infrastructure;

namespace UserManagement.API;

public static class UserManagementServiceRegistration
{
    public static IServiceCollection AddUserManagementServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetOrCreateProfileQueryHandler).Assembly));

        services.AddInfrastructureServices(configuration);
        
        return services;
    }
}
