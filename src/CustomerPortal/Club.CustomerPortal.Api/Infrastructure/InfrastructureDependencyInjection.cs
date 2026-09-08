using Hyper.Application;
using Hyper.CustomerPortal.Api.Services;
using Hyper.Domain;
using Hyper.Infrastructure;
using Hyper.Infrastructure.Features.PubSub;
using Hyper.Infrastructure.Features.Sms.SmsDummy;
using Neo.Application.Features.Outbox;
using Neo.Application.Features.Queue;
using Neo.Domain.Features.Integrations;
using Neo.Domain.Features.ObjectStore;
using Neo.Domain.Features.PubSub;
using Neo.Domain.Features.Sms;

namespace Hyper.CustomerPortal.Api.Infrastructure;

/// <summary>
/// Infrastructure services for CustomerPortal.Api
/// CustomerPortal needs: Database (Read/Write), Cache, ObjectStore (for files/images)
/// Similar to Call Center API, using in-memory services for development
/// </summary>
public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddCustomerPortalInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        // Core Domain & Application Services
        services.AddHyperDomainServices(configuration);
        services.AddHyperApplicationServices(configuration);

        // CORS Policy
        AddCorsPolicy(services, configuration);

		// Database & Repositories
		services.AddHyperRepositories(configuration);

		// In-memory services for development (similar to Call Center API)
		services.AddSingleton<InMemorySmsService>();
		services.AddSmsDummyServices(configuration);
		services.AddSingleton<ISmsService>(sp => sp.GetRequiredService<InMemorySmsService>());
        services.AddSingleton<IOtpService>(sp => sp.GetRequiredService<InMemorySmsService>());
        services.AddSingleton<IObjectStoreService, InMemoryObjectStoreService>();
        services.AddSingleton<IJobExecuter, InMemoryJobExecuter>();
        services.AddSingleton<IDistributedLock, InMemoryDistributedLock>();
        services.AddSingleton<IRecurringJobsManager, NoOpRecurringJobsManager>();
        services.AddSingleton<IExternalApiService, NoopExternalApiService>();
        
        // Recurring Jobs Registration (No-op for CustomerPortal)
        services.AddScoped<IRegisterRecurringJobs, NoOpRegisterRecurringJobs>();
        
        // Publisher (MediatR)
        services.AddScoped<INeoPublisher, MediatRNeoPublisher>();
        
        return services;
    }

    private static void AddCorsPolicy(IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration["CustomerPortal:AllowedOrigins"];
        if (origins is not null)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowCustomerPortal", policy =>
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
}

