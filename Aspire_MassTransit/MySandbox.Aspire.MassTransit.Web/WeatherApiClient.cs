using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;

namespace MySandbox.Aspire.MassTransit.Web;

public class WeatherApiClient(HttpClient httpClient, ILogger<WeatherApiClient> logger)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast", cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }

    public async Task<bool> StartWeatherForecastSaga()
    {
        logger.LogInformation("Starting weather forecast saga...");

        var postResult = await httpClient.PostAsync("/startWeatherForecastSaga", content: null);

        logger.LogInformation("Saga started {isSuccess}", postResult.IsSuccessStatusCode);

        return postResult.IsSuccessStatusCode;
    }
}
