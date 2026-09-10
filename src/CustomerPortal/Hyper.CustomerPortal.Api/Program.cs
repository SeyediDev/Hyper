using Neo.Domain.Features.Telementry;
using Neo.Endpoint.Infrastructure;
using Neo.Infrastructure.Features.Client;
using Neo.Infrastructure.Features.Telementry;
using Hyper.CustomerPortal.Api;
using Hyper.CustomerPortal.Api.Infrastructure;
using Hyper.CustomerPortal.Api.Middlewares;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection(nameof(TelemetryOptions)));
// Use CustomerPortal-specific infrastructure services
builder.Services.AddCustomerPortalInfrastructureServices(builder.Configuration, builder.Environment);
builder.Services.AddCustomerPortalApplicationServices(builder.Configuration);

builder.Host.AddNeoSerilog();
builder.Services.AddNeoOpenTelementry(builder.Configuration);
builder.Services.AddNeoAuthentication(builder.Configuration);
builder.Services.AddNeoAuthorization(builder.Configuration);

builder.Services.AddWebServices(builder.Configuration, builder.Environment);

// سوگر ساده
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hyper Customer Portal API v1");
        c.RoutePrefix = "api"; // دسترسی از /api
        c.DisplayRequestDuration();
        
        // اضافه کردن لینک مانیتورینگ
        c.HeadContent = @"
            <script>
                window.addEventListener('load', function() {
                    var monitoringLink = document.createElement('a');
                    monitoringLink.href = '/monitoring';
                    monitoringLink.target = '_blank';
                    monitoringLink.className = 'btn';
                    monitoringLink.style.cssText = 'position: fixed; top: 10px; right: 10px; z-index: 9999; background: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; font-weight: bold;';
                    monitoringLink.textContent = '📊 مانیتورینگ';
                    document.body.appendChild(monitoringLink);
                });
            </script>
        ";
    });
}
else
{
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseCors("AllowCustomerPortal");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRecuringJobs();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<UserAgentLoggingMiddleware>();

app.UseExceptionHandler(options => { });

app.MapControllers();

app.Run();

namespace Hyper.CustomerPortal.Api
{
    public partial class Program { }
}

