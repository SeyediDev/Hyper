using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hyper.Domain.Entities.Integrations;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class BasalamOAuthSettings
{
    public string ClientId { get; set; } = "";
    public string? ClientSecret { get; set; }
    public string AuthorizationEndpoint { get; set; } = "https://basalam.com/accounts/sso";
    public string TokenEndpoint { get; set; } = "https://auth.basalam.com/oauth/token";
    public string RedirectUri { get; set; } = "";
    public string Scopes { get; set; } = "customer.profile.read vendor.profile.read";
    // The official SDK documents confidential authorization-code flow, without PKCE.
    // Enable only when support is confirmed for the registered application.
    public bool UsePkce { get; set; }
}

public sealed class BasalamTokenResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; } = "";
    [JsonPropertyName("token_type")] public string TokenType { get; set; } = "Bearer";
    [JsonPropertyName("expires_in")] public long? ExpiresIn { get; set; }
    [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
    [JsonPropertyName("scope")] public string? Scope { get; set; }
}

public sealed record BasalamVendor(string Id, string Title);
public sealed record BasalamAuthorizationState(Guid RequestId, Guid SimulationId, string AdminId,
    string BrowserNonce, string? CodeVerifier, string RedirectUri);

public sealed class BasalamOAuthService(IOptions<BasalamOAuthSettings> options, HttpClient client,
    IDataProtectionProvider protection)
{
    private BasalamOAuthSettings Settings => options.Value;
    public string Scopes => Settings.Scopes;
    public string AuthorizationEndpoint => Settings.AuthorizationEndpoint;
    public string RedirectUri => Settings.RedirectUri;

    public string? ConfigurationError()
    {
        static bool Missing(string? value) => string.IsNullOrWhiteSpace(value)
            || value.Contains("your-", StringComparison.OrdinalIgnoreCase)
            || value.Contains("YOUR_", StringComparison.OrdinalIgnoreCase);
        if (Missing(Settings.ClientId) || Missing(Settings.ClientSecret))
            return "شناسه و رمز برنامه باسلام تنظیم نشده است (Basalam:ClientId / ClientSecret).";
        if (Settings.AuthorizationEndpoint != "https://basalam.com/accounts/sso"
            || Settings.TokenEndpoint != "https://auth.basalam.com/oauth/token")
            return "نشانی ورود یا دریافت توکن باسلام با قرارداد رسمی تطابق ندارد.";
        if (!Uri.TryCreate(Settings.RedirectUri, UriKind.Absolute, out var uri)
            || (uri.Scheme != "https" && !(uri.Scheme == "http" && uri.IsLoopback))
            || uri.AbsolutePath != "/api/auth/basalam/callback" || uri.Query.Length != 0
            || uri.Fragment.Length != 0 || uri.UserInfo.Length != 0)
            return "نشانی بازگشت باسلام باید آدرس کامل پنل با مسیر /api/auth/basalam/callback باشد.";
        if (string.IsNullOrWhiteSpace(Settings.Scopes) || Settings.Scopes.Length > 2000
            || Settings.Scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Any(s => s is "inventory.read" or "orders.read" or "products.read"))
            return "مجوزهای برنامه باید از scopeهای رسمی باسلام انتخاب شوند.";
        return null;
    }

    public string CreateAuthorizationUrl(Guid requestId, Guid simulationId, string adminId, string browserNonce)
    {
        if (ConfigurationError() is { } error) throw new InvalidOperationException(error);
        var verifier = Settings.UsePkce ? Nonce() : null;
        var state = new BasalamAuthorizationState(requestId, simulationId, adminId, browserNonce, verifier, Settings.RedirectUri);
        var ticket = protection.CreateProtector("Hyper.Basalam.Authorization.v2").ToTimeLimitedDataProtector()
            .Protect(JsonSerializer.Serialize(state), TimeSpan.FromMinutes(10));
        var parameters = new Dictionary<string, string>
        {
            ["client_id"] = Settings.ClientId, ["redirect_uri"] = Settings.RedirectUri,
            ["response_type"] = "code", ["scope"] = Settings.Scopes, ["state"] = ticket
        };
        if (verifier is not null)
        {
            parameters["code_challenge"] = Base64Url(SHA256.HashData(Encoding.ASCII.GetBytes(verifier)));
            parameters["code_challenge_method"] = "S256";
        }
        return Settings.AuthorizationEndpoint + "?" + string.Join("&",
            parameters.Select(p => Uri.EscapeDataString(p.Key) + "=" + Uri.EscapeDataString(p.Value)));
    }

    public BasalamAuthorizationState ReadState(string? state, string? nonce)
    {
        if (string.IsNullOrWhiteSpace(state) || state.Length > 8192 || string.IsNullOrWhiteSpace(nonce))
            throw new InvalidOperationException("درخواست بازگشت معتبر نیست؛ اتصال را دوباره شروع کنید.");
        try
        {
            var json = protection.CreateProtector("Hyper.Basalam.Authorization.v2").ToTimeLimitedDataProtector().Unprotect(state);
            var data = JsonSerializer.Deserialize<BasalamAuthorizationState>(json);
            if (data is null || !CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(data.BrowserNonce), Encoding.UTF8.GetBytes(nonce))
                || data.RedirectUri != Settings.RedirectUri)
                throw new CryptographicException();
            return data;
        }
        catch (Exception ex) when (ex is CryptographicException or JsonException or ArgumentException)
        {
            throw new InvalidOperationException("درخواست بازگشت منقضی یا نامعتبر است؛ اتصال را دوباره شروع کنید.");
        }
    }

    public async Task<BasalamTokenResponse> ExchangeCodeForTokenAsync(string code,
        BasalamAuthorizationState state, CancellationToken ct)
    {
        if (ConfigurationError() is { } error) throw new InvalidOperationException(error);
        var values = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code", ["code"] = code,
            ["client_id"] = Settings.ClientId, ["client_secret"] = Settings.ClientSecret!,
            ["redirect_uri"] = state.RedirectUri
        };
        if (state.CodeVerifier is not null) values["code_verifier"] = state.CodeVerifier;
        return await SendTokenAsync(values, ct);
    }

    public Task<BasalamTokenResponse> RefreshAccessTokenAsync(string refreshToken, CancellationToken ct = default) =>
        SendTokenAsync(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token", ["refresh_token"] = refreshToken,
            ["client_id"] = Settings.ClientId, ["client_secret"] = Settings.ClientSecret!
        }, ct);

    private async Task<BasalamTokenResponse> SendTokenAsync(Dictionary<string, string> values, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Settings.TokenEndpoint)
            { Content = new FormUrlEncodedContent(values) };
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException("باسلام درخواست دریافت توکن را نپذیرفت.");
        await response.Content.LoadIntoBufferAsync(1024 * 1024, ct);
        var token = JsonSerializer.Deserialize<BasalamTokenResponse>(await response.Content.ReadAsStringAsync(ct));
        if (token is null || string.IsNullOrWhiteSpace(token.AccessToken) || token.AccessToken.Any(char.IsWhiteSpace)
            || !string.Equals(token.TokenType, "Bearer", StringComparison.OrdinalIgnoreCase)
            || token.ExpiresIn is <= 0 or > 315360000 || token.Scope?.Length > 2000)
            throw new InvalidOperationException("پاسخ توکن باسلام معتبر نیست.");
        return token;
    }

    public async Task<BasalamVendor> GetVendorAsync(BasalamTokenResponse token, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://openapi.basalam.com/v1/users/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!response.IsSuccessStatusCode) throw new InvalidOperationException("خواندن غرفه حساب باسلام ممکن نشد؛ مجوز پروفایل را بررسی کنید.");
        await response.Content.LoadIntoBufferAsync(1024 * 1024, ct);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
        if (!json.RootElement.TryGetProperty("vendor", out var vendor) || vendor.ValueKind != JsonValueKind.Object
            || !vendor.TryGetProperty("id", out var id) || !id.TryGetInt64(out var number) || number <= 0)
            throw new InvalidOperationException("حساب باسلام انتخاب‌شده غرفه ندارد؛ با حساب صاحب غرفه وارد شوید.");
        var title = vendor.TryGetProperty("title", out var t) ? t.GetString() : null;
        return new BasalamVendor(number.ToString(CultureInfo.InvariantCulture),
            string.IsNullOrWhiteSpace(title) ? number.ToString(CultureInfo.InvariantCulture) : title[..Math.Min(title.Length, 200)]);
    }

    public ExternalOAuthToken CreateTokenEntity(long connectionId, int shopId, string tenantId,
        IntegrationProvider provider, BasalamTokenResponse tokenData) => new()
    {
        ConnectionId = connectionId, ShopId = shopId, TenantId = tenantId, Provider = provider,
        AccessToken = EncryptToken(tokenData.AccessToken),
        RefreshToken = string.IsNullOrWhiteSpace(tokenData.RefreshToken) ? null : EncryptToken(tokenData.RefreshToken),
        TokenType = tokenData.TokenType, Scopes = tokenData.Scope,
        ExpiresAtUtc = tokenData.ExpiresIn is { } seconds ? DateTime.UtcNow.AddSeconds(seconds) : null,
        IssuedAtUtc = DateTime.UtcNow, IsActive = true,
        // Never duplicate access/refresh tokens in an unencrypted raw response column.
        RawTokenResponse = null
    };

    public void UpdateTokenEntity(ExternalOAuthToken target, BasalamTokenResponse tokenData)
    {
        target.AccessToken = EncryptToken(tokenData.AccessToken);
        if (!string.IsNullOrWhiteSpace(tokenData.RefreshToken))
            target.RefreshToken = EncryptToken(tokenData.RefreshToken);
        target.TokenType = tokenData.TokenType;
        target.Scopes = tokenData.Scope ?? target.Scopes;
        target.ExpiresAtUtc = tokenData.ExpiresIn is { } seconds
            ? DateTime.UtcNow.AddSeconds(seconds) : null;
        target.IssuedAtUtc = DateTime.UtcNow;
        target.UpdatedAtUtc = DateTime.UtcNow;
        target.IsActive = true;
    }

    public string DecryptToken(string token) => protection.CreateProtector("Basalam.OAuth.Token").Unprotect(token);
    private string EncryptToken(string token) => protection.CreateProtector("Basalam.OAuth.Token").Protect(token);
    public static string Nonce() => Base64Url(RandomNumberGenerator.GetBytes(32));
    private static string Base64Url(byte[] value) => Convert.ToBase64String(value).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
