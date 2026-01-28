using AssetManagement.Application.Commands;
using AssetManagement.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AssetManagement.API;

public static class AssetManagementServiceRegistration
{
    public static IServiceCollection AddAssetManagementServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateAssetPropertyCommandHandler).Assembly));

        services.AddInfrastructureServices(configuration);
        
        return services;
    }
}
