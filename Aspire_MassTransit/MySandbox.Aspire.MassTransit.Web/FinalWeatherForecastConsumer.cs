using MassTransit;
using Microsoft.AspNetCore.SignalR;
using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;

namespace MySandbox.Aspire.MassTransit.Web
{
    public sealed class FinalWeatherForecastConsumer : IConsumer<FinalWeatherForecast>
    {
        private readonly IHubContext<FinalForecastHub> _signalRHub;

        public FinalWeatherForecastConsumer(IHubContext<FinalForecastHub> signalRHub)
        {
            _signalRHub = signalRHub;
        }

        public async Task Consume(ConsumeContext<FinalWeatherForecast> context)
        {
            await _signalRHub.Clients.All.SendAsync("SendFinalForecast", context.Message);
        }
    }
}
