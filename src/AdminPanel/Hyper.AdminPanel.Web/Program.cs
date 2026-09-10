using Hyper.AdminPanel.Web;
using Neo.Domain.Features.Telementry;
using Neo.Endpoint.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection(nameof(TelemetryOptions))); 

builder.Host.UseDefaultServiceProvider(
        (_, options) =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

builder.Host.AddNeoSerilog();
builder.Services.AddHyperAdminPanelServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseHyperBpms(builder.Configuration, builder.Environment);
app.UseRecuringJobs();
/*app.Use(async (context, next) =>
{
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "connect-src 'self' http://localhost:* ws://localhost:* http://127.0.0.1:* ws://127.0.0.1:*"
    );
    await next();
});*/
app.Run();
