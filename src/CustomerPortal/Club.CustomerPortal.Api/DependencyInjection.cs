using Neo.Domain.Features.Client;
using Neo.Endpoint;
using Hyper.CustomerPortal.Application.Interfaces;
using Microsoft.Extensions.FileProviders;

namespace Hyper.CustomerPortal.Api;

public static class DependencyInjection
{
    private static IConfiguration? _configuration;
    
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        _configuration = configuration;
        
        services.AddHttpContextAccessor();
        services.AddScoped<IRequesterUser, CustomerRequesterUser>();
        services.AddScoped<ICustomerRequesterUser, CustomerRequesterUser>();
        
        // اضافه کردن پشتیبانی از Views برای Monitoring (قبل از AddNeoControllerServices)
        var mvcBuilder = services.AddNeoMonitoringViews(environment as Microsoft.AspNetCore.Hosting.IWebHostEnvironment);
        
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
        services.AddNeoControllerServices(configuration, "Hyper Customer Portal API", includeViews: true, existingMvcBuilder: mvcBuilder);

        services.AddDatabaseDeveloperPageExceptionFilter();

        // Health checks for this API. In this environment we avoid EF-specific
        // AddDbContextCheck extensions to prevent build-time issues with the
        // EntityFrameworkCore health checks assembly; a basic health check
        // registration is sufficient for liveness.
        services.AddHealthChecks();

        // Configure CORS for Customer Portal
        services.AddCors(options =>
        {
            options.AddPolicy("AllowCustomerPortal", policy =>
            {
                var origins = _configuration?.GetSection("CustomerPortal:AllowedOrigins").Value?.Split(',') 
                    ?? ["http://localhost:3000"];
                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
