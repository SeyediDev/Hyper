using Hyper.Domain;
using Hyper.Infrastructure;
using Neo.Application.Features.Queue;
using Neo.Application.Features.Queue.Implementation.NoOp;
using Neo.Endpoint;
using Neo.Infrastructure;
using Neo.Infrastructure.Features.Cache;
using Neo.Infrastructure.Features.Outbox;
using Neo.Infrastructure.Features.Queue.Hangfire;

namespace Hyper.EventHandler.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEventHandlerInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
		services.AddHttpContextAccessor();
		services.AddNeoControllerServices(configuration, "Hyper Channel Event Handler API");

		services.AddHealthChecks();
		
        services.AddHyperDomainServices(configuration);
        services.AddNeoInfrastructureServices(configuration, environment);

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

		services.AddHyperRepositories(configuration);

		return services;
    }
}