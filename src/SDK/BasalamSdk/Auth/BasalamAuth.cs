using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Basalam.SDK.Auth;
using Basalam.SDK.Clients;
using Basalam.SDK.Config;
using Basalam.SDK.Errors;
using Basalam.SDK.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Basalam.SDK.Auth;

public abstract class BasalamAuthBase(BasalamConfig config)
{
    protected readonly BasalamConfig Config = config ?? throw new ArgumentNullException(nameof(config));

    public abstract GrantType GrantType { get; }
    public abstract Task<TokenInfo> GetTokenAsync(CancellationToken ct = default);
}

public sealed class ClientCredentialsAuth : BasalamAuthBase
{
    public override GrantType GrantType => GrantType.ClientCredentials;

    public ClientCredentialsAuth(BasalamConfig config) : base(config)
    {
        if (string.IsNullOrEmpty(config.ClientId))
            throw new BasalamAuthError("ClientId is required for ClientCredentials auth");
        if (string.IsNullOrEmpty(config.ClientSecret))
            throw new BasalamAuthError("ClientSecret is required for ClientCredentials auth");
    }

    public override async Task<TokenInfo> GetTokenAsync(CancellationToken ct = default)
    {
        var httpClient = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(Config.TimeoutSeconds) };
        var requestBody = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = Config.ClientId!,
            ["client_secret"] = Config.ClientSecret!
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, Config.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        using var response = await httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new BasalamAuthError($"Token request failed: {(int)response.StatusCode}", content);

        return ParseToken(content);
    }

    private static TokenInfo ParseToken(string json)
    {
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;
        var accessToken = root.GetProperty("access_token").GetString()!;
        var expiresIn = root.TryGetProperty("expires_in", out var ei) ? ei.GetInt32() : 3600;
        var refreshToken = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;

        return new TokenInfo
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = expiresIn,
            ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn),
            RefreshToken = refreshToken,
            Scopes = Scope.All
        };
    }
}

public sealed class AuthorizationCodeAuth : BasalamAuthBase
{
    public override GrantType GrantType => GrantType.AuthorizationCode;

    public AuthorizationCodeAuth(BasalamConfig config) : base(config)
    {
        if (string.IsNullOrEmpty(config.ClientId))
            throw new BasalamAuthError("ClientId is required for AuthorizationCode auth");
        if (string.IsNullOrEmpty(config.ClientSecret))
            throw new BasalamAuthError("ClientSecret is required for AuthorizationCode auth");
    }

    public string GetAuthorizationUrl(string redirectUri, string state, string codeVerifier, Scope scopes = Scope.VendorProfileRead)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(redirectUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(codeVerifier);
        var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
        query["client_id"] = Config.ClientId!;
        query["redirect_uri"] = redirectUri;
        query["state"] = state;
        query["code_challenge"] = Convert.ToBase64String(SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier)))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
        query["code_challenge_method"] = "S256";
        query["scope"] = FormatScopes(scopes);
        return $"{Config.AuthorizeEndpoint}?{query}";
    }

    private static string FormatScopes(Scope scopes)
    {
        if (scopes == Scope.All)
            throw new ArgumentException("Scope.All is not a valid authorization request; select explicit scopes.", nameof(scopes));

        var values = new List<string>();
        Add(Scope.CustomerProfileRead, "customer.profile.read");
        Add(Scope.CustomerProfileWrite, "customer.profile.write");
        Add(Scope.VendorProfileRead, "vendor.profile.read");
        Add(Scope.VendorProfileWrite, "vendor.profile.write");
        Add(Scope.ProductRead, "vendor.product.read");
        Add(Scope.ProductWrite, "vendor.product.write");
        Add(Scope.ParcelRead, "vendor.parcel.read");
        Add(Scope.ParcelWrite, "vendor.parcel.write");
        Add(Scope.OrderRead, "customer.order.read");
        Add(Scope.OrderWrite, "customer.order.write");
        Add(Scope.ChatRead, "customer.chat.read");
        Add(Scope.ChatWrite, "customer.chat.write");
        return string.Join(' ', values);

        void Add(Scope value, string name)
        {
            if ((scopes & value) == value) values.Add(name);
        }
    }

    public override async Task<TokenInfo> GetTokenAsync(CancellationToken ct = default)
    {
        throw new InvalidOperationException("AuthorizationCode requires code exchange. Use ExchangeCodeAsync.");
    }

    public async Task<TokenInfo> ExchangeCodeAsync(string code, string redirectUri, CancellationToken ct = default)
    {
        var requestBody = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["client_id"] = Config.ClientId!,
            ["client_secret"] = Config.ClientSecret!,
            ["redirect_uri"] = redirectUri
        };

        var httpClient = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(Config.TimeoutSeconds) };
        using var request = new HttpRequestMessage(HttpMethod.Post, Config.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        using var response = await httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new BasalamAuthError($"Token exchange failed: {(int)response.StatusCode}", content);

        return ParseToken(content);
    }

    private static TokenInfo ParseToken(string json)
    {
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        var root = doc.RootElement;
        var accessToken = root.GetProperty("access_token").GetString()!;
        var expiresIn = root.TryGetProperty("expires_in", out var ei) ? ei.GetInt32() : 3600;
        var refreshToken = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;

        return new TokenInfo
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = expiresIn,
            ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn),
            RefreshToken = refreshToken,
            Scopes = Scope.All
        };
    }
}

public sealed class PersonalTokenAuth : BasalamAuthBase
{
    public override GrantType GrantType => GrantType.PersonalToken;

    public PersonalTokenAuth(BasalamConfig config) : base(config)
    {
        if (string.IsNullOrEmpty(config.AccessToken))
            throw new BasalamAuthError("AccessToken is required for PersonalToken auth");
    }

    public override Task<TokenInfo> GetTokenAsync(CancellationToken ct = default)
    {
        var token = new TokenInfo
        {
            AccessToken = Config.AccessToken!,
            TokenType = "Bearer",
            ExpiresIn = int.MaxValue,
            ExpiresAt = DateTime.MaxValue,
            Scopes = Scope.All
        };
        return Task.FromResult(token);
    }
}
