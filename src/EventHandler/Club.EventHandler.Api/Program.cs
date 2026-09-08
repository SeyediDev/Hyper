using Hyper.Application;
using Hyper.EventHandler.Api.Infrastructure;
using Hyper.EventHandler.Application;
using Neo.Domain.Features.Telementry;
using Neo.Endpoint.Infrastructure;
using Neo.Infrastructure.Features.Telementry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection(nameof(TelemetryOptions)));
builder.Services.AddHyperApplicationServices(builder.Configuration);
builder.Services.AddEventHandlerApplication();
builder.Services.AddEventHandlerInfrastructureServices(builder.Configuration, builder.Environment);

builder.Host.AddNeoSerilog();
builder.Services.AddNeoOpenTelementry(builder.Configuration);

var app = builder.Build();

app.UseHealthChecks("/health");
app.UseRecuringJobs();

app.MapGet("/", () => Results.Ok("Event handler online"));

// Monitoring endpoint - استفاده از MonitoringController از Neo.Endpoint
// این endpoint از طریق AddNeoControllerServices و MapControllers در دسترس است

app.MapControllers();

app.Run();
