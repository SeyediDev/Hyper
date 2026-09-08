using Neo.Application;
using Hyper.Application.Features.Hyper.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hyper.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddHyperApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNeoApplicationServices(configuration, typeof(DependencyInjection).Assembly);
        
        // Register recurring jobs
        services.AddScoped<IProcessScheduledLotteriesJob, ProcessScheduledLotteriesJob>();
        services.AddScoped<IProcessScheduledPromotionsJob, ProcessScheduledPromotionsJob>();
        services.AddScoped<IScheduleLotteriesJob, ScheduleLotteriesJob>();
        services.AddScoped<IExecuteLotteryJob, ExecuteLotteryJob>();
        
        // Register product services
        services.AddHttpClient();
        
        return services;
    }
}
