﻿using MassTransit;
using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;
using System.Text.Json;

namespace MySandbox.Aspire.MassTransit.ApiService.WeatherSaga
{
    public class WeatherForecastSagaStateMachine : MassTransitStateMachine<WeatherForecastSagaInstance>
    {
        public WeatherForecastSagaStateMachine(ILogger<WeatherForecastSagaStateMachine> logger)
        {
            // Define the property on the saga instance that will track the state of the saga throughout its lifecycle.
            InstanceState(x => x.CurrentState);

            // We don't need to define event correlations because each event for this saga implements CorrelatedBy<Guid> interface

            Initially(
                When(OnStartGetWeatherForecast)
                    .Publish(context => new GetForecast(context.CorrelationId.GetValueOrDefault()))
                    .Then(_ =>
                    {
                        logger.LogInformation("Started Weather Forecast Saga...");
                    })
                    .TransitionTo(RetrievingForecast));

            During(RetrievingForecast,
                When(OnForecastRetrievedSuccessfully)
                    .Then(context =>
                    {
                        var forecastJson = JsonSerializer.Serialize(context.Message.WeatherForecast,
                            new JsonSerializerOptions()
                            {
                                WriteIndented = true,
                            });
                        logger.LogInformation("Got Forecast:\n{forecast}", forecastJson);
                    })
                    .TransitionTo(Final));
        }

        public State RetrievingForecast { get; private set; } = null!;

        public Event<StartGetWeatherForecastSaga> OnStartGetWeatherForecast { get; private set; } = null!;

        public Event<ForecastRetrievedSuccessfully> OnForecastRetrievedSuccessfully { get; private set; } = null!;
    }

    public class WeatherForecastSagaInstance : SagaStateMachineInstance
    {
        /// <inheritdoc/>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Current Saga State
        /// </summary>
        public string CurrentState { get; set; } = null!;
    }
}