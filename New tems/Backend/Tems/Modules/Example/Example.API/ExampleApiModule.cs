using Example.Application.Features;
using Example.Application.Interfaces;
using Example.Infrastructure.BackgroundServices;
using Example.Infrastructure.Persistence.Repositories;
using Example.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tems.Common.Database;

namespace Tems.Example.API;

public static class ExampleApiModule
{
    public static IServiceCollection AddExampleApiModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetWeatherForecastHandler).Assembly));

        services.AddMongoDb(configuration);
        
        services.AddScoped<IWeatherService, WeatherService>();
        services.AddScoped<IWeatherRepository, WeatherRepository>();
        services.AddScoped<IWeatherRepository, WeatherRepository>();

        services.AddHostedService<WeatherDataSyncWorker>();

        return services;
    }
}
