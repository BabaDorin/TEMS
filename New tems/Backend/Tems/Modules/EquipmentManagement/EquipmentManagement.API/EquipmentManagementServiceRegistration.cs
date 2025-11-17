using EquipmentManagement.Application.Commands;
using EquipmentManagement.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EquipmentManagement.API;

public static class EquipmentManagementServiceRegistration
{
    public static IServiceCollection AddEquipmentManagementServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateEquipmentPropertyCommandHandler).Assembly));

        services.AddInfrastructureServices(configuration);
        
        return services;
    }
}
