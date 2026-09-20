using Hyper.SDK.Auth;
using Hyper.SDK.Config;
using Hyper.SDK.Errors;
using Hyper.SDK.Clients;
using Hyper.SDK.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hyper.SDK;

public static class DependencyInjection
{
    private const string SectionName = "Basalam";

    public static IServiceCollection AddBasalamSdk(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        services.Configure<BasalamConfig>(section);
        services.AddSingleton<BasalamConfig>(sp => sp.GetRequiredService<IOptionsMonitor<BasalamConfig>>().CurrentValue);

        services.TryAddSingleton<Clients.IBasalamHttpClient, Clients.BasalamHttpClient>();
        services.TryAddSingleton<Clients.IBasalamAuthClient, Clients.BasalamAuthClient>();
        services.TryAddSingleton<Auth.BasalamAuthBase>(sp =>
        {
            var config = sp.GetRequiredService<IOptionsMonitor<BasalamConfig>>().CurrentValue;
            var logger = sp.GetRequiredService<ILogger<Auth.ClientCredentialsAuth>>();
            if (!string.IsNullOrEmpty(config.ClientId) && !string.IsNullOrEmpty(config.ClientSecret))
            {
                return new Auth.ClientCredentialsAuth(config);
            }
            return null!;
        });
        services.TryAddSingleton<IBasalamClient, BasalamClient>();

        services.TryAddScoped<Services.IVendorService, Services.VendorService>();
        services.TryAddScoped<Services.IProductService, Services.ProductService>();
        services.TryAddScoped<Services.IVariationService, Services.VariationService>();
        services.TryAddScoped<Services.ICatalogService, Services.CatalogService>();
        services.TryAddScoped<Services.IOrderService, Services.OrderService>();
        services.TryAddScoped<Services.IParcelService, Services.ParcelService>();
        services.TryAddScoped<Services.ICustomerService, Services.CustomerService>();
        services.TryAddScoped<Services.IWebhookService, Services.WebhookService>();

        return services;
    }

    public static IServiceCollection AddBasalamSdk(this IServiceCollection services, BasalamConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        services.AddSingleton(config);
        services.AddBasalamSdk(new ConfigurationBuilder().Build());
        return services;
    }
}
