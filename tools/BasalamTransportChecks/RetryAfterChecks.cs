using System.Globalization;
using System.Net;
using Basalam.SDK;
using Basalam.SDK.Config;
using Basalam.SDK.Errors;

internal static class RetryAfterChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        foreach (var status in new[] { 408, 429, 500, 502, 503, 504 })
        {
            var (error, requests) = await Failure(status, "540");
            check(requests == 1 && error.StatusCode == status && error.RetryAfter == TimeSpan.FromMinutes(9),
                $"HTTP {status} positive cooldown escapes SDK immediately without shortening");
        }
        var deadline = DateTimeOffset.UtcNow.AddMinutes(9);
        var (dated, dateRequests) = await Failure(429, deadline.ToString("R", CultureInfo.InvariantCulture));
        check(dateRequests == 1 && dated.RetryAfter > TimeSpan.FromMinutes(8)
            && dated.RetryAfter <= TimeSpan.FromMinutes(9), "HTTP-date cooldown reaches SDK caller");
        var (shortDelay, shortRequests) = await Failure(429, "1");
        check(shortRequests == 1 && shortDelay.RetryAfter == TimeSpan.FromSeconds(1),
            "even short positive cooldown is delegated rather than held in worker");
        foreach (var header in new[] { "0", DateTimeOffset.UtcNow.AddHours(-1).ToString("R", CultureInfo.InvariantCulture) })
        {
            var (error, requests) = await Failure(503, header);
            check(requests == 3 && error.RetryAfter == TimeSpan.Zero,
                "zero/past Retry-After permits bounded immediate retries and never negative delay");
        }
        foreach (var header in new string?[] { null, "not-a-delay", "-12" })
        {
            var (error, requests) = await Failure(503, header);
            check(requests == 3 && error.RetryAfter is null,
                "absent/malformed Retry-After retains finite SDK fallback without invented cooldown");
        }
        foreach (var status in new[] { 400, 401, 403, 422 })
        {
            var (error, requests) = await Failure(status, "540");
            check(requests == 1 && error.StatusCode == status && error.RetryAfter == TimeSpan.FromMinutes(9),
                $"permanent HTTP {status} is not retried merely because it carries Retry-After");
        }
        var (last, lastRequests) = await Failure(503, "540", skipFirstHeader: true);
        check(lastRequests == 2 && last.RetryAfter == TimeSpan.FromMinutes(9),
            "cooldown on a later attempt is returned from that final response");
        var (disabled, disabledRequests) = await Failure(429, "540", retries: 0);
        check(disabledRequests == 1 && disabled.RetryAfter == TimeSpan.FromMinutes(9),
            "disabled SDK retries still retain Retry-After");
    }

    private static async Task<(BasalamAPIError Error, int Requests)> Failure(int status, string? header,
        bool skipFirstHeader = false, int retries = 2)
    {
        using var transport = new FailureTransport(status, header, skipFirstHeader);
        using var http = new HttpClient(transport);
        using var sdk = new BasalamClient(new BasalamConfig { MaxRetries = retries, RetryDelayMilliseconds = 0 }, httpClient: http);
        // Regression guard: never wait a real nine-minute provider cooldown.
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        try { await sdk.Products.PatchStockAsync(12, 0, timeout.Token); }
        catch (BasalamAPIError error) { return (error, transport.Requests); }
        throw new InvalidOperationException("Expected final provider HTTP failure");
    }

    private sealed class FailureTransport(int status, string? header, bool skipFirstHeader) : HttpMessageHandler
    {
        public int Requests;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Requests++;
            var response = new HttpResponseMessage((HttpStatusCode)status) { Content = new StringContent("sensitive-provider-fixture") };
            if (header is not null && (!skipFirstHeader || Requests > 1))
                response.Headers.TryAddWithoutValidation("Retry-After", header);
            return Task.FromResult(response);
        }
    }
}
