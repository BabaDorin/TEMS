using Example.Contract.Features;
using FastEndpoints;
using MediatR;

namespace Tems.Example.API.Endpoints.WeatherForecast;

public class GetWeatherForecastEndpoint(IMediator mediator) : EndpointWithoutRequest<GetWeatherForecastResponse>
{
    public override void Configure()
    {
        Get("/weatherforecast");
        Policies("CanViewEntities");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetWeatherForecastQuery();
        var result = await mediator.Send(query, ct);
        
        await SendAsync(result, cancellation: ct);
    }
}
