using MediatR;

namespace Example.Contract.Features;

public record GetWeatherForecastQuery : IRequest<GetWeatherForecastResponse>;

public record GetWeatherForecastResponse(WeatherForecast[] Forecasts);

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
