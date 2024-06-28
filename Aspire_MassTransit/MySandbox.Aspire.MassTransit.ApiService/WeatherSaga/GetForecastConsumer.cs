using MassTransit;
using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;

namespace MySandbox.Aspire.MassTransit.ApiService.WeatherSaga
{
    /// <summary>
    /// Forecast Consumer that returns a random forecast for the next 5 days
    /// </summary>
    /// <remarks>
    /// Instance of Forecast Consumer
    /// </remarks>
    /// <param name="logger"></param>
    public sealed class GetForecastConsumer(ILogger<GetForecastConsumer> logger) : IConsumer<GetForecast>
    {
        /// <inheritdoc/>
        public async Task Consume(ConsumeContext<GetForecast> context)
        {
            logger.LogInformation("Getting forecaset...");

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
                })
                .ToArray();

            // simulate data access
            await Task.Delay(TimeSpan.FromSeconds(2));

            await context.Publish(new ForecastRetrievedSuccessfully(context.CorrelationId.GetValueOrDefault(), forecast));

            logger.LogInformation("Forecast published...");
        }
    }
}
