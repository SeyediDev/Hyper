using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Domain.Features.Integrations;

internal static class AccountingHttpChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        var transport = new Responses();
        using var http = new HttpClient(transport) { BaseAddress = new Uri("https://accounting.fixture.invalid/") };
        var api = new HyperyekAccountingApiClient(http);
        var command = new IntegrationExternalProductChangedCommand("event-1", 7, "tenant-a", 1, 44, "12", null, null, "title", 1, null, 1);
        async Task Expect(Func<Task> operation, string code, bool retryable)
        {
            var before = transport.Calls;
            try { await operation(); }
            catch (IntegrationProviderException ex) when (ex.Code == code && ex.Retryable == retryable)
            {
                check(transport.Calls == before + 1 && !ex.ToString().Contains("sensitive-body"), code + " has one HTTP attempt and safe classified error");
                return;
            }
            throw new InvalidOperationException("Expected " + code);
        }
        foreach (var status in new[] { 408, 429, 500, 502, 503, 504 })
        {
            transport.Status = (HttpStatusCode)status;
            await Expect(() => api.ApplyExternalProductChangedAsync(command, default), "AccountingApi_" + status, true);
            await Expect(() => api.GetProductsAsync(7, "tenant-a", default), "AccountingApi_" + status, true);
        }
        transport.Status = HttpStatusCode.TooManyRequests;
        transport.Delay = new RetryConditionHeaderValue(TimeSpan.FromMinutes(12));
        try { await api.ApplyExternalProductChangedAsync(command, default); }
        catch (IntegrationProviderException ex) { check(ex.RetryAfter == TimeSpan.FromMinutes(12), "accounting delta Retry-After is preserved"); }
        transport.Delay = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMinutes(15));
        try { await api.GetByIdsAsync([7], default); }
        catch (IntegrationProviderException ex) { check(ex.RetryAfter?.TotalMinutes is > 14 and <= 15, "accounting HTTP-date Retry-After reaches bulk read caller"); }
        transport.Delay = new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMinutes(-1));
        try { await api.GetAsync(7, default); }
        catch (IntegrationProviderException ex) { check(ex.RetryAfter == TimeSpan.Zero, "past Retry-After cannot create negative queue delay"); }
        transport.Delay = null;
        foreach (var status in new[] { 400, 401, 403, 404, 501 })
        {
            transport.Status = (HttpStatusCode)status;
            var rejected = await api.ApplyExternalProductChangedAsync(command, default);
            check(rejected.Status == BusinessCommandStatus.Rejected && rejected.ErrorCode == "AccountingApi_" + status,
                "permanent accounting command HTTP " + status + " is not auto-retried");
            await Expect(() => api.GetProductsAsync(7, "tenant-a", default), "AccountingApi_" + status, false);
        }
        transport.Status = HttpStatusCode.NotFound;
        check(await api.GetAsync(7, default) is null, "single shop read retains expected not-found semantics");
        foreach (var (status, body, expected) in new[]
        {
            (200, "{\"status\":1,\"internalReference\":\"44\"}", BusinessCommandStatus.Applied),
            (409, "{\"status\":2,\"internalReference\":\"44\",\"stockCommitted\":true}", BusinessCommandStatus.Duplicate),
            (202, "{\"status\":3,\"errorCode\":\"NeedsPolicy\"}", BusinessCommandStatus.PendingDependency),
            (422, "{\"status\":4,\"errorCode\":\"InvalidPrice\"}", BusinessCommandStatus.Rejected)
        })
        {
            transport.Status = (HttpStatusCode)status; transport.Body = body;
            var result = await api.ApplyExternalProductChangedAsync(command, default);
            check(result.Status == expected && (status != 409 || result.StockCommitted && result.InternalReference == "44"),
                "accounting HTTP " + status + " preserves documented result and duplicate stock acknowledgment");
        }
        foreach (var (status, body) in new[]
        {
            (200,"{}"),(200,"null"),(200,"not-json"),(200,"{\"status\":99}"),
            (409,"{\"status\":1}"),(202,"{\"status\":1}"),(422,"{\"status\":2}"),
            (202,"{\"status\":3,\"stockCommitted\":true}"),(204,"{\"status\":1}")
        })
        {
            transport.Status = (HttpStatusCode)status; transport.Body = body;
            await Expect(() => api.ApplyExternalProductChangedAsync(command, default), "AccountingApiInvalidResponse", false);
        }
        transport.ThrowTransport = true;
        try { await api.ApplyExternalProductChangedAsync(command, default); throw new Exception("Expected network failure"); }
        catch (HttpRequestException) { check(true, "network errors propagate to the durable worker without hidden HTTP retries"); }
        transport.ThrowTransport = false;
        using var cancelled = new CancellationTokenSource(); cancelled.Cancel();
        try { await api.ApplyExternalProductChangedAsync(command, cancelled.Token); throw new Exception("Expected cancellation"); }
        catch (OperationCanceledException) { check(true, "caller cancellation is not converted into a business rejection"); }
    }

    private sealed class Responses : HttpMessageHandler
    {
        public HttpStatusCode Status = HttpStatusCode.OK;
        public string Body = "sensitive-body";
        public RetryConditionHeaderValue? Delay;
        public int Calls;
        public bool ThrowTransport;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested(); Calls++;
            if (ThrowTransport) throw new HttpRequestException("controlled network failure");
            var response = new HttpResponseMessage(Status) { Content = new StringContent(Body, Encoding.UTF8, "application/json") };
            response.Headers.RetryAfter = Delay;
            return Task.FromResult(response);
        }
    }
}
