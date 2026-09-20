using Hyper.Infrastructure.Features.Integrations;
using Hyper.SDK;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
if (Environment.GetEnvironmentVariable("HYPER_SETTINGS_FILE") is { Length: > 0 } settings)
    builder.Configuration.AddJsonFile(Path.GetFullPath(settings), optional: false).AddEnvironmentVariables();
var connection = builder.Configuration.GetConnectionString("Domain");
if (string.IsNullOrWhiteSpace(connection))
    throw new InvalidOperationException("Configure ConnectionStrings__Domain or HYPER_SETTINGS_FILE for the integration worker.");
builder.Services.AddBasalamSdk(builder.Configuration.GetSection("Basalam"));
builder.Services.AddHyperIntegrations(connection);
builder.Services.Configure<IntegrationInventoryCaptureOptions>(builder.Configuration.GetSection("IntegrationInventoryCapture"));
builder.Services.AddHostedService<Hyper.IntegrationWorker.Application.IntegrationWorker>();
await builder.Build().RunAsync();
