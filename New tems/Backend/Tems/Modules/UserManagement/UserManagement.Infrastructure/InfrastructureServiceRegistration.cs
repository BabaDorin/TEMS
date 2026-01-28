using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tems.Common.Database;
using UserManagement.Infrastructure.Repositories;

namespace UserManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddMongoDb(configuration);

        return services;
    }
}
