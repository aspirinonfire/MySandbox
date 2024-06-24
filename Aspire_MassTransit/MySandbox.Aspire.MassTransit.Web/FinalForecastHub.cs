using MassTransit;
using Microsoft.AspNetCore.SignalR;
using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;

namespace MySandbox.Aspire.MassTransit.Web
{
    public sealed class FinalForecastHub: Hub
    {
        public async Task SendFinalForecast(WeatherForecast message)
        {
            await Clients.All.SendAsync(nameof(SendFinalForecast), message);
        }
    }
}
