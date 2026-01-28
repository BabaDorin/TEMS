using AssetManagement.Application.Interfaces;
using AssetManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tems.Common.Database;

namespace AssetManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAssetPropertyRepository, AssetPropertyRepository>();
        services.AddScoped<IAssetTypeRepository, AssetTypeRepository>();
        services.AddScoped<IAssetDefinitionRepository, AssetDefinitionRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        
        services.AddMongoDb(configuration);

        return services;
    }
}
