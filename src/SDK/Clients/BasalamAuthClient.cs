using System.Net.Http;
using System.Text.Json;
using Hyper.SDK.Auth;
using Hyper.SDK.Clients;
using Hyper.SDK.Config;
using Hyper.SDK.Errors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Hyper.SDK.Clients;

public interface IBasalamAuthClient
{
    Task<TokenInfo> GetTokenAsync(CancellationToken ct = default);
    Task<TokenInfo> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}

public sealed class BasalamAuthClient : IBasalamAuthClient
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly BasalamConfig _config;
    private readonly ILogger<BasalamAuthClient> _logger;
    private TokenInfo? _cachedToken;
    private readonly object _lock = new();

    public BasalamAuthClient(
        BasalamConfig config,
        ILogger<BasalamAuthClient>? logger = null,
        System.Net.Http.HttpClient? httpClient = null)
    {
        _config = config;
        _logger = logger ?? NullLogger<BasalamAuthClient>.Instance;
        _httpClient = httpClient ?? new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds) };
    }

    public async Task<TokenInfo> GetTokenAsync(CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (_cachedToken != null && !_cachedToken.IsExpired)
                return _cachedToken;
        }

        return await RefreshTokenImpl(ct);
    }

    public async Task<TokenInfo> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(refreshToken);
        return await RefreshTokenImpl(ct, refreshToken);
    }

    private async Task<TokenInfo> RefreshTokenImpl(CancellationToken ct, string? refreshTokenOverride = null)
    {
        var requestBody = new
        {
            grant_type = refreshTokenOverride != null ? "refresh_token" : "client_credentials",
            client_id = _config.ClientId,
            client_secret = _config.ClientSecret,
            refresh_token = refreshTokenOverride ?? _config.RefreshToken
        };

        _logger.LogInformation("Requesting token from {Endpoint}", _config.TokenEndpoint);

        var json = JsonSerializer.Serialize(requestBody);
        using var request = new HttpRequestMessage(HttpMethod.Post, _config.TokenEndpoint)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };

        using var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new BasalamAuthError($"Token request failed: {(int)response.StatusCode} {response.ReasonPhrase}", content);

        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        var accessToken = root.GetProperty("access_token").GetString()!;
        var tokenType = root.TryGetProperty("token_type", out var tt) ? tt.GetString() : "Bearer";
        var expiresIn = root.TryGetProperty("expires_in", out var ei) ? ei.GetInt32() : 3600;
        var refreshTokenResult = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;

        var token = new TokenInfo
        {
            AccessToken = accessToken,
            TokenType = tokenType ?? "Bearer",
            ExpiresIn = expiresIn,
            ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn),
            RefreshToken = refreshTokenResult,
            Scopes = Scope.All
        };

        lock (_lock)
        {
            _cachedToken = token;
        }

        _logger.LogInformation("Token acquired, expires in {ExpiresIn}s", expiresIn);
        return token;
    }
}
