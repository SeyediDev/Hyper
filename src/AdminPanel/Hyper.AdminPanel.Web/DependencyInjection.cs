using System.IO;
using Hyper.AdminPanel.Web.Infrastructure.Icons;
using Neo.Bpms.Api;
using Neo.Bpms.UI.MVC.Controls;
using Neo.Bpms.UI.MVC.Features;
using Neo.Endpoint;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

namespace Hyper.AdminPanel.Web;

public static class DependencyInjection
{
    public static void AddHyperAdminPanelServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddSingleton<AdminSimulationTickets>();
        // Core Domain Services
        services.AddHyperDomainServices(configuration);

        if (environment.IsDevelopment())
        {
            var keysPath = Path.Combine(environment.ContentRootPath, "DataProtection-keys");
            Directory.CreateDirectory(keysPath);
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
                .SetApplicationName("Hyper.AdminPanel");

            // Neo defaults the panel cookie to SecurePolicy.Always. Local HTTP
            // development must be able to send the cookie back to AdminDashboard.
            services.PostConfigure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme,
                options => options.Cookie.SecurePolicy = CookieSecurePolicy.None);
        }

        services.AddHyperApplicationServices(configuration);

        services.AddNeoInfrastructureServices(configuration, environment);
        services.AddNeoAuthorization(configuration);
        services.AddNeoOpenTelementry(configuration);
        services.AddNeoBpmsInfrastructure(configuration);

        // Cache Services (Memory for Dev, Redis for Production)
        if (environment.IsDevelopment())
        {
            services.AddNeoMemoryCacheServices(configuration);
            services.AddNeoOutboxWithCatch(configuration);
        }
        else
        {
            services.AddNeoRedisCacheServices(configuration);
            services.AddNeoOutboxWithMongo(configuration);
        }

        // Object Storage (MinIO)
        services.AddNeoMinIo(configuration);

        // Background Jobs (Hangfire)
        services.AddAdminJobContracts(configuration);

        // CORS Policy
        AddCorsPolicy(services, configuration);

        // Database & Repositories
        services.AddHyperRepositories(configuration);

        // Feature Services (SMS, Jobs, etc.)
        AddFeatureServices(services, configuration);

        services.AddBpmsMVC(configuration);
        
        // Add Neo.Bpms.Api services (Monitoring, Dashboard, etc.)
        services.AddNeoBpmsApi(configuration, options =>
        {
            options.EnableMonitoring = true;
            options.EnableDashboard = true;
        });
        
        // اضافه کردن پشتیبانی از Views برای Monitoring از Neo.Endpoint
        var mvcBuilder = services.AddNeoMonitoringViews(environment as Microsoft.AspNetCore.Hosting.IWebHostEnvironment);
        
        // اضافه کردن MonitoringController از Neo.Endpoint برای API endpoints
        // این لازم است تا API endpoints (/api/monitoring/*) در دسترس باشند
        services.AddNeoControllerServices(configuration, "Hyper Admin Panel", includeViews: true, existingMvcBuilder: mvcBuilder);
        
        // Custom Icon Provider for Hyper platform
        services.AddSingleton<ICustomIconProvider, CustomIconProvider>();
        
        // Register MenuHelper for dependency injection
        services.AddScoped<IMenuHelper, MenuHelper>();
        
        // HttpClientFactory for Channel API calls
        services.AddHttpClient();
        
        // Performance Optimizations: Compression & Caching
        services.AddPerformanceOptimizations();
        
        // Database Caching (با قابلیت غیرفعال‌سازی)
        services.AddDatabaseCaching(configuration);
        
        SpecificCommonlyNeededAssets.SetNeededResources("~/Content/images/login-logo.svg",
            "~/Content/common-assets-includes/icons/svgSprite.svg#Hyper-svg-icon-header");
    }

    public static void UseHyperBpms(this IApplicationBuilder app,
        IConfiguration configuration, IHostEnvironment environment, BpmsMVCConfigurationOptions options = null!)
    {
        // Integration work runs in the independent worker; admin does not expose the legacy Hangfire dashboard.

        // Performance Optimizations: must be called BEFORE UseBpmsMVC
        // This ensures UseResponseCompression and UseStaticFiles (with caching) are registered first
        app.UsePerformanceOptimizations(environment);

        // Configure MenuHelper static HttpContextAccessor before UseBpmsMVC
        var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
        MenuHelper.SetHttpContextAccessor(httpContextAccessor);

        app.UseBpmsMVC(configuration, options);
        
        // Map Neo.Bpms.Api endpoints (SignalR hubs, etc.)
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapNeoBpmsApiEndpoints();
            endpoints.MapNeoEndpoints();
        });
        
        DependencyInjectionHolder.Instance.SsoIntegrator = null;//Inject<SsoIntegrator>(app);//FOR SSO
    }

    private static void AddCorsPolicy(IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["AllowedOrigins"];
        if (origins is not null)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(origins.Split(','))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    }

    private static IServiceCollection AddFeatureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(options => configuration.Bind(options));

        // SMS Services (AdminPanel needs SMS for sending OTP to users)
        services.AddSmsDummyServices(configuration);

        // Recurring Jobs (AdminPanel manages background jobs)


        services.AddScoped<INeoPublisher, MediatRNeoPublisher>();
        _ = services.AddScoped<ICmmnDocument, CmmnDocument>();
        _ = services.AddSingleton<IProjectMetaLoader, ProjectMetaLoader<HyperProjectDefinition, HyperServiceDefinitions, HyperMenuDefinitions>>();
        _ = services.AddSingleton<IDataProviderContainer, DataProviderContainer>();

        _ = services.AddScoped<IRequesterUser, RequesterUser>();
        _ = services.AddScoped<IIdentityUserService, IdentityUserService>();
        return services;
    }
}
