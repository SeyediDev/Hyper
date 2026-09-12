using System.Net;
using System.Text;
using System.Text.Json;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Hyper.Infrastructure.Features.Integrations;

internal static class BasalamChecks
{
    public static async Task<int> Run()
    {
        var passed = 0;
        async Task Check(string name, Func<Task> test) { await test(); passed++; Console.WriteLine("PASS Basalam " + name); }
        static ExternalIntegrationConnection Ready() => new()
        {
            ShopId = 1, TenantId = "fixture", Provider = IntegrationProvider.Basalam, AccountIdentifier = "42",
            CredentialType = IntegrationCredentialType.BearerToken, CredentialsJson = "{\"access_token\":\"fixture-secret\"}"
        };
        static HttpResponseMessage Json(string json) => new(HttpStatusCode.OK) { Content = new StringContent(json, Encoding.UTF8, "application/json") };
        static async Task Fails(Func<Task> action, string code)
        {
            try { await action(); }
            catch (IntegrationProviderException error) when (error.Code == code) { return; }
            throw new Exception("Expected " + code);
        }
        await Check("paginated catalog and variants", async () =>
        {
            var calls = 0;
            var adapter = new BasalamIntegrationAdapter(new FakeHttp((request, _) =>
            {
                calls++;
                if (request.Headers.Authorization?.ToString() != "Bearer fixture-secret"
                    || request.RequestUri?.Host != "openapi.basalam.com"
                    || !request.RequestUri.Query.Contains($"page={calls}&")
                    || !request.RequestUri.Query.Contains("variants_flatting=false")) throw new Exception("Invalid catalog request.");
                return Task.FromResult(Json(calls == 1
                    ? """{"data":[{"id":10,"title":"a","price":300,"inventory":7,"sku":"A"}],"total_page":2,"page":1} """
                    : """{"data":[{"id":11,"title":"b","price":200,"inventory":99,"variant":[{"id":111,"stock":4,"price":500,"sku":"B1"},{"id":112,"stock":2,"price":600}]}],"total_page":2,"page":2} """));
            }));
            var result = (await adapter.ReadCatalogAsync(Ready(), default)).ToArray();
            if (calls != 2 || result.Length != 3 || result[1].VariantId != "111" || result[1].Inventory != 4 || result[0].Sku != "A")
                throw new Exception("Incorrect catalog mapping.");
        });
        await Check("no pagination metadata reads through empty page", async () =>
        {
            var calls = 0;
            var adapter = new BasalamIntegrationAdapter(new FakeHttp((_, _) => Task.FromResult(Json(++calls == 1
                ? """{"data":[{"id":10,"title":"a","inventory":1}]}""" : """{"data":[],"result_count":0}"""))));
            if ((await adapter.ReadCatalogAsync(Ready(), default)).Count != 1 || calls != 2) throw new Exception("Truncated catalog.");
        });
        await Check("duplicate pages rejected", () => Fails(() => new BasalamIntegrationAdapter(new FakeHttp((_, _) =>
            Task.FromResult(Json("""{"data":[{"id":10,"title":"a","inventory":1}]}""")))).ReadCatalogAsync(Ready(), default), "DuplicateCatalogIdentifier"));
        await Check("malformed catalog never empty success", () => Fails(() => new BasalamIntegrationAdapter(new FakeHttp((_, _) =>
            Task.FromResult(Json("{}")))).ReadCatalogAsync(Ready(), default), "InvalidCatalog"));
        foreach (var variant in new string?[] { null, "101" })
        {
            await Check("absolute stock route " + (variant ?? "product"), async () =>
            {
                var calls = 0;
                var adapter = new BasalamIntegrationAdapter(new FakeHttp(async (request, ct) =>
                {
                    calls++;
                    if (request.Method == HttpMethod.Get) return Json(variant is null
                        ? """{"id":10,"vendor":{"id":42},"variants":[]}"""
                        : """{"id":10,"vendor":{"id":42},"variants":[{"id":101}]}""");
                    if (request.Method != HttpMethod.Patch || request.RequestUri?.AbsolutePath != "/v1/products/10" + (variant is null ? "" : "/variations/101"))
                        throw new Exception("Wrong inventory route.");
                    using var payload = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(ct));
                    if (payload.RootElement.EnumerateObject().Count() != 1 || payload.RootElement.GetProperty("stock").GetInt32() != 0)
                        throw new Exception("Stock-only zero update changed another property.");
                    return Json("{}");
                }));
                await adapter.PublishInventoryAsync(Ready(), [new("10", variant, 0)], default);
                if (calls != 2) throw new Exception("Ownership not checked.");
            });
        }
        foreach (var quantity in new[] { -1m, 1.5m, (decimal)int.MaxValue + 1 })
            await Check("invalid quantity rejected before HTTP " + quantity,
                () => Fails(() => new BasalamIntegrationAdapter(new FakeHttp((_, _) => throw new Exception("Unexpected HTTP")))
                    .PublishInventoryAsync(Ready(), [new("10", null, quantity)], default), "InvalidStockQuantity"));
        await Check("vendor mismatch cannot write", () => Fails(() => new BasalamIntegrationAdapter(new FakeHttp((request, _) =>
        {
            if (request.Method != HttpMethod.Get) throw new Exception("Cross-vendor write.");
            return Task.FromResult(Json("""{"id":10,"vendor":{"id":43}}"""));
        })).PublishInventoryAsync(Ready(), [new("10", null, 1)], default), "VendorMismatch"));
        await Check("variant required for variable product", () => Fails(() => new BasalamIntegrationAdapter(new FakeHttp((_, _) =>
            Task.FromResult(Json("""{"id":10,"vendor":{"id":42},"variants":[{"id":101}]}"""))))
                .PublishInventoryAsync(Ready(), [new("10", null, 1)], default), "VariantMismatch"));
        await Check("path injection rejected before HTTP", () => Fails(() => new BasalamIntegrationAdapter(new FakeHttp((_, _) => throw new Exception("Unexpected HTTP")))
            .PublishInventoryAsync(Ready(), [new("../vendors/43", null, 1)], default), "InvalidExternalIdentifier"));
        foreach (var status in new[] { 202, 401, 422, 429, 503 })
        {
            await Check("sanitized HTTP " + status, async () =>
            {
                var adapter = new BasalamIntegrationAdapter(new FakeHttp((_, _) =>
                {
                    var response = new HttpResponseMessage((HttpStatusCode)status) { Content = new StringContent("secret-should-never-escape") };
                    response.Headers.RetryAfter = new(TimeSpan.FromSeconds(90));
                    return Task.FromResult(response);
                }));
                try { await adapter.ReadCatalogAsync(Ready(), default); }
                catch (IntegrationProviderException error)
                {
                    if (error.Message != "Http" + status || error.Retryable != (status is 429 or 503)
                        || error.RetryAfter != TimeSpan.FromSeconds(90)) throw new Exception("Incorrect retry classification or leaked body.");
                    return;
                }
                throw new Exception("HTTP failure reported success.");
            });
        }
        await Check("retry policy honors server delay", () =>
        {
            if (IntegrationRetryPolicy.Delay(1) != TimeSpan.FromMinutes(1)
                || IntegrationRetryPolicy.Delay(5) != TimeSpan.FromHours(24)
                || IntegrationRetryPolicy.Delay(1, TimeSpan.FromHours(2)) != TimeSpan.FromHours(2)) throw new Exception("Incorrect retry schedule.");
            return Task.CompletedTask;
        });
        await Check("whole-shop availability and reservation boundary", () =>
        {
            if (IntegrationAvailableInventory.Calculate(10, 3, true) != 7
                || IntegrationAvailableInventory.Calculate(2, 3, true) != 0
                || IntegrationAvailableInventory.Calculate(-1, 0, true) != 0
                || IntegrationAvailableInventory.Calculate(10, 0, false) != 0
                || IntegrationAvailableInventory.Calculate(1.5m, 0.25m, true) != 1.25m)
                throw new Exception("Incorrect available stock.");
            try { IntegrationAvailableInventory.Calculate(10, -1, true); }
            catch (InvalidOperationException) { return Task.CompletedTask; }
            throw new Exception("Negative reservation balance accepted.");
        });
        return passed;
    }

    private sealed class FakeHttp(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler, IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(this, disposeHandler: false);
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) => send(request, ct);
    }
}
