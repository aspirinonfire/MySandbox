using MassTransit;
using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;

namespace MySandbox.Aspire.MassTransit.ApiService.WeatherSaga
{
    public class GetForecastConsumer : IConsumer<GetForecast>
    {
        public GetForecastConsumer(ILogger<GetForecastConsumer> logger)
        {
            _logger = logger;
        }

        private readonly static string[] summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];
        private readonly ILogger<GetForecastConsumer> _logger;

        public async Task Consume(ConsumeContext<GetForecast> context)
        {
            _logger.LogInformation("Getting forecaset...");

            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();

            await context.Publish(new ForecastRetrievedSuccessfully(context.CorrelationId.GetValueOrDefault(), forecast));

            _logger.LogInformation("Forecast published...");
        }
    }
}
