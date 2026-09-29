using System.Globalization;
using System.Net;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

internal static class RefreshRetryChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        using var handler = new FailureTransport();
        using var http = new HttpClient(handler);
        var service = new BasalamOAuthService(Options.Create(new BasalamOAuthSettings
        { ClientId = "fixture-client", ClientSecret = "fixture-secret", RedirectUri = "http://localhost/api/auth/basalam/callback" }),
            http, new EphemeralDataProtectionProvider(), FixtureProtection.Provider());
        foreach (var code in new[] { 408, 429, 500, 502, 503, 504 })
        {
            handler.Status = code; handler.Header = "540";
            var error = await Failure();
            check(error.Code == "OAuthRefreshHttp" + code && error.Retryable && error.RetryAfter == TimeSpan.FromMinutes(9)
                && error.Message == error.Code && error.InnerException is null,
                $"refresh HTTP {code} forwards safe retryable code and delay without reading response body");
        }
        handler.Header = DateTimeOffset.UtcNow.AddMinutes(9).ToString("R", CultureInfo.InvariantCulture);
        var dated = await Failure();
        check(dated.RetryAfter > TimeSpan.FromMinutes(8) && dated.RetryAfter <= TimeSpan.FromMinutes(9),
            "refresh HTTP-date delay retained");
        foreach (var header in new[] { "0", DateTimeOffset.UtcNow.AddMinutes(-1).ToString("R", CultureInfo.InvariantCulture) })
        {
            handler.Header = header;
            check((await Failure()).RetryAfter == TimeSpan.Zero, "zero/past refresh delay is nonnegative");
        }
        foreach (var header in new string?[] { null, "invalid", "-5" })
        {
            handler.Header = header;
            check((await Failure()).RetryAfter is null, "absent/malformed refresh delay leaves queue fallback policy intact");
        }
        foreach (var code in new[] { 400, 401, 403, 422, 501 })
        {
            handler.Status = code; handler.Header = "540";
            var before = handler.Calls;
            try { await service.RefreshAccessTokenAsync("fixture-refresh"); throw new Exception("Expected rejection"); }
            catch (InvalidOperationException) { check(handler.Calls == before + 1, $"refresh HTTP {code} remains terminal with one attempt"); }
        }
        handler.Status = 503;
        var calls = handler.Calls;
        try
        {
            await service.ExchangeCodeForTokenAsync("fixture-code",
                new(Guid.NewGuid(), Guid.NewGuid(), "fixture-admin", "fixture-nonce", null, "http://localhost/api/auth/basalam/callback"), default);
            throw new Exception("Expected callback rejection");
        }
        catch (InvalidOperationException) { check(handler.Calls == calls + 1, "authorization code exchange retains callback error contract without auto-replay"); }

        async Task<IntegrationProviderException> Failure()
        {
            var before = handler.Calls;
            try { await service.RefreshAccessTokenAsync("fixture-refresh"); }
            catch (IntegrationProviderException ex) when (handler.Calls == before + 1) { return ex; }
            throw new Exception("Expected one classified refresh attempt");
        }
    }

    private sealed class FailureTransport : HttpMessageHandler
    {
        public int Calls; public int Status; public string? Header;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            Calls++;
            var response = new HttpResponseMessage((HttpStatusCode)Status) { Content = new UnreadableContent() };
            if (Header is not null) response.Headers.TryAddWithoutValidation("Retry-After", Header);
            return Task.FromResult(response);
        }
    }
    private sealed class UnreadableContent : HttpContent
    {
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context) =>
            throw new InvalidOperationException("Failure response body must not be read");
        protected override bool TryComputeLength(out long length) { length = 0; return false; }
    }
}
