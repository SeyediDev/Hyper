using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hyper.SDK.Auth;
using Hyper.SDK.Clients;
using Hyper.SDK.Config;
using Hyper.SDK.Errors;
using Hyper.SDK.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Hyper.SDK.Clients;

public interface IBasalamHttpClient
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct = default);
    Task<T?> GetAsync<T>(string url, CancellationToken ct = default);
    Task<T?> PostAsync<T>(string url, object? body, CancellationToken ct = default);
    Task<T?> PutAsync<T>(string url, object? body, CancellationToken ct = default);
    Task<T?> PatchAsync<T>(string url, object? body, CancellationToken ct = default);
    Task<T?> DeleteAsync<T>(string url, CancellationToken ct = default);
}

public sealed class BasalamHttpClient : IBasalamHttpClient, IDisposable
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly BasalamConfig _config;
    private readonly ILogger<BasalamHttpClient> _logger;
    private TokenInfo? _token;
    private readonly object _tokenLock = new();

    public BasalamHttpClient(
        BasalamConfig config,
        ILogger<BasalamHttpClient>? logger = null,
        System.Net.Http.HttpClient? httpClient = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? NullLogger<BasalamHttpClient>.Instance;
        _httpClient = httpClient ?? new System.Net.Http.HttpClient
        {
            Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds)
        };
    }

    public void SetToken(TokenInfo? token)
    {
        lock (_tokenLock)
        {
            _token = token;
        }
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.RequestUri != null && !request.RequestUri.IsAbsoluteUri)
        {
            request.RequestUri = new Uri(_config.ResolveRequestUrl(request.RequestUri.ToString()));
        }

        request.Headers.UserAgent.ParseAdd(_config.GetUserAgent());

        var token = GetCurrentToken();
        if (token != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
        }

        _logger.LogInformation("Sending {Method} {Url}", request.Method, request.RequestUri);

        var response = await _httpClient.SendAsync(request, ct);

        _logger.LogInformation("Received {StatusCode} from {Url}", (int)response.StatusCode, request.RequestUri);

        return response;
    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        using var response = await SendAsync(request, ct);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> PostAsync<T>(string url, object? body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        if (body != null)
        {
            request.Content = JsonContent.Create(body, options: new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
        using var response = await SendAsync(request, ct);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> PutAsync<T>(string url, object? body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, url);
        if (body != null)
        {
            request.Content = JsonContent.Create(body, options: new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
        using var response = await SendAsync(request, ct);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> PatchAsync<T>(string url, object? body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);
        if (body != null)
        {
            request.Content = JsonContent.Create(body, options: new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
        using var response = await SendAsync(request, ct);
        return await DeserializeResponseAsync<T>(response);
    }

    public async Task<T?> DeleteAsync<T>(string url, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        using var response = await SendAsync(request, ct);
        return await DeserializeResponseAsync<T>(response);
    }

    private TokenInfo? GetCurrentToken()
    {
        lock (_tokenLock)
        {
            return _token;
        }
    }

    private static async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new BasalamAPIError(
                $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}",
                (int)response.StatusCode,
                body);
        }

        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
            return default;

        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            AllowTrailingCommas = true
        });
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
