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

    // register saga state machine with in-memory persistence.
    // use durable storage in prod (sql, mongo, etc)
    busConfig
        .AddSagaStateMachine<WeatherForecastSagaStateMachine, WeatherForecastSagaInstance>()
        .InMemoryRepository();

    // register all consumers in current project
    busConfig.AddConsumers(typeof(Program).Assembly);

    // use RabbitMQ as message transport
    busConfig.UsingRabbitMq((context, rabbitCfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var host = configuration.GetConnectionString("RabbitMQConnection");

        rabbitCfg.Host(host);
        rabbitCfg.ConfigureEndpoints(context);
    });

    // configure MT host options
    busConfig
        .AddOptions<MassTransitHostOptions>()
        .Configure(options =>
        {
            options.WaitUntilStarted = true;
            options.StartTimeout = TimeSpan.FromSeconds(30);
            options.StopTimeout = TimeSpan.FromSeconds(30);
            options.ConsumerStopTimeout = TimeSpan.FromSeconds(30);
        });
});

var app = builder.Build();

// TODO configure and register necessary middleware - authN/Z, compression, etc

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// register API route to start weather forecast
app.MapPost("/startWeatherForecastSaga", async (IBus mtBus) =>
{
    var sagaId = NewId.NextSequentialGuid();
    await mtBus.Publish(new StartGetWeatherForecastSaga(sagaId));
    return Results.Accepted(uri: null, value: sagaId);
});

app.MapDefaultEndpoints();

app.Run();
