using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Integration.Contracts;
using Basalam.SDK;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hyper.Infrastructure.Features.Integrations;

public static class IntegrationServiceRegistration
{
    // Shared by API, admin and independent worker, without any Neo base-table repository.
    public static IServiceCollection AddHyperIntegrations(this IServiceCollection services,
        string platformConnectionString, string? integrationConnectionString = null,
        IConfiguration? configuration = null)
    {
        services.AddDbContext<HyperSqlServerContext>(options => options.UseSqlServer(platformConnectionString));
        services.AddDbContext<HyperIntegrationContext>(options => options.UseSqlServer(
            string.IsNullOrWhiteSpace(integrationConnectionString) ? platformConnectionString : integrationConnectionString));
        services.AddScoped<IIntegrationSynchronizationService, IntegrationSynchronizationService>();
        services.AddScoped<IIntegrationScenarioQueue, IntegrationScenarioQueue>();
        services.AddScoped<IntegrationScenarioProcessor>();
        services.AddScoped<IntegrationBusinessEventDispatcher>();
        services.AddScoped<IIntegrationEngagementPort, UnregisteredIntegrationEngagementPort>();
        services.AddScoped<MediatR.INotificationHandler<IntegrationScenarioRequested>, IntegrationScenarioSubscriber>();
        services.AddScoped<IIntegrationOutbox, IntegrationOutbox>();
        services.AddScoped<IIntegrationAccountingEventIngress, IntegrationAccountingEventIngress>();
        services.AddScoped<IBasalamWebhookRegistration, BasalamWebhookRegistration>();
        // The admin OAuth/simulation controllers consume this concrete store.
        // Keep it registered here with the rest of the Integration composition
        // so AdminPanel and the independent worker resolve the same boundary.
        services.AddScoped<BasalamOAuthStore>();
        services.AddOptions<IntegrationInventoryCaptureOptions>();
        services.AddScoped<IIntegrationInventoryCapture, IntegrationInventoryCapture>();
        services.AddScoped<IIntegrationStrategyResolver, IntegrationStrategyResolver>();
        services.AddScoped<BasalamDemoProvisioner>();
        services.AddScoped<IIntegrationShopAccess, IntegrationShopAccess>();
        services.AddScoped<IIntegrationPlatformShopPort, HyperyekPlatformShopAdapter>();
        services.AddScoped<IIntegrationPlatformCatalogPort, HyperyekPlatformShopAdapter>();
        services.AddOptions<IntegrationCustomerOptions>();
        services.AddScoped<IIntegrationAccountingPort, HyperyekAccountingCustomerAdapter>();
        services.AddScoped<IIntegrationCustomerRegistration, IntegrationCustomerRegistration>();
        services.AddScoped<IAdminMerchantSimulationService, AdminMerchantSimulationService>();
        services.AddScoped<IIntegrationDashboardQuery, IntegrationDashboardQuery>();
        services.AddScoped<IAdminOverviewQuery, AdminOverviewQuery>();
        services.AddSingleton<IIntegrationWebhookVerifier, IntegrationWebhookVerifier>();
        services.AddScoped<IIntegrationWebhookIngress, IntegrationWebhookIngress>();
        services.AddScoped<IIntegrationManagementApi, IntegrationManagementApi>();
        services.AddScoped<IIntegrationMappingApi, IntegrationMappingApi>();
        services.AddScoped<IIntegrationSyncApi, IntegrationSyncApi>();
        services.AddScoped<IIntegrationDashboardApi, IntegrationDashboardApi>();
        services.AddScoped<IIntegrationTokenApi, IntegrationTokenApi>();
        services.AddSingleton<IIntegrationScopeAuthorization, IntegrationScopeAuthorization>();
        services.AddHttpClient("ExternalIntegrations", client => client.Timeout = TimeSpan.FromSeconds(30))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
        services.AddHttpClient<IIntegrationBusinessCommandPort, HyperyekAccountingApiClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<HyperyekAccountingApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseAddress, UriKind.Absolute);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddOptions<HyperyekAccountingApiOptions>();
        if (configuration is not null)
        {
            services.Configure<BasalamOAuthSettings>(configuration.GetSection("Basalam"));
            services.AddDataProtection();
            services.AddBasalamSdk(configuration.GetSection("Basalam"));
            services.AddHttpClient<BasalamOAuthService>()
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
                .ConfigureHttpClient(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("HyperIntegration/1.0");
                });
        }
        services.AddScoped<IExternalIntegrationAdapter, BasalamSdkAdapter>();
        services.AddSingleton<IExternalIntegrationAdapter, DigikalaIntegrationAdapter>();
        services.AddSingleton<IExternalIntegrationAdapter, TorobIntegrationAdapter>();
        return services;
    }
}
