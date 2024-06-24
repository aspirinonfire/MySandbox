using MassTransit;
using MySandbox.Aspire.MassTransit.ApiService.WeatherSaga;
using MySandbox.Aspire.MassTransit.ServiceDefaults.Messages;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add masstransit with rabbitmq
// Run sagas and consumers in the same process and use in-memory persistence for this example.
builder.Services.AddMassTransit(busConfig =>
{
    busConfig.SetKebabCaseEndpointNameFormatter();

    busConfig
        .AddSagaStateMachine<WeatherForecastSagaStateMachine, WeatherForecastSagaInstance>()
        .InMemoryRepository();

    busConfig.AddConsumers(typeof(Program).Assembly);

    busConfig.UsingRabbitMq((context, rabbitCfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var host = configuration.GetConnectionString("RabbitMQConnection");

        rabbitCfg.Host(host);
        rabbitCfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapPost("/startWeatherForecastSaga", async (IBus mtBus) =>
{
    await mtBus.Publish(new StartGetWeatherForecastSaga(NewId.NextSequentialGuid()));
});

app.MapDefaultEndpoints();

app.Run();
