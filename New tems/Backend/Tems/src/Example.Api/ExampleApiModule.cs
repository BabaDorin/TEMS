using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Api;

public static class ExampleApiModule
{
    public static IServiceCollection AddExampleApiModule(this IServiceCollection services)
    {
        services.AddFastEndpoints();
        return services;
    }
}
