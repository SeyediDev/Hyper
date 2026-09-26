using Basalam.SDK.Auth;
using Basalam.SDK.Clients;
using Basalam.SDK.Config;
using Basalam.SDK.Errors;
using Basalam.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Basalam.SDK;

public interface IBasalamClient
{
    VendorService Vendors { get; }
    ProductService Products { get; }
    VariationService Variations { get; }
    CatalogService Catalog { get; }
    OrderService Orders { get; }
    ParcelService Parcels { get; }
    CustomerService Customers { get; }
    WebhookService Webhooks { get; }
    TokenInfo? Token { get; }
    Task<TokenInfo> RefreshTokenAsync(CancellationToken ct = default);
    void SetToken(TokenInfo? token);
}

public sealed class BasalamClient : IBasalamClient, IDisposable
{
    private readonly BasalamConfig _config;
    private readonly ILogger<BasalamClient>? _logger;
    private readonly System.Net.Http.HttpClient? _httpClient;
    private readonly IDisposable? _httpClientOwner;
    private readonly BasalamHttpClient _transport;
    private TokenInfo? _token = null;
    private readonly object _tokenLock = new();

    public BasalamClient(
        BasalamConfig config,
        ILogger<object>? logger = null,
        System.Net.Http.HttpClient? httpClient = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger as ILogger<BasalamClient>;
        _httpClient = httpClient;

        var http = httpClient ?? new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds) };
        if (httpClient == null)
            _httpClientOwner = http;

        var httpFactory = new Clients.BasalamHttpClient(config, logger as ILogger<Clients.BasalamHttpClient>, http);
        _transport = httpFactory;

        Vendors = new VendorService(httpFactory, logger as ILogger<Services.VendorService>);
        Products = new ProductService(httpFactory, logger as ILogger<Services.ProductService>);
        Variations = new VariationService(httpFactory, logger as ILogger<Services.VariationService>);
        Catalog = new CatalogService(httpFactory, logger as ILogger<Services.CatalogService>);
        Orders = new OrderService(httpFactory, logger as ILogger<Services.OrderService>);
        Parcels = new ParcelService(httpFactory, logger as ILogger<Services.ParcelService>);
        Customers = new CustomerService(httpFactory, logger as ILogger<Services.CustomerService>);
        Webhooks = new WebhookService(httpFactory, logger as ILogger<Services.WebhookService>);
    }

    public VendorService Vendors { get; }
    public ProductService Products { get; }
    public VariationService Variations { get; }
    public CatalogService Catalog { get; }
    public OrderService Orders { get; }
    public ParcelService Parcels { get; }
    public CustomerService Customers { get; }
    public WebhookService Webhooks { get; }

    public TokenInfo? Token
    {
        get
        {
            lock (_tokenLock) return _token;
        }
    }

    public async Task<TokenInfo> RefreshTokenAsync(CancellationToken ct = default)
    {
        _logger?.LogInformation("Refreshing Basalam token");
        throw new InvalidOperationException("Auth client not configured for token refresh");
    }

    public void SetToken(TokenInfo? token)
    {
        lock (_tokenLock)
        {
            _token = token;
            // All child services share this transport. Keep their authentication
            // in sync when a booth token is loaded, refreshed or cleared.
            _transport.SetToken(token);
        }
    }

    public void Dispose()
    {
        _httpClientOwner?.Dispose();
    }
}
