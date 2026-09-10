using Hyper.CustomerPortal.Application.Features.Plans.Jobs;
using Neo.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomerPortalApplicationServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddNeoApplicationServices(
            configuration, 
            typeof(DependencyInjection).Assembly);
        
        // Register Mock Service implementations for testing
        // TODO: Replace with real implementations from Infrastructure layer
        services.AddScoped<ICustomerService, Services.MockCustomerService>();
        services.AddScoped<IAuthenticationService, Services.MockAuthenticationService>();
        services.AddScoped<IPointService, Services.MockPointService>();
        services.AddScoped<IRewardService, Services.MockRewardService>();
        services.AddScoped<IReferralService, Services.MockReferralService>();
        services.AddScoped<IPromotionService, Services.PromotionService>();
        services.AddScoped<ILotteryService, Services.MockLotteryService>();
        services.AddScoped<ISurveyService, Services.MockSurveyService>();
        services.AddScoped<IDashboardService, Services.MockDashboardService>();
        services.AddScoped<IPlanService, Services.PlanService>();
        services.AddScoped<IWheelService, Services.WheelService>();
        services.AddScoped<IProductService, Services.ProductService>();
        services.AddScoped<IExpireCustomerPlansJob, ExpireCustomerPlansJob>();
        
        return services;
    }
}


