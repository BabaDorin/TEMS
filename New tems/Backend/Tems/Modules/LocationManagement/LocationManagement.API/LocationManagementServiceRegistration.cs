using LocationManagement.Application.Commands;
using LocationManagement.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LocationManagement.Api;

public static class LocationManagementServiceRegistration
{
    public static IServiceCollection AddLocationManagementModule(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateSiteCommandHandler).Assembly));
        services.AddLocationManagementInfrastructure();

        return services;
    }
}
