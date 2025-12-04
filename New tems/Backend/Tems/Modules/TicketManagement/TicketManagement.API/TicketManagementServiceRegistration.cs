using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketManagement.Application.Commands.TicketTypes;
using TicketManagement.Infrastructure;

namespace TicketManagement.API;

public static class TicketManagementServiceRegistration
{
    public static IServiceCollection AddTicketManagementServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTicketTypeCommandHandler).Assembly));
        
        services.AddValidatorsFromAssemblyContaining<CreateTicketTypeCommandHandler>();

        services.AddInfrastructureServices(configuration);
        
        return services;
    }
}
