using Hyper.Domain;
using Hyper.Infrastructure;
using Hyper.Infrastructure.Configuration;
using Hyper.Infrastructure.Features.PubSub;
using Hyper.Infrastructure.Features.Sms.SmsDummy;
using Neo.Application.Features.Queue;
using Neo.Application.Features.Queue.Implementation.NoOp;
using Neo.Domain.Features.PubSub;
using Neo.Infrastructure;
using Neo.Infrastructure.Features.Cache;
using Neo.Infrastructure.Features.Outbox;
using Neo.Infrastructure.Features.Queue.Hangfire;

namespace Hyper.Channel.Api.Infrastructure;

/// <summary>
/// Infrastructure services for Channel.Api
/// Channel API receives events from external systems and needs:
/// - Database (Write mostly - saving events, transactions)
/// - Cache (for performance)
/// - SMS (for sending notifications to customers)
/// Does NOT need: ObjectStore (no file uploads), Hangfire (uses parent system)
/// </summary>
public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddChannelInfrastructureServices(
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
        
        // Queue / Hangfire for Outbox scheduling (fallback to no-op if configuration not provided)
        if (configuration.GetSection("Hangfire").Exists())
        {
            services.AddNeoHangfire(configuration);
        }
        else
        {
            services.AddSingleton<IJobExecuter, NoOpJobExecuter>();
            services.AddSingleton<IRecurringJobsManager, NoOpRecurringJobsManager>();
            services.AddSingleton<ICronJobManager, NoOpCronJobManager>();
        }
        
        // CORS Policy
        AddCorsPolicy(services, configuration);

		// Database & Repositories
		services.AddHyperRepositories(configuration);

		// Feature Services (SMS for notifications)
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
        
        // SMS Services (Channel API needs to send notifications)
        services.AddSmsDummyServices(configuration);
        
        // NO Recurring Jobs - managed by AdminPanel
        
        return services;
    }
}