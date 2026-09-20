using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hyper.Infrastructure.Features.Integrations;

public static class IntegrationServiceRegistration
{
    // Shared by API, admin and independent worker, without any Neo base-table repository.
    public static IServiceCollection AddHyperIntegrations(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<HyperSqlServerContext>(options => options.UseSqlServer(connectionString));
        services.AddDbContext<HyperIntegrationContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IIntegrationSynchronizationService, IntegrationSynchronizationService>();
        services.AddScoped<IIntegrationScenarioQueue, IntegrationScenarioQueue>();
        services.AddScoped<IntegrationScenarioProcessor>();
        services.AddScoped<MediatR.INotificationHandler<IntegrationScenarioRequested>, IntegrationScenarioSubscriber>();
        services.AddScoped<IIntegrationOutbox, IntegrationOutbox>();
        services.AddOptions<IntegrationInventoryCaptureOptions>();
        services.AddScoped<IIntegrationInventoryCapture, IntegrationInventoryCapture>();
        services.AddScoped<IIntegrationStrategyResolver, IntegrationStrategyResolver>();
        services.AddScoped<IIntegrationShopAccess, IntegrationShopAccess>();
        services.AddScoped<IAdminMerchantSimulationService, AdminMerchantSimulationService>();
        services.AddScoped<IIntegrationDashboardQuery, IntegrationDashboardQuery>();
        services.AddScoped<IAdminOverviewQuery, AdminOverviewQuery>();
        services.AddSingleton<IIntegrationWebhookVerifier, IntegrationWebhookVerifier>();
        services.AddHttpClient("ExternalIntegrations", client => client.Timeout = TimeSpan.FromSeconds(30))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
        services.AddScoped<IExternalIntegrationAdapter, BasalamSdkAdapter>();
        services.AddSingleton<IExternalIntegrationAdapter, DigikalaIntegrationAdapter>();
        services.AddSingleton<IExternalIntegrationAdapter, TorobIntegrationAdapter>();
        return services;
    }
}
