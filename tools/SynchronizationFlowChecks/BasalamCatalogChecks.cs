using System.Net;
using System.Text;
using Basalam.SDK;
using Basalam.SDK.Config;
using Hyper.Infrastructure.Features.Integrations;

internal static class BasalamCatalogChecks
{
    public static async Task Run(ExternalIntegrationConnection connection, BasalamOAuthStore tokens,
        Action<bool, string> check)
    {
        const string first = """{"data":[{"id":12,"title":"A","inventory":0}],"page":1,"total_page":2}""";
        const string last = """{"data":[{"id":13,"title":"B","price":20,"variants":[{"id":80,"title":"Blue","stock":0},{"id":81,"price":0}]}],"page":2,"total_page":2}""";
        using var transport = new PagesTransport();
        using var http = new HttpClient(transport);
        using var sdk = new BasalamClient(new BasalamConfig(), httpClient: http);
        var adapter = new BasalamSdkAdapter(sdk, tokens);
        transport.Reset(first, last);
        var catalog = await adapter.ReadCatalogAsync(connection, default);
        check(catalog.Count == 3 && catalog.First() is { ExternalProductId: "12", VariantId: null, Inventory: 0 }
            && catalog.Skip(1).First() is { ExternalProductId: "13", VariantId: "80", Inventory: 0, Price: 20 }
            && catalog.Last() is { ExternalProductId: "13", VariantId: "81", Inventory: null, Price: 0 },
            "catalog reads all pages and preserves parent/variant identity, zero and unknown values");
        check(transport.Queries.Count == 2 && transport.Queries[0].Contains("page=1&per_page=100")
            && transport.Queries[1].Contains("page=2&per_page=100")
            && transport.Queries.All(q => q.Contains("variants_flatting=false")),
            "catalog advances pages with selected connection grant and parent-preserving query");
        transport.Reset("""{"data":[{"id":12,"title":"A"}]}""", """{"data":[]}""");
        check((await adapter.ReadCatalogAsync(connection, default)).Count == 1 && transport.Queries.Count == 2,
            "catalog without pagination metadata reads through empty terminator");
        transport.Reset("""{"data":[],"total_page":0,"total_count":0}""");
        check((await adapter.ReadCatalogAsync(connection, default)).Count == 0, "empty booth remains successful");
        foreach (var (pages, code, name) in new (string[], string, string)[]
        {
            ([first, """{"data":[],"page":2,"total_page":3}"""], "CatalogInvalidResponse", "missing middle page"),
            ([first, """{"data":[],"page":2,"total_page":2}"""], "CatalogInvalidResponse", "missing last page"),
            ([first, first], "CatalogInvalidResponse", "repeated page metadata"),
            ([first, """{"data":[{"id":12,"title":"Duplicate"}],"page":2,"total_page":2}"""], "CatalogIdentityConflict", "product repeated across pages"),
            (["""{"data":[{"id":12,"title":"A"},{"id":12,"title":"A"}],"hasMore":false}"""], "CatalogIdentityConflict", "product repeated within page"),
            (["""{"data":[{"id":12,"title":"A","variants":[{"id":80},{"id":80}]}],"hasMore":false}"""], "CatalogIdentityConflict", "repeated variant"),
            (["""{"data":[{"id":12,"title":"A","variants":[{"id":80}]},{"id":13,"title":"B","variants":[{"id":80}]}],"hasMore":false}"""], "CatalogIdentityConflict", "variant shared by two parents"),
            (["""{"data":[{"id":12,"title":"A","vendor":{"id":72}}]}"""], "CatalogInvalidResponse", "different vendor"),
            ([first, "{\"data\":\"sensitive-fixture\"}"], "CatalogInvalidResponse", "malformed later page"),
            (["""{"data":[{"id":0,"title":"A"}]}"""], "CatalogInvalidResponse", "invalid product ID")
        })
        {
            transport.Reset(pages);
            try { await adapter.ReadCatalogAsync(connection, default); check(false, name + " rejected atomically"); }
            catch (IntegrationProviderException ex)
            {
                check(ex.Code == code && !ex.Retryable && ex.Message == code && ex.InnerException is null,
                    name + " rejected atomically with safe terminal code");
            }
        }
    }

    private sealed class PagesTransport : HttpMessageHandler
    {
        private readonly Queue<string> pages = new();
        public List<string> Queries { get; } = [];
        public void Reset(params string[] bodies)
        {
            pages.Clear(); Queries.Clear();
            foreach (var body in bodies) pages.Enqueue(body);
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            if (request.Method != HttpMethod.Get || request.RequestUri?.AbsolutePath != "/v1/vendors/71/products"
                || request.Headers.Authorization?.ToString() != "Bearer fixture-grant" || pages.Count == 0)
                throw new InvalidOperationException("Unexpected catalog fixture request");
            Queries.Add(request.RequestUri.Query);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(pages.Dequeue(), Encoding.UTF8, "application/json") });
        }
    }
}
