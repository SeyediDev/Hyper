using System.Net;
using Basalam.SDK;
using Basalam.SDK.Auth;
using Basalam.SDK.Config;
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
    // Capture all HTTP in memory: these checks never contact a provider.
    var transport = new Capture();
    using var http = new HttpClient(transport);
    using var client = new BasalamClient(new BasalamConfig(), httpClient: http);
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

    var otherTransport = new Capture();
    using var otherHttp = new HttpClient(otherTransport);
    using var otherClient = new BasalamClient(new BasalamConfig(), httpClient: otherHttp);
    otherClient.SetToken(new TokenInfo { AccessToken = "fixture-other", TokenType = "Bearer" });
    await otherClient.Catalog.GetProductsAsync(72);
    await client.Catalog.GetProductsAsync(71);
    Check(otherTransport.LastAuthorization == "Bearer fixture-other"
        && transport.LastAuthorization == "Bearer fixture-refreshed", "clients do not share booth tokens");

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
    public string? LastAuthorization { get; private set; }
    public string? LastBody { get; private set; }
    public Uri? LastUri { get; private set; }
    public HttpMethod? LastMethod { get; private set; }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        LastAuthorization = request.Headers.Authorization?.ToString();
        LastUri = request.RequestUri;
        LastMethod = request.Method;
        LastBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(ct);
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(request.Method == HttpMethod.Get
                ? "{\"data\":[],\"hasMore\":false}" : "{}", System.Text.Encoding.UTF8, "application/json")
        };
    }
}
