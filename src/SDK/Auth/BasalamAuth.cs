using System.Net.Http;
using System.Text.Json;
using Hyper.SDK.Auth;
using Hyper.SDK.Clients;
using Hyper.SDK.Config;
using Hyper.SDK.Errors;
using Hyper.SDK.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Hyper.SDK.Auth;

public abstract class BasalamAuthBase
{
    protected readonly BasalamConfig Config;

    protected BasalamAuthBase(BasalamConfig config)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
    }

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
        var requestBody = new
        {
            grant_type = "client_credentials",
            client_id = Config.ClientId,
            client_secret = Config.ClientSecret
        };

        var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
        using var request = new HttpRequestMessage(HttpMethod.Post, Config.TokenEndpoint)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
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

    public string GetAuthorizationUrl(string redirectUri, string state, string codeVerifier, Scope scopes = Scope.CustomerProfileRead | Scope.VendorProfileRead)
    {
        var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
        query["response_type"] = "code";
        query["client_id"] = Config.ClientId!;
        query["redirect_uri"] = redirectUri;
        query["state"] = state;
        query["code_challenge"] = codeVerifier;
        query["code_challenge_method"] = "S256";
        query["scope"] = scopes.ToString();
        return $"{Config.AuthorizeEndpoint}?{query}";
    }

    public override async Task<TokenInfo> GetTokenAsync(CancellationToken ct = default)
    {
        throw new InvalidOperationException("AuthorizationCode requires code exchange. Use ExchangeCodeAsync.");
    }

    public async Task<TokenInfo> ExchangeCodeAsync(string code, string redirectUri, CancellationToken ct = default)
    {
        var requestBody = new
        {
            grant_type = "authorization_code",
            code,
            client_id = Config.ClientId,
            client_secret = Config.ClientSecret,
            redirect_uri = redirectUri
        };

        var httpClient = new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(Config.TimeoutSeconds) };
        var json = System.Text.Json.JsonSerializer.Serialize(requestBody);
        using var request = new HttpRequestMessage(HttpMethod.Post, Config.TokenEndpoint)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
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
