namespace Hyper.AdminPanel.Web.Infrastructure;
/// <summary>
/// Infrastructure services for AdminPanel.Web
/// AdminPanel needs full infrastructure: Database, Cache, ObjectStore, Hangfire, SMS, etc.
/// </summary>
public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddAdminPanelInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        // Core Domain Services
        services.AddHyperDomainServices(configuration);
        services.AddNeoInfrastructureServices(configuration, environment);
        
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
        
        // Publisher (MediatR)
        services.AddScoped<INeoPublisher, MediatRNeoPublisher>();
        
        return services;
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

        
        return services;
    }
}

