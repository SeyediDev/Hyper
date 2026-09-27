using System.Net;
using System.Text;
using System.Text.Json;
using Basalam.SDK;
using Basalam.SDK.Config;

internal static class CatalogPageChecks
{
    public static async Task Run(Action<bool, string> check)
    {
        using var transport = new PageTransport();
        using var http = new HttpClient(transport);
        using var sdk = new BasalamClient(new BasalamConfig(), httpClient: http);
        foreach (var (body, page, name) in new (string, int, string)[]
        {
            ("""{"data":[],"hasMore":true}""", 1, "empty page promising more"),
            ("""{"data":[],"page":2,"total_page":3}""", 2, "empty middle page"),
            ("""{"data":[],"page":2,"total_page":2}""", 2, "empty declared final page"),
            ("""{"data":[],"page":1,"total_count":4}""", 1, "empty nonempty catalog"),
            ("""{"data":[],"page":1}""", 2, "repeated page number"),
            ("""{"data":[],"page":0}""", 1, "zero page"),
            ("""{"data":[],"total_page":-1}""", 1, "negative page count"),
            ("""{"data":[],"total_count":-1}""", 1, "negative item count"),
            ("""{"data":[],"per_page":0}""", 1, "zero page size"),
            ("""{"data":[],"page":2,"total_page":1}""", 2, "page beyond declared end"),
            ("""{"data":[],"hasMore":"false"}""", 1, "malformed continuation flag"),
            ("""{"data":[{"id":12,"title":"A"}],"total_page":0}""", 1, "items with zero pages"),
            ("""{"data":[{"id":12,"title":"A"}],"total_count":0}""", 1, "items with zero total"),
            ("""{"data":[{"id":12,"title":"A"}],"total_page":2,"hasMore":false}""", 1, "conflicting pagination metadata")
        })
        {
            transport.Body = body;
            try { await sdk.Catalog.GetProductsAsync(71, page); check(false, name + " rejected"); }
            catch (JsonException) { check(true, name + " rejected"); }
        }
        foreach (var body in new[] { """{"data":[]}""", """{"data":[],"total_page":0,"total_count":0}""",
                     """{"data":[],"total_page":1,"total_count":0,"hasMore":false}""" })
        {
            transport.Body = body;
            var result = await sdk.Catalog.GetProductsAsync(71);
            check(result.Data.Count == 0 && !result.HasMore, "valid empty booth accepted");
        }
        transport.Body = """{"data":[],"total_count":1}""";
        check(!(await sdk.Catalog.GetProductsAsync(71, 2)).HasMore,
            "metadata-free traversal may finish with empty page and global total");
        transport.Body = """{"data":[{"id":13,"title":"B"}],"page":2,"total_page":2,"total_count":2}""";
        check(!(await sdk.Catalog.GetProductsAsync(71, 2)).HasMore, "nonempty last page ends traversal");
    }

    private sealed class PageTransport : HttpMessageHandler
    {
        public string Body = "{}";
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StringContent(Body, Encoding.UTF8, "application/json") });
    }
}
