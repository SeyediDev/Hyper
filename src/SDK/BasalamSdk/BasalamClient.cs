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
    ChatService Chat { get; }
    UploadService Uploads { get; }
    AppstoreService Appstore { get; }
    ShippingService Shipping { get; }
    SearchService Search { get; }
    WalletService Wallet { get; }
    StoryService Story { get; }
    CoreService Core { get; }
    OrderProcessingService OrderProcessing { get; }
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
    private readonly IBasalamAuthClient _authClient;
    private TokenInfo? _token = null;
    private readonly object _tokenLock = new();

    public BasalamClient(
        BasalamConfig config,
        ILogger<object>? logger = null,
        System.Net.Http.HttpClient? httpClient = null,
        IBasalamAuthClient? authClient = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger as ILogger<BasalamClient>;
        _httpClient = httpClient;

        var http = httpClient ?? new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds) };
        if (httpClient == null)
            _httpClientOwner = http;

        var httpFactory = new Clients.BasalamHttpClient(config, logger as ILogger<Clients.BasalamHttpClient>, http);
        _transport = httpFactory;
        _authClient = authClient ?? new BasalamAuthClient(config, httpClient: http);

        Vendors = new VendorService(httpFactory, logger as ILogger<Services.VendorService>);
        Products = new ProductService(httpFactory, logger as ILogger<Services.ProductService>);
        Variations = new VariationService(httpFactory, logger as ILogger<Services.VariationService>);
        Catalog = new CatalogService(httpFactory, logger as ILogger<Services.CatalogService>);
        Orders = new OrderService(httpFactory, logger as ILogger<Services.OrderService>);
        Parcels = new ParcelService(httpFactory, logger as ILogger<Services.ParcelService>);
        Customers = new CustomerService(httpFactory, logger as ILogger<Services.CustomerService>);
        Webhooks = new WebhookService(httpFactory, logger as ILogger<Services.WebhookService>);
        Chat = new ChatService(httpFactory, logger as ILogger<Services.ChatService>);
        Uploads = new UploadService(httpFactory, logger as ILogger<Services.UploadService>);
        Appstore = new AppstoreService(httpFactory, logger as ILogger<Services.AppstoreService>);
        Shipping = new ShippingService(httpFactory, logger as ILogger<Services.ShippingService>);
        Search = new SearchService(httpFactory);
        Wallet = new WalletService(httpFactory);
        Story = new StoryService(httpFactory);
        Core = new CoreService(httpFactory);
        OrderProcessing = new OrderProcessingService(httpFactory);
    }

    public VendorService Vendors { get; }
    public ProductService Products { get; }
    public VariationService Variations { get; }
    public CatalogService Catalog { get; }
    public OrderService Orders { get; }
    public ParcelService Parcels { get; }
    public CustomerService Customers { get; }
    public WebhookService Webhooks { get; }
    public ChatService Chat { get; }
    public UploadService Uploads { get; }
    public AppstoreService Appstore { get; }
    public ShippingService Shipping { get; }
    public SearchService Search { get; }
    public WalletService Wallet { get; }
    public StoryService Story { get; }
    public CoreService Core { get; }
    public OrderProcessingService OrderProcessing { get; }

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
        var current = Token;
        var refreshed = string.IsNullOrWhiteSpace(current?.RefreshToken)
            ? await _authClient.GetTokenAsync(ct)
            : await _authClient.RefreshTokenAsync(current.RefreshToken, ct);
        SetToken(refreshed);
        return refreshed;
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
