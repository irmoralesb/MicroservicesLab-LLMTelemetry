using Azure.Data.Tables;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton(sp =>
{
    var tableStorageUrl = Environment.GetEnvironmentVariable("TableStorageAccountUrl");
    
    if (tableStorageUrl == null)
        throw new InvalidOperationException("TableStorageAccountUrl environment variable is not set.");

    return new TableServiceClient(new Uri(tableStorageUrl), new DefaultAzureCredential());
});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
