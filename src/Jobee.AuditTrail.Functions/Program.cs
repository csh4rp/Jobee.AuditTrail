using Jobee.AuditTrail.Functions.Services;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddTransient<AuditLogsService>()
            .AddTransient<EventLogsService>();
        
        services.AddSingleton(s => {
            return new CosmosClientBuilder("")
                .Build();
        });
    })
    .Build();

host.Run();