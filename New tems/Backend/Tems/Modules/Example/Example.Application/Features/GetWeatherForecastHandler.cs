using Example.Application.Interfaces;
using Example.Contract.Features;
using MediatR;

namespace Example.Application.Features;

public class GetWeatherForecastHandler(IWeatherRepository weatherRepository)
    : IRequestHandler<GetWeatherForecastQuery, GetWeatherForecastResponse>
{
    public async Task<GetWeatherForecastResponse> Handle(GetWeatherForecastQuery request, CancellationToken cancellationToken)
    {
        var forecasts = await weatherRepository.GetAllForecastsAsync(cancellationToken);
        return new GetWeatherForecastResponse(forecasts.ToArray());
    }
}
