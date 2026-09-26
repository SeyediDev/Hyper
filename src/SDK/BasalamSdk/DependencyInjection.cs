using Basalam.SDK.Auth;
using Basalam.SDK.Config;
using Basalam.SDK.Errors;
using Basalam.SDK.Clients;
using Basalam.SDK.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Basalam.SDK;

public static class DependencyInjection
{
    private const string SectionName = "Basalam";

    public static IServiceCollection AddBasalamSdk(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        var section = configuration is IConfigurationSection selected
            && string.Equals(selected.Key, SectionName, StringComparison.OrdinalIgnoreCase)
                ? selected : configuration.GetSection(SectionName);
        services.Configure<BasalamConfig>(section);
        // The explicit-config overload has already registered the caller's
        // instance; do not replace it with an empty options binding.
        services.TryAddSingleton<BasalamConfig>(sp => sp.GetRequiredService<IOptionsMonitor<BasalamConfig>>().CurrentValue);

        // The SDK client carries per-connection authentication state; it must not be singleton.
        services.TryAddScoped<Clients.IBasalamHttpClient, Clients.BasalamHttpClient>();
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
        services.TryAddScoped<IBasalamClient, BasalamClient>();

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
