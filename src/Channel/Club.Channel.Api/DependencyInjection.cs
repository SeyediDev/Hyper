using Hyper.Channel.Api.Infrastructure;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.Extensions.FileProviders;
using Neo.Application;
using Neo.Domain.Features.Client;
using Neo.Endpoint;
using Neo.Infrastructure.Features.Client;
using Neo.Infrastructure.Features.Telementry;

namespace Hyper.Channel.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddHyperChannelApiServices(this IServiceCollection services, 
		IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IRequesterUser, RequesterUser>();
        
        // اضافه کردن پشتیبانی از Views برای Monitoring (قبل از AddNeoControllerServices)
        var mvcBuilder = services.AddNeoMonitoringViews(environment as Microsoft.AspNetCore.Hosting.IWebHostEnvironment);
        
        // اضافه کردن مسیر Views از output directory این پروژه
        // این لازم است چون Views در output directory این پروژه کپی می‌شوند
        // AddNeoMonitoringViews فقط مسیرهای Neo.Endpoint را اضافه می‌کند، اما Views در output این پروژه هستند
        
        // اضافه کردن FileProvider برای Runtime Compilation (فقط در Development)
        // در ASP.NET Core 8.0، FileProviders فقط در MvcRazorRuntimeCompilationOptions موجود است، نه در RazorViewEngineOptions
        if (environment.IsDevelopment() && environment is IWebHostEnvironment webEnv)
        {
            mvcBuilder.AddRazorRuntimeCompilation(options =>
            {
                var baseDirectory = AppContext.BaseDirectory;
                
                if (!string.IsNullOrEmpty(baseDirectory))
                {
                    var outputViewsPath = Path.Combine(baseDirectory, "Views");
                    
                    if (Directory.Exists(outputViewsPath))
                    {
                        // اضافه کردن به FileProviders (نه Clear کردن - چون AddNeoMonitoringViews قبلاً تنظیم کرده)
                        options.FileProviders.Add(new PhysicalFileProvider(outputViewsPath));
                    }
                }
            });
        }
        
        // AddNeoControllerServices با mvcBuilder موجود برای حفظ view services
        services.AddNeoControllerServices(configuration, "Hyper Channel API", includeViews: true, existingMvcBuilder: mvcBuilder);

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddHealthChecks()
            .AddDbContextCheck<HyperContextCommand>()
            .AddDbContextCheck<HyperContextQuery>();


		services.AddNeoApplicationServices(
			configuration,
			typeof(Neo.Application.DependencyInjection).Assembly,
			typeof(Neo.Endpoint.DependencyInjection).Assembly,
			typeof(Application.Features.Channel.Commands.EnqueueBulkEventsCommand).Assembly
			);

		// زیرساخت کانال
		services.AddChannelInfrastructureServices(configuration, environment);
		services.AddNeoOpenTelementry(configuration);
		services.AddNeoAuthentication(configuration);
		services.AddNeoAuthorization(configuration);
		return services;
    }
}
