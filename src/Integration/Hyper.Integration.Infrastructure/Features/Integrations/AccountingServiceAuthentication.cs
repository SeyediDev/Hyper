using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hyper.Infrastructure.Features.Integrations;

public static class AccountingClientRegistration
{
    public const string ApiClient = "HyperyekAccountingService";
    public const string TokenClient = "HyperyekAccountingToken";

    public static IServiceCollection AddHyperyekAccountingClients(this IServiceCollection services, IConfiguration? configuration)
    {
        var options = services.AddOptions<HyperyekAccountingApiOptions>();
        if (configuration is not null) options.Bind(configuration.GetSection("HyperyekAccountingApi"));
        services.AddSingleton<AccountingServiceTokenProvider>();
        services.AddTransient<AccountingServiceTokenHandler>();
        services.AddHttpClient(TokenClient, client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.MaxResponseContentBufferSize = 64 * 1024;
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });
        services.AddHttpClient(ApiClient, (sp, client) =>
        {
            var settings = sp.GetRequiredService<IOptions<HyperyekAccountingApiOptions>>().Value;
            // Resolve panel/worker dependencies without requiring a live service
            // setup. Fail closed on use, inside the authentication handler.
            client.BaseAddress = Uri.TryCreate(settings.BaseAddress, UriKind.Absolute, out var address) ? address : null;
            client.Timeout = TimeSpan.FromSeconds(30);
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false })
            .AddHttpMessageHandler<AccountingServiceTokenHandler>();
        services.AddScoped(sp => new HyperyekAccountingApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient(ApiClient)));
        services.AddScoped<IIntegrationBusinessCommandPort>(sp => sp.GetRequiredService<HyperyekAccountingApiClient>());
        services.AddScoped<IIntegrationAccountingPort>(sp => sp.GetRequiredService<HyperyekAccountingApiClient>());
        services.AddScoped<IIntegrationPlatformShopPort>(sp => sp.GetRequiredService<HyperyekAccountingApiClient>());
        services.AddScoped<IIntegrationPlatformCatalogPort>(sp => sp.GetRequiredService<HyperyekAccountingApiClient>());
        services.AddScoped<IIntegrationPlatformOverviewPort>(sp => sp.GetRequiredService<HyperyekAccountingApiClient>());
        services.AddScoped<IIntegrationFinancialPreviewPort>(sp =>
            new AccountingFinancialPreviewClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient(ApiClient)));
        return services;
    }

    internal static void Validate(HyperyekAccountingApiOptions options)
    {
        if (!SecureUri(options.BaseAddress) || !options.BaseAddress.EndsWith('/')
            || !SecureUri(options.TokenEndpoint) || string.IsNullOrWhiteSpace(options.ClientId)
            || string.IsNullOrWhiteSpace(options.ClientSecret) || string.IsNullOrWhiteSpace(options.Scope))
            throw new InvalidOperationException("AccountingServiceConfigurationInvalid");
    }

    private static bool SecureUri(string value) => Uri.TryCreate(value, UriKind.Absolute, out var uri)
        && uri.Scheme == Uri.UriSchemeHttps && string.IsNullOrEmpty(uri.UserInfo)
        && string.IsNullOrEmpty(uri.Query) && string.IsNullOrEmpty(uri.Fragment);
}

// Dedicated service credential/cache, usable in Worker without HttpContext.
// Does not share Neo's admin-client token cache or any merchant Basalam token.
public sealed class AccountingServiceTokenProvider(IHttpClientFactory clients, IOptions<HyperyekAccountingApiOptions> settings) : IDisposable
{
    private readonly SemaphoreSlim gate = new(1, 1);
    private string? token;
    private DateTimeOffset refreshAt;

    public async Task<string> GetAsync(CancellationToken ct)
    {
        await gate.WaitAsync(ct);
        try
        {
            if (token is not null && DateTimeOffset.UtcNow < refreshAt) return token;
            var options = settings.Value;
            AccountingClientRegistration.Validate(options);
            var requestedAt = DateTimeOffset.UtcNow;
            using var http = clients.CreateClient(AccountingClientRegistration.TokenClient);
            using var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials", ["client_id"] = options.ClientId,
                ["client_secret"] = options.ClientSecret, ["scope"] = options.Scope
            });
            try
            {
                using var response = await http.PostAsync(options.TokenEndpoint, content, ct);
                if (!response.IsSuccessStatusCode)
                    throw new IntegrationProviderException("AccountingTokenRequestFailed",
                        response.StatusCode == HttpStatusCode.TooManyRequests || (int)response.StatusCode >= 500);
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);
                if (result is null || string.IsNullOrWhiteSpace(result.AccessToken)
                    || result.AccessToken.Any(char.IsWhiteSpace) || result.AccessToken.Any(char.IsControl)
                    || !string.Equals(result.TokenType, "Bearer", StringComparison.OrdinalIgnoreCase)
                    || result.ExpiresIn is <= 0 or > 604800
                    || requestedAt.AddSeconds(result.ExpiresIn) <= DateTimeOffset.UtcNow)
                    throw new IntegrationProviderException("AccountingTokenResponseInvalid", false);
                token = result.AccessToken;
                refreshAt = requestedAt.AddSeconds(Math.Max(0, Math.Min(result.ExpiresIn - 30, 3600)));
                return token;
            }
            catch (HttpRequestException) { throw new IntegrationProviderException("AccountingTokenUnavailable", true); }
            catch (JsonException) { throw new IntegrationProviderException("AccountingTokenResponseInvalid", false); }
            catch (OperationCanceledException) when (!ct.IsCancellationRequested)
            { throw new IntegrationProviderException("AccountingTokenTimeout", true); }
        }
        finally { gate.Release(); }
    }

    public async Task InvalidateAsync(string rejectedToken)
    {
        await gate.WaitAsync();
        try { if (token == rejectedToken) { token = null; refreshAt = default; } }
        finally { gate.Release(); }
    }

    public void Dispose() => gate.Dispose();

    private sealed record TokenResponse(
        [property: JsonPropertyName("access_token")] string? AccessToken,
        [property: JsonPropertyName("token_type")] string? TokenType,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);
}

public sealed class AccountingServiceTokenHandler(AccountingServiceTokenProvider tokens,
    IOptions<HyperyekAccountingApiOptions> options) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        AccountingClientRegistration.Validate(options.Value);
        if (request.RequestUri is null || request.RequestUri.UserInfo.Length != 0 || request.RequestUri.Fragment.Length != 0
            || !new Uri(options.Value.BaseAddress).IsBaseOf(request.RequestUri))
            throw new InvalidOperationException("AccountingRequestDestinationInvalid");
        var token = await tokens.GetAsync(ct);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await base.SendAsync(request, ct);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            response.Dispose();
            await tokens.InvalidateAsync(token);
            // Retry is owned by the durable queue, never a blind replay of a POST.
            throw new IntegrationProviderException("AccountingAuthenticationFailed", true);
        }
        return response;
    }
}
