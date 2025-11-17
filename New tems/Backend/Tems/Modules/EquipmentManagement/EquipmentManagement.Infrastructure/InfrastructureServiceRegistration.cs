using EquipmentManagement.Application.Interfaces;
using EquipmentManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tems.Common.Database;

namespace EquipmentManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEquipmentPropertyRepository, EquipmentPropertyRepository>();
        
        services.AddMongoDb(configuration);

        return services;
    }
}
