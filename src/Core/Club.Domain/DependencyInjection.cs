using Hyper.Domain.Features.Channels;
using Microsoft.Extensions.Configuration;
using Neo.Domain;
using Neo.Domain.Features.Client;

namespace Hyper.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddHyperDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNeoDomainServices(configuration);
        services.AddScoped<IAttributeService, AttributeService>();
        services.AddScoped<IAttributeValueService, AttributeValueService>();
        services.AddScoped<IAttributeAggregationQueryService, AttributeAggregationQueryService>();
        services.AddScoped<IRewardAssetService, RewardAssetService>();
        services.AddScoped<IRewardAssetInternalService, RewardAssetInternalService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ILotteryService, LotteryService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerTenantService, CustomerTenantService>();
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IEventTypeService, EventTypeService>();
        services.AddScoped<IChannelService, ChannelService>();
        services.AddScoped<IPromotionActionService, PromotionActionService>();
        services.AddScoped<IPromotionMetricsService, PromotionMetricsService>();
        services.AddScoped<ISerialGenerator, SerialGenerator>();
        services.AddScoped<IRewardService, RewardService>();
        services.AddScoped<Features.IAwardService, Features.AwardService>();
        services.AddScoped<IPromotionBudgetService, PromotionBudgetService>();
        services.AddScoped<IPointLevelService, PointLevelService>();
        services.AddScoped<IPointTransferService, PointTransferService>();
        services.AddScoped<IEvaluateFormulaService, EvaluateFormulaService>();
        services.AddScoped<ICustomerSegmentService, CustomerSegmentService>();
        services.AddScoped<IReferrerCodeValidationService, ReferrerCodeValidationService>();
        services.AddScoped<ILoginUserService<UserId>, LoginUserService>();
        
        // Theme Service
        services.AddSingleton<IThemeService, ThemeService>();
        
        return services;
    }
}
