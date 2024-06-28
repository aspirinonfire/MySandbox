using MassTransit;
using Microsoft.AspNetCore.SignalR;
using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;

namespace MySandbox.Aspire.MassTransit.Web
{
    /// <summary>
    /// SignalR hub to publish weather forecasts
    /// </summary>
    public sealed class FinalForecastHub: Hub
    {
        /// <summary>
        /// Publis weather forecast to all connected clients
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishFinalForecast(WeatherForecast message)
        {
            await Clients.All.SendAsync(nameof(PublishFinalForecast), message);
        }
    }
}
