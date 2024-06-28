using MassTransit;
using Microsoft.AspNetCore.SignalR;
using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;

namespace MySandbox.Aspire.MassTransit.Web
{
    /// <summary>
    /// Forecast Saga completion message consumer
    /// </summary>
    public sealed class FinalWeatherForecastConsumer : IConsumer<FinalWeatherForecast>
    {
        private readonly IHubContext<FinalForecastHub> _signalRHub;

        /// <summary>
        /// Instance of Forecast Saga completion consumer
        /// </summary>
        /// <param name="signalRHub"></param>
        public FinalWeatherForecastConsumer(IHubContext<FinalForecastHub> signalRHub)
        {
            _signalRHub = signalRHub;
        }

        /// <inheritdoc/>
        public async Task Consume(ConsumeContext<FinalWeatherForecast> context)
        {
            await _signalRHub.Clients.All.SendAsync("PublishFinalForecast", context.Message);
        }
    }
}
