using MongoDB.Driver;

namespace Tems.Host.Seeding;

public class DatabaseSeederService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseSeederService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting database seeding...");

        try
        {
            using var scope = serviceProvider.CreateScope();
            
            // Use the configured IMongoDatabase from DI instead of hardcoding database name
            var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            await SeedAssetManagementAsync(database);
            await SeedTicketManagementAsync(database);

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding database.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task SeedAssetManagementAsync(IMongoDatabase database)
    {
        var seeder = new AssetManagementSeeder(database, serviceProvider.GetRequiredService<ILogger<AssetManagementSeeder>>());
        await seeder.SeedAsync();
    }

    private async Task SeedTicketManagementAsync(IMongoDatabase database)
    {
        var seeder = new TicketManagementSeeder(database, serviceProvider.GetRequiredService<ILogger<TicketManagementSeeder>>());
        await seeder.SeedAsync();
    }
}
