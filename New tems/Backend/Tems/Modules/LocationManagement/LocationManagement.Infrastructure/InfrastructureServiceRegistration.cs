using LocationManagement.Application.Interfaces;
using LocationManagement.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LocationManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddLocationManagementInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IBuildingRepository, BuildingRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();

        return services;
    }
}
