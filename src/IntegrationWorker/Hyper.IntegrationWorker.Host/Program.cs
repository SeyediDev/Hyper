using Hyper.Infrastructure.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.SDK;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
if (Environment.GetEnvironmentVariable("HYPER_SETTINGS_FILE") is { Length: > 0 } settings)
{
    var settingsPath = Path.GetFullPath(settings);
    var settingsDirectory = Path.GetDirectoryName(settingsPath)!;
    // Development overrides are layered on the adjacent base settings so the
    // worker receives both the demo Basalam endpoints and the SQL connection.
    builder.Configuration.SetBasePath(settingsDirectory)
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile(Path.GetFileName(settingsPath), optional: false)
        .AddEnvironmentVariables();
}
var connection = builder.Configuration.GetConnectionString("Domain")
    ?? builder.Configuration.GetConnectionString("DomainCommandConnection")
    ?? builder.Configuration.GetConnectionString("default");
if (string.IsNullOrWhiteSpace(connection))
    throw new InvalidOperationException("Configure ConnectionStrings__Domain or HYPER_SETTINGS_FILE for the integration worker.");
var integrationConnection = builder.Configuration.GetConnectionString("IntegrationConnection") ?? connection;
builder.Services.AddBasalamSdk(builder.Configuration.GetSection("Basalam"));
// The worker does not use the full AdminPanel repository registration, but the
// Basalam adapter still needs the encrypted OAuth token store.
builder.Services.AddDbContext<HyperContextCommand>(options => options.UseSqlServer(connection));
builder.Services.Configure<BasalamOAuthSettings>(builder.Configuration.GetSection("Basalam"));
builder.Services.AddDataProtection();
builder.Services.AddHttpClient<BasalamOAuthService>();
builder.Services.AddScoped<BasalamOAuthStore>();
builder.Services.AddHyperIntegrations(connection, integrationConnection, builder.Configuration);
builder.Services.Configure<IntegrationInventoryCaptureOptions>(builder.Configuration.GetSection("IntegrationInventoryCapture"));
builder.Services.AddHostedService<Hyper.IntegrationWorker.Application.IntegrationWorker>();
await builder.Build().RunAsync();
