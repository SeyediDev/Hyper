using Hyper.AdminPanel.Web;
using Neo.Domain.Features.Telementry;

var builder = WebApplication.CreateBuilder(args);
Hyper.AdminPanel.Web.Infrastructure.AdminRuntime.ConfigureLocalSql(builder);

builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection(nameof(TelemetryOptions))); 

builder.Host.UseDefaultServiceProvider(
        (_, options) =>
        {
            // The full Neo/Hyper service graph is large. Validating every scoped
            // descriptor before the first request adds noticeable Development
            // startup latency; production keeps the stricter validation.
            options.ValidateOnBuild = builder.Environment.IsProduction();
            options.ValidateScopes = true;
        });

builder.Host.AddNeoSerilog();
builder.Services.AddHyperAdminPanelServices(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseHyperBpms(builder.Configuration, builder.Environment);
/*app.Use(async (context, next) =>
{
    context.Response.Headers.Append(
        "Content-Security-Policy",
        "connect-src 'self' http://localhost:* ws://localhost:* http://127.0.0.1:* ws://127.0.0.1:*"
    );
    await next();
});*/
app.Run();
