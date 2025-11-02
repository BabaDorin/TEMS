using Example.Contract.Features;

namespace Example.Application.Interfaces;

public interface IWeatherService
{
    Task<IEnumerable<WeatherForecast>> GetWeatherDataAsync(string location, CancellationToken cancellationToken = default);
}
