var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddRabbitMQ("RabbitMQConnection");

var apiService = builder.AddProject<Projects.MySandbox_Aspire_MassTransit_ApiService>("apiservice")
    .WithReference(messaging);

builder.AddProject<Projects.MySandbox_Aspire_MassTransit_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(messaging)
    .WithReference(apiService);

builder.Build().Run();
