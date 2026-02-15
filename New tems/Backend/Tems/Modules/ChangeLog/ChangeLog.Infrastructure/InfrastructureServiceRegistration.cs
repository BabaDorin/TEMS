using ChangeLog.Application.Interfaces;
using ChangeLog.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tems.Common.Database;

namespace ChangeLog.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddChangeLogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IChangeLogRepository, ChangeLogRepository>();
        services.AddMongoDb(configuration);
        return services;
    }
}
