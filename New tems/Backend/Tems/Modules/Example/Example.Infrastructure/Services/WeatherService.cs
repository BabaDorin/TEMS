using Example.Application.Interfaces;
using Example.Contract.Features;

namespace Example.Infrastructure.Services;

public class WeatherService : IWeatherService
{
    private static readonly string[] Summaries = 
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<IEnumerable<WeatherForecast>> GetWeatherDataAsync(string location, CancellationToken cancellationToken = default)
    {
        // Simulate external weather service call
        var forecasts = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            ))
            .ToArray();

        return Task.FromResult<IEnumerable<WeatherForecast>>(forecasts);
    }
}
