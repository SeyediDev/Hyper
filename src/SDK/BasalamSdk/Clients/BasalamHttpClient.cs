using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Basalam.SDK.Auth;
using Basalam.SDK.Clients;
using Basalam.SDK.Config;
using Basalam.SDK.Errors;
using Basalam.SDK.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Basalam.SDK.Clients;

public interface IBasalamHttpClient
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct = default);
    Task<T?> GetAsync<T>(string url, CancellationToken ct = default);
    Task<T?> PostAsync<T>(string url, object? body, CancellationToken ct = default);
    Task<T?> PutAsync<T>(string url, object? body, CancellationToken ct = default);
    Task<T?> PatchAsync<T>(string url, object? body, CancellationToken ct = default);
    Task<T?> DeleteAsync<T>(string url, CancellationToken ct = default);
    Task<T?> DeleteAsync<T>(string url, object? body, CancellationToken ct = default);
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
            request.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);

        var maxRetries = Math.Clamp(_config.MaxRetries, 0, 10);
        var delayBase = Math.Clamp(_config.RetryDelayMilliseconds, 0, 60_000);
        for (var attempt = 0; ; attempt++)
        {
            // HttpRequestMessage instances cannot be sent twice. Clone before every
            // attempt so POST/PATCH bodies are retried exactly as issued.
            using var outgoing = await CloneAsync(request, ct);
            _logger.LogInformation("Sending {Method} {Url} (attempt {Attempt})", outgoing.Method,
                outgoing.RequestUri, attempt + 1);
            var response = await _httpClient.SendAsync(outgoing, ct);
            _logger.LogInformation("Received {StatusCode} from {Url}", (int)response.StatusCode, outgoing.RequestUri);
            if (!IsTransient(response.StatusCode) || attempt >= maxRetries) return response;

            // A provider cooldown belongs to the caller's durable scheduler. Do
            // not shorten it or hold a worker lease until its timeout hides it.
            if (ReadRetryAfter(response) is { } cooldown && cooldown > TimeSpan.Zero) return response;

            var delay = RetryDelay(response, attempt, delayBase);
            response.Dispose();
            if (delay > TimeSpan.Zero) await Task.Delay(delay, ct);
        }
    }

    private static bool IsTransient(HttpStatusCode status) => status is HttpStatusCode.RequestTimeout
        or (HttpStatusCode)429 || status is HttpStatusCode.InternalServerError
        or HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable
        or HttpStatusCode.GatewayTimeout;

    private static TimeSpan RetryDelay(HttpResponseMessage response, int attempt, int baseMilliseconds)
    {
        if (ReadRetryAfter(response) is { } delay) return delay;
        var milliseconds = (long)baseMilliseconds * (1L << Math.Min(attempt, 6));
        return TimeSpan.FromMilliseconds(Math.Min(milliseconds, 60_000));
    }

    private static TimeSpan? ReadRetryAfter(HttpResponseMessage response)
    {
        if (response.Headers.RetryAfter?.Delta is { } delta && delta >= TimeSpan.Zero)
            return delta;
        if (response.Headers.RetryAfter?.Date is { } date)
        {
            var remaining = date - DateTimeOffset.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
        return null;
    }

    private static async Task<HttpRequestMessage> CloneAsync(HttpRequestMessage source, CancellationToken ct)
    {
        var clone = new HttpRequestMessage(source.Method, source.RequestUri);
        foreach (var header in source.Headers) clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        if (source.Content is not null)
        {
            var bytes = await source.Content.ReadAsByteArrayAsync(ct);
            clone.Content = new ByteArrayContent(bytes);
            foreach (var header in source.Content.Headers) clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return clone;
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

    public async Task<T?> DeleteAsync<T>(string url, object? body, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, url);
        if (body is not null)
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
                body) { RetryAfter = ReadRetryAfter(response) };
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
