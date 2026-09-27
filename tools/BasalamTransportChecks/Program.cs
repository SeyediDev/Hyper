using System.Net;
using System.Text.Json;
using Basalam.SDK;
using Basalam.SDK.Auth;
using Basalam.SDK.Clients;
using Basalam.SDK.Config;
using Basalam.SDK.Models;
using Basalam.SDK.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var failures = new List<string>();
var checks = 0;
void Check(bool condition, string name)
{
    checks++;
    if (!condition) failures.Add(name);
    Console.WriteLine($"{(condition ? "PASS" : "FAIL")}: {name}");
}

try
{
    await CatalogPageChecks.Run(Check);
    // Capture all HTTP in memory: these checks never contact a provider.
    var transport = new Capture();
    using var http = new HttpClient(transport);
    var auth = new FixtureAuthClient();
    using var client = new BasalamClient(new BasalamConfig(), httpClient: http, authClient: auth);
    client.SetToken(new TokenInfo { AccessToken = "fixture-first", TokenType = "Bearer" });
    await client.Catalog.GetProductsAsync(71);
    Check(transport.LastAuthorization == "Bearer fixture-first", "catalog receives the connection token");
    Check(transport.LastUri?.Scheme == "https" && transport.LastUri.Host == "openapi.basalam.com"
        && transport.LastUri.AbsolutePath == "/v1/vendors/71/products", "relative SDK route resolves to production HTTPS");

    client.SetToken(new TokenInfo { AccessToken = "fixture-refreshed", TokenType = "Bearer" });
    await client.Products.PatchStockAsync(12, 0);
    Check(transport.LastAuthorization == "Bearer fixture-refreshed", "inventory PATCH receives refreshed token");
    Check(transport.LastBody == "{\"stock\":0}" && transport.LastMethod == HttpMethod.Patch,
        "zero inventory is an explicit absolute PATCH");
    await client.Products.PatchDetailsAsync(12, "New name", 250);
    using (var patch = JsonDocument.Parse(transport.LastBody!))
        Check(patch.RootElement.GetProperty("name").GetString() == "New name"
            && patch.RootElement.GetProperty("primary_price").GetInt64() == 250
            && patch.RootElement.EnumerateObject().Count() == 2,
            "product details use official wire names and cannot mutate inventory");
    await client.Products.PatchDetailsAsync(12, null, 0);
    Check(transport.LastBody == "{\"primary_price\":0}", "price-only patch omits name and preserves explicit zero");
    await client.Products.PatchDetailsAsync(12, "Name only", null);
    Check(transport.LastBody == "{\"name\":\"Name only\"}", "name-only patch omits price");

    client.SetToken(new TokenInfo { AccessToken = "old", TokenType = "Bearer", RefreshToken = "refresh-me" });
    var refreshed = await client.RefreshTokenAsync();
    Check(refreshed.AccessToken == "new-access" && client.Token?.AccessToken == "new-access"
        && auth.LastRefreshToken == "refresh-me", "client refreshes and propagates the refreshed token");

    transport.ResponseBody = "{\"id\":12,\"name\":\"Written\",\"vendorId\":71}";
    await client.Products.UpdateProductAsync(12, new ProductWriteRequest("Written", 71, Price: 10));
    Check(transport.LastUri?.AbsolutePath == "/v1/products/12" && transport.LastMethod == HttpMethod.Patch,
        "typed product update uses the official product route");
    transport.ResponseBody = "{\"id\":8,\"productId\":12,\"vendorId\":71}";
    await client.Variations.UpdateVariationAsync(8, new VariationWriteRequest(12, Title: "Blue"));
    Check(transport.LastUri?.AbsolutePath == "/v1/variations/8" && transport.LastMethod == HttpMethod.Patch,
        "typed variation update uses the official variation route");
    transport.ResponseBody = "{\"id\":91,\"status\":\"paid\",\"totalAmount\":1250,\"parcels\":[{\"id\":7,\"orderId\":91,\"status\":\"ready\"}]}";
    var orderSnapshot = await client.Orders.GetOrderSnapshotAsync(91);
    Check(orderSnapshot?.Id == 91 && orderSnapshot.TotalAmount == 1250 && orderSnapshot.Parcels.Count == 1,
        "typed order snapshot maps stable lifecycle fields");
    transport.ResponseBody = "{\"id\":7,\"orderId\":91,\"status\":\"ready\",\"trackingCode\":\"TRK-1\"}";
    var parcelSnapshot = await client.Parcels.GetParcelSnapshotAsync(7);
    Check(parcelSnapshot?.OrderId == 91 && parcelSnapshot.TrackingCode == "TRK-1",
        "typed parcel snapshot maps order and tracking identity");
    transport.ResponseBody = "{\"id\":3,\"unseen\":1}";
    await client.Chat.GetUnseenCountAsync();
    Check(transport.LastUri?.AbsolutePath == "/v1/chats/unseen-count" && transport.LastMethod == HttpMethod.Get,
        "chat unseen count uses the official route");
    await client.Chat.CreateMessageAsync(new ChatMessageRequest(3, "text", new { text = "fixture" }));
    Check(transport.LastUri?.AbsolutePath == "/v1/chats/3/messages" && transport.LastMethod == HttpMethod.Post,
        "chat message uses the scoped official route");
    transport.ResponseBody = null;

    transport.Statuses.Enqueue(HttpStatusCode.ServiceUnavailable);
    transport.Statuses.Enqueue(HttpStatusCode.ServiceUnavailable);
    transport.Statuses.Enqueue(HttpStatusCode.OK);
    transport.RetryAfterZero = true;
    await client.Products.PatchStockAsync(12, 7);
    Check(transport.Requests >= 3 && transport.LastBody == "{\"stock\":7}",
        "transient provider responses retry the same PATCH and honor Retry-After");

    var otherTransport = new Capture();
    using var otherHttp = new HttpClient(otherTransport);
    using var otherClient = new BasalamClient(new BasalamConfig(), httpClient: otherHttp);
    otherClient.SetToken(new TokenInfo { AccessToken = "fixture-other", TokenType = "Bearer" });
    await otherClient.Catalog.GetProductsAsync(72);
    await client.Catalog.GetProductsAsync(71);
    Check(otherTransport.LastAuthorization == "Bearer fixture-other"
        && transport.LastAuthorization == "Bearer new-access", "clients do not share booth tokens");

    client.SetToken(null);
    await client.Catalog.GetProductsAsync(71);
    Check(transport.LastAuthorization is null && client.Token is null, "clearing token also clears transport authentication");

    var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Basalam:ApiBaseUrl"] = "https://configured.fixture.invalid/",
        ["Basalam:TimeoutSeconds"] = "17"
    }).Build();
    foreach (var useSection in new[] { false, true })
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddBasalamSdk(useSection ? configuration.GetSection("Basalam") : configuration);
        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<BasalamConfig>();
        Check(options.ApiBaseUrl == "https://configured.fixture.invalid/" && options.TimeoutSeconds == 17,
            useSection ? "DI binds an already-selected Basalam section" : "DI binds root configuration");
        using var a = provider.CreateScope();
        using var b = provider.CreateScope();
        Check(!ReferenceEquals(a.ServiceProvider.GetRequiredService<IBasalamClient>(),
            b.ServiceProvider.GetRequiredService<IBasalamClient>()), "DI isolates SDK client scopes");
    }
    var explicitConfig = new BasalamConfig { TimeoutSeconds = 19, ApiBaseUrl = "https://explicit.fixture.invalid/" };
    var explicitServices = new ServiceCollection();
    explicitServices.AddLogging();
    explicitServices.AddBasalamSdk(explicitConfig);
    using var explicitProvider = explicitServices.BuildServiceProvider();
    Check(ReferenceEquals(explicitConfig, explicitProvider.GetRequiredService<BasalamConfig>()),
        "explicit configuration overload preserves the supplied instance");

    transport.ResponseBody = "{\"id\":12,\"title\":\"Real shape\",\"vendor\":{\"id\":71},\"inventory\":0,\"price\":250}";
    var product = await client.Catalog.GetProductAsync(12);
    Check(product is { Id: 12, VendorId: 71, Name: "Real shape", Stock: 0, Price: 250 },
        "official product wire shape maps title, vendor ownership and zero inventory");
    transport.ResponseBody = "{\"data\":[{\"id\":12,\"title\":\"List item\",\"inventory\":4}],\"total_count\":2,\"page\":1,\"total_page\":2,\"per_page\":1}";
    var products = await client.Catalog.GetProductsAsync(71, 1, 1);
    Check(products.HasMore && products.TotalPages == 2 && products.Total == 2 && products.PerPage == 1
        && products.Data.Single().VendorId == 71, "snake_case pagination continues to the next official catalog page");
    transport.ResponseBody = "{\"data\":[{\"id\":12,\"title\":\"Unknown stock\"}]}";
    products = await client.Catalog.GetProductsAsync(71);
    Check(products.HasMore && products.Data.Single().Stock is null, "missing pagination continues until empty and missing stock is not zero");
    transport.ResponseBody = "{\"data\":[]}";
    Check(!(await client.Catalog.GetProductsAsync(71)).HasMore, "empty catalog page stops pagination");
    transport.ResponseBody = "{\"id\":12,\"title\":\"No ownership\"}";
    try { await client.Catalog.GetProductAsync(12); Check(false, "product detail must identify its vendor"); }
    catch (System.Text.Json.JsonException) { Check(true, "product detail must identify its vendor"); }
    transport.ResponseBody = "{}";
    try { await client.Catalog.GetProductsAsync(71); Check(false, "malformed page cannot masquerade as an empty catalog"); }
    catch (System.Text.Json.JsonException) { Check(true, "malformed page cannot masquerade as an empty catalog"); }
    Console.WriteLine($"{checks - failures.Count}/{checks} transport checks passed. No network or database writes.");
    return failures.Count == 0 ? 0 : 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Transport check failed: {ex.GetType().Name}: {ex.Message}");
    return 1;
}

sealed class Capture : HttpMessageHandler
{
    public Queue<HttpStatusCode> Statuses { get; } = new();
    public bool RetryAfterZero { get; set; }
    public int Requests { get; private set; }
    public string? ResponseBody { get; set; }
    public string? LastAuthorization { get; private set; }
    public string? LastBody { get; private set; }
    public Uri? LastUri { get; private set; }
    public HttpMethod? LastMethod { get; private set; }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        Requests++;
        LastAuthorization = request.Headers.Authorization?.ToString();
        LastUri = request.RequestUri;
        LastMethod = request.Method;
        LastBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(ct);
        var status = Statuses.Count == 0 ? HttpStatusCode.OK : Statuses.Dequeue();
        var response = new HttpResponseMessage(status)
        {
            Content = new StringContent(ResponseBody ?? (request.Method == HttpMethod.Get
                ? "{\"data\":[],\"hasMore\":false}" : "{}"), System.Text.Encoding.UTF8, "application/json")
        };
        if (RetryAfterZero && status != HttpStatusCode.OK)
            response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.Zero);
        return response;
    }
}

sealed class FixtureAuthClient : IBasalamAuthClient
{
    public string? LastRefreshToken { get; private set; }
    public Task<TokenInfo> GetTokenAsync(CancellationToken ct = default) =>
        Task.FromResult(new TokenInfo { AccessToken = "new-access", TokenType = "Bearer" });
    public Task<TokenInfo> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        LastRefreshToken = refreshToken;
        return Task.FromResult(new TokenInfo { AccessToken = "new-access", TokenType = "Bearer", RefreshToken = "new-refresh" });
    }
}
