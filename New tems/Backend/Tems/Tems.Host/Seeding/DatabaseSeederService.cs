using MongoDB.Driver;

namespace Tems.Host.Seeding;

public class DatabaseSeederService(
    IServiceProvider serviceProvider,
    ILogger<DatabaseSeederService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting database seeding...");

        using var scope = serviceProvider.CreateScope();

        // Use the configured IMongoDatabase from DI instead of hardcoding database name
        var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

        await RunSeederStepAsync("location management", () => SeedLocationManagementAsync(database));
        await RunSeederStepAsync("asset management", () => SeedAssetManagementAsync(database));
        await RunSeederStepAsync("ticket management", () => SeedTicketManagementAsync(database));
        await RunSeederStepAsync("bulk equipment", () => SeedBulkEquipmentAsync(database));

        logger.LogInformation("Database seeding completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task SeedLocationManagementAsync(IMongoDatabase database)
    {
        var seeder = new LocationManagementSeeder(database, serviceProvider.GetRequiredService<ILogger<LocationManagementSeeder>>());
        await seeder.SeedAsync();
    }

    private async Task SeedAssetManagementAsync(IMongoDatabase database)
    {
        var seeder = new AssetManagementSeeder(database, serviceProvider.GetRequiredService<ILogger<AssetManagementSeeder>>());
        await seeder.SeedAsync();
    }

    private async Task SeedBulkEquipmentAsync(IMongoDatabase database)
    {
        var seeder = new BulkEquipmentSeeder(database, serviceProvider.GetRequiredService<ILogger<BulkEquipmentSeeder>>());
        await seeder.SeedAsync();
    }

    private async Task SeedTicketManagementAsync(IMongoDatabase database)
    {
        var seeder = new TicketManagementSeeder(database, serviceProvider.GetRequiredService<ILogger<TicketManagementSeeder>>());
        await seeder.SeedAsync();
    }

    private async Task RunSeederStepAsync(string stepName, Func<Task> seedAction)
    {
        try
        {
            await seedAction();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding {StepName}. Continuing with remaining seeders.", stepName);
        }
    }
}
