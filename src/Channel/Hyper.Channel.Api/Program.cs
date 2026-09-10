using Hyper.Channel.Api;
using Microsoft.AspNetCore.HttpOverrides;
using Neo.Domain.Features.Telementry;
using Neo.Endpoint.Infrastructure;
using Neo.Infrastructure.Features.Telementry;

var builder = WebApplication.CreateBuilder(args);

// تنظیمات پایه
builder.Services.Configure<TelemetryOptions>(builder.Configuration.GetSection(nameof(TelemetryOptions)));
builder.Host.AddNeoSerilog();
builder.Services.AddNeoOpenTelementry(builder.Configuration);
builder.Services.AddHyperChannelApiServices(builder.Configuration, builder.Environment);

// سوگر ساده
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

var app = builder.Build();

// Forwarded Headers
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
	ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// خطایابی
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hyper Channel API v1");
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
	app.UseExceptionHandler("/error");
	app.UseHsts();
}

// میدلورهای استاندارد
app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// غیرفعال کردن BrowserLink برای مسیر Monitoring
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/Monitoring"))
    {
        // غیرفعال کردن BrowserLink برای Monitoring
        context.Response.Headers.Remove("X-BrowserLink");
    }
    await next();
});

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseRecuringJobs();

app.MapControllers();

app.Run();

// Partial class برای دسترسی از Integration Tests
namespace Hyper.Channel.Api
{
    public partial class Program { }
}
