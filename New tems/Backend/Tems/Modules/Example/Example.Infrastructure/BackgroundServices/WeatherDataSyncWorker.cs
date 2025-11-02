using Example.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Example.Infrastructure.BackgroundServices;

public class WeatherDataSyncWorker(
    ILogger<WeatherDataSyncWorker> logger,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    private const string Location = "Moldova";
    private static readonly TimeSpan SyncInterval = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Weather Data Sync Worker started");

        // Run immediately on startup
        await SyncWeatherDataAsync(stoppingToken);

        // Use PeriodicTimer for cleaner periodic execution
        using var timer = new PeriodicTimer(SyncInterval);

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await SyncWeatherDataAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Weather Data Sync Worker is stopping");
        }
    }

    private async Task SyncWeatherDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting weather data sync for {Location}", Location);

            using var scope = serviceProvider.CreateScope();
            var weatherService = scope.ServiceProvider.GetRequiredService<IWeatherService>();
            var weatherRepository = scope.ServiceProvider.GetRequiredService<IWeatherRepository>();

            // Get weather data from external service
            var forecasts = await weatherService.GetWeatherDataAsync(Location, cancellationToken);

            // Save to database
            await weatherRepository.SaveForecastsAsync(forecasts, Location, cancellationToken);

            logger.LogInformation("Weather data sync completed successfully for {Location}. {Count} forecasts saved", 
                Location, forecasts.Count());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while syncing weather data for {Location}", Location);
        }
    }
}
