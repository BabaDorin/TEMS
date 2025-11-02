using Example.Application.Interfaces;
using Example.Contract.Features;
using Example.Infrastructure.Persistence.Entities;
using MongoDB.Driver;

namespace Example.Infrastructure.Persistence.Repositories;

public class WeatherRepository(IMongoDatabase database) : IWeatherRepository
{
    private readonly IMongoCollection<WeatherForecastEntity> _collection = database.GetCollection<WeatherForecastEntity>("WeatherForecasts");

    public async Task<IEnumerable<WeatherForecast>> GetAllForecastsAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _collection
            .Find(_ => true)
            .SortBy(x => x.Date)
            .ToListAsync(cancellationToken);

        return entities.Select(MapToModel);
    }

    public async Task SaveForecastsAsync(IEnumerable<WeatherForecast> forecasts, string location, CancellationToken cancellationToken = default)
    {
        // Delete existing forecasts for this location
        await _collection.DeleteManyAsync(x => x.Location == location, cancellationToken);

        // Insert new forecasts
        var entities = forecasts.Select(f => MapToEntity(f, location));
        await _collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }

    private static WeatherForecast MapToModel(WeatherForecastEntity entity)
    {
        return new WeatherForecast(
            DateOnly.FromDateTime(entity.Date),
            entity.TemperatureC,
            entity.Summary
        );
    }

    private static WeatherForecastEntity MapToEntity(WeatherForecast model, string location)
    {
        return new WeatherForecastEntity
        {
            Date = model.Date.ToDateTime(TimeOnly.MinValue),
            TemperatureC = model.TemperatureC,
            Summary = model.Summary,
            Location = location,
            LastUpdated = DateTime.UtcNow
        };
    }
}
