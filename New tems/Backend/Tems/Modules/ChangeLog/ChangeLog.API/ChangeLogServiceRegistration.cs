using ChangeLog.Application.Queries;
using ChangeLog.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeLog.API;

public static class ChangeLogServiceRegistration
{
    public static IServiceCollection AddChangeLogServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetEntityTimelineQueryHandler).Assembly));
        services.AddChangeLogInfrastructure(configuration);
        return services;
    }
}
