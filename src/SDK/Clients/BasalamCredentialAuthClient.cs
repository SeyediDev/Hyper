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

public sealed class BasalamCredentialAuthClient : IDisposable
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly BasalamConfig _config;
    private readonly ILogger<BasalamCredentialAuthClient>? _logger;
    private TokenInfo? _token;
    private readonly object _lock = new();
    private Timer? _refreshTimer;

    public BasalamCredentialAuthClient(
        BasalamConfig config,
        ILogger<BasalamCredentialAuthClient>? logger = null,
        System.Net.Http.HttpClient? httpClient = null)
    {
        _config = config;
        _logger = logger;
        _httpClient = httpClient ?? new System.Net.Http.HttpClient { Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds) };
    }

    public TokenInfo? CurrentToken
    {
        get
        {
            lock (_lock) return _token;
        }
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await RefreshTokenImpl(ct);
        var dueTime = TimeSpan.FromSeconds(Math.Max(_token!.ExpiresIn - 300, 60) * 1000);
        _refreshTimer = new Timer(async _ => await RefreshTokenImpl(CancellationToken.None), null, dueTime, TimeSpan.FromMilliseconds(-1));
    }

    public void Dispose()
    {
        _refreshTimer?.Dispose();
    }

    private async Task RefreshTokenImpl(CancellationToken ct)
    {
        var requestBody = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _config.ClientId!,
            ["client_secret"] = _config.ClientSecret!
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, _config.TokenEndpoint)
        {
            Content = new FormUrlEncodedContent(requestBody)
        };

        using var response = await _httpClient.SendAsync(request, ct);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new BasalamAuthError($"Token refresh failed: {(int)response.StatusCode}", content);

        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;
        var accessToken = root.GetProperty("access_token").GetString()!;
        var expiresIn = root.TryGetProperty("expires_in", out var ei) ? ei.GetInt32() : 3600;
        var refreshToken = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;

        var token = new TokenInfo
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresIn = expiresIn,
            ExpiresAt = DateTime.UtcNow.AddSeconds(expiresIn),
            RefreshToken = refreshToken,
            Scopes = Scope.All
        };

        lock (_lock)
        {
            _token = token;
        }

        _logger?.LogInformation("Token refreshed, expires in {Seconds}", expiresIn);
    }
}
