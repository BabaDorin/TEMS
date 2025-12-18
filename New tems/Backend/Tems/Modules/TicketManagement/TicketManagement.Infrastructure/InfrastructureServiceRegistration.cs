using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketManagement.Application.Interfaces;
using TicketManagement.Infrastructure.Repositories;
using Tems.Common.Database;

namespace TicketManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITicketConversationRepository, TicketConversationRepository>();
        
        services.AddMongoDb(configuration);

        return services;
    }
}
