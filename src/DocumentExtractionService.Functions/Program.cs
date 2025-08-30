using DocumentExtractionService.Application.Extensions;
using DocumentExtractionService.Infrastructure.Extensions;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddApplication();
        services.AddInfrastructure(context.Configuration);

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddApplicationInsights();
        });
    })
    .Build();

host.Run();
