using MassTransit;
using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;

namespace MySandbox.Aspire.MassTransit.ApiService.WeatherSaga
{
    public sealed class GetForecastConsumer : IConsumer<GetForecast>
    {
        public GetForecastConsumer(ILogger<GetForecastConsumer> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<GetForecastConsumer> _logger;

        public async Task Consume(ConsumeContext<GetForecast> context)
        {
            _logger.LogInformation("Getting forecaset...");

            var forecast = Enumerable.Range(1, 5)
                .Select(index =>
                {
                    var temperature = Random.Shared.Next(-20, 55);
                    var description = temperature switch
                    {
                        <0 => "Freezing",
                        
                        (>=0) and (<5) => "Bracing",

                        (>=5) and (<10) => "Chilly",
                        
                        (>=10) and (<15) => "Cool",
                        
                        (>=15) and (<20) => "Mild",
                        
                        (>=20) and (<25) => "Warm",

                        (>= 25) and (<30) => "Balmy",
                        
                        (>= 30) and (<35) => "Hot",
                        
                        (>= 35) and (<40) => "Swelting",

                        _ => "Scorching"
                    };


                    return new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        temperature,
                        description
                    );
                }).ToArray();

            await context.Publish(new ForecastRetrievedSuccessfully(context.CorrelationId.GetValueOrDefault(), forecast));

            _logger.LogInformation("Forecast published...");
        }
    }
}
