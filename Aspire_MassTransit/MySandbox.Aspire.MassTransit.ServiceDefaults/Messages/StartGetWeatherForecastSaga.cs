using MassTransit;

namespace MySandbox.Aspire.MassTransit.ServiceDefaults.Messages
{
    public sealed record StartGetWeatherForecastSaga(Guid CorrelationId) : CorrelatedBy<Guid>;

    public sealed record GetForecast(Guid CorrelationId) : CorrelatedBy<Guid>;

    public sealed record ForecastRetrievedSuccessfully(Guid CorrelationId, WeatherForecast[] WeatherForecast) : CorrelatedBy<Guid>;

    public sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
