using Example.Contract.Features;

namespace Example.Application.Interfaces;

public interface IWeatherRepository
{
    Task<IEnumerable<WeatherForecast>> GetAllForecastsAsync(CancellationToken cancellationToken = default);
    Task SaveForecastsAsync(IEnumerable<WeatherForecast> forecasts, string location, CancellationToken cancellationToken = default);
}
