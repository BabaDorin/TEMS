using Example.Contract.Features;
using MediatR;

namespace AviationNavigator.API;

public class Class1(IMediator mediator)
{
    Task GetWeatherForecast()
    {
        var request = new GetWeatherForecastQuery();

        var weatherForecast = await mediator.SendAsync(request);
    }
}