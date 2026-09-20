using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

// Contract: basalam/python-sdk openapi_data/core.json (see docs/BASALAM_CONTRACT.md).
public sealed class BasalamIntegrationAdapter(IHttpClientFactory clients, HyperContextCommand db,
    BasalamOAuthService oauth) : ExternalIntegrationAdapterBase(clients)
{
    private const string Origin = "https://openapi.basalam.com";
    public override IntegrationProvider Provider => IntegrationProvider.Basalam;
    public override bool IsImplemented => true;
    public override bool SupportsCredentialType(IntegrationCredentialType type) =>
        type is IntegrationCredentialType.OAuth2 or IntegrationCredentialType.BearerToken;

    public override async Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(
        ExternalIntegrationConnection connection, CancellationToken cancellationToken)
    {
        Validate(connection);
        var vendor = Identifier(connection.AccountIdentifier);
        var result = new List<ExternalCatalogItem>();
        var seen = new HashSet<(string, string?)>();
        using var client = Client;
        for (var page = 1; page <= 10000; page++)
        {
            using var request = await RequestAsync(connection, HttpMethod.Get,
                $"/v1/vendors/{vendor}/products?page={page}&per_page=100&variants_flatting=false&sort=id:asc", cancellationToken);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            EnsureSuccess(response);
            using var json = await ReadJson(response, cancellationToken);
            var root = json.RootElement;
            if (!root.TryGetProperty("data", out var data))
                throw new IntegrationProviderException("InvalidCatalog", false);
            if (data.ValueKind == JsonValueKind.Null && OptionalInteger(root, "result_count") == 0
                && OptionalInteger(root, "total_page") is null or 0) return result;
            if (data.ValueKind != JsonValueKind.Array)
                throw new IntegrationProviderException("InvalidCatalog", false);
            if (OptionalInteger(root, "page") is { } current && current != page)
                throw new IntegrationProviderException("InvalidPagination", false);
            foreach (var product in data.EnumerateArray())
            {
                var id = Id(product, "id");
                var title = product.GetProperty("title").GetString()
                    ?? throw new IntegrationProviderException("InvalidCatalog", false);
                if (product.TryGetProperty("variant", out var variants) && variants.ValueKind == JsonValueKind.Array
                    && variants.GetArrayLength() > 0)
                {
                    foreach (var variant in variants.EnumerateArray())
                        Add(new(id, OptionalString(variant, "sku"), title, OptionalDecimal(variant, "price"),
                            variant.GetProperty("stock").GetDecimal(), Id(variant, "id")));
                }
                else Add(new(id, OptionalString(product, "sku"), title, OptionalDecimal(product, "price"),
                    product.GetProperty("inventory").GetDecimal(), null));
            }
            var totalPages = OptionalInteger(root, "total_page");
            if (totalPages is < 0 || (data.GetArrayLength() == 0 && totalPages > page))
                throw new IntegrationProviderException("InvalidPagination", false);
            if (totalPages.HasValue ? page >= totalPages.Value : data.GetArrayLength() == 0)
                return result;
        }
        throw new IntegrationProviderException("CatalogLimitExceeded", false);

        void Add(ExternalCatalogItem item)
        {
            if (!seen.Add((item.ExternalProductId, item.VariantId)))
                throw new IntegrationProviderException("DuplicateCatalogIdentifier", false);
            result.Add(item);
        }
    }

    public override async Task PublishInventoryAsync(ExternalIntegrationConnection connection,
        IReadOnlyCollection<ExternalInventoryUpdate> updates, CancellationToken cancellationToken)
    {
        Validate(connection);
        // Validate the entire batch before any write. Basalam stock is an integer count.
        foreach (var update in updates)
        {
            Identifier(update.ExternalProductId);
            if (update.VariantId is not null) Identifier(update.VariantId);
            if (update.Quantity < 0 || update.Quantity > int.MaxValue || decimal.Truncate(update.Quantity) != update.Quantity)
                throw new IntegrationProviderException("InvalidStockQuantity", false);
        }
        using var client = Client;
        foreach (var update in updates)
        {
            var path = $"/v1/products/{Identifier(update.ExternalProductId)}";
            // Recheck remote ownership and variant membership before publishing a mapping.
            using (var lookup = await RequestAsync(connection, HttpMethod.Get, path, cancellationToken))
            using (var read = await client.SendAsync(lookup, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                EnsureSuccess(read);
                using var json = await ReadJson(read, cancellationToken);
                var product = json.RootElement;
                if (Id(product, "id") != update.ExternalProductId
                    || Id(product.GetProperty("vendor"), "id") != connection.AccountIdentifier)
                    throw new IntegrationProviderException("VendorMismatch", false);
                var hasVariants = product.TryGetProperty("variants", out var variants)
                    && variants.ValueKind == JsonValueKind.Array && variants.GetArrayLength() > 0;
                if (update.VariantId is null ? hasVariants : !hasVariants || !variants.EnumerateArray().Any(v => Id(v, "id") == update.VariantId))
                    throw new IntegrationProviderException("VariantMismatch", false);
            }
            if (update.VariantId is not null) path += $"/variations/{Identifier(update.VariantId)}";
            using var request = await RequestAsync(connection, HttpMethod.Patch, path, cancellationToken);
            request.Content = JsonContent.Create(new { stock = (int)update.Quantity });
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            EnsureSuccess(response);
        }
    }

    private void Validate(ExternalIntegrationConnection connection)
    {
        IntegrationConnectionReadiness.Validate(connection, DateTime.UtcNow);
        if (connection.Provider != Provider || !SupportsCredentialType(connection.CredentialType))
            throw new NotSupportedException("Unsupported Basalam credential strategy.");
        Identifier(connection.AccountIdentifier);
    }

    private async Task<HttpRequestMessage> RequestAsync(ExternalIntegrationConnection connection, HttpMethod method,
        string path, CancellationToken ct = default)
    {
        var stored = await db.ExternalOAuthTokens.SingleOrDefaultAsync(x =>
            x.ConnectionId == connection.Id && x.ShopId == connection.ShopId
            && x.TenantId == connection.TenantId && x.Provider == IntegrationProvider.Basalam
            && x.IsActive, ct);
        if (stored is null || string.IsNullOrWhiteSpace(stored.AccessToken))
            throw new IntegrationProviderException("InvalidCredentials", false);
        if (stored.ExpiresAtUtc <= DateTime.UtcNow.AddMinutes(1))
        {
            if (string.IsNullOrWhiteSpace(stored.RefreshToken))
                throw new IntegrationProviderException("ExpiredCredentials", false);
            BasalamTokenResponse refreshed;
            try { refreshed = await oauth.RefreshAccessTokenAsync(oauth.DecryptToken(stored.RefreshToken), ct); }
            catch (InvalidOperationException) { throw new IntegrationProviderException("ExpiredCredentials", false); }
            oauth.UpdateTokenEntity(stored, refreshed);
            await db.SaveChangesAsync(ct);
        }
        string token;
        try { token = oauth.DecryptToken(stored.AccessToken); }
        catch (Exception) { throw new IntegrationProviderException("InvalidCredentials", false); }
        if (string.IsNullOrWhiteSpace(token) || token.Any(char.IsWhiteSpace)
            || !string.Equals(stored.TokenType, "Bearer", StringComparison.OrdinalIgnoreCase))
            throw new IntegrationProviderException("InvalidCredentials", false);
        var request = new HttpRequestMessage(method, Origin + path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return request;
    }

    private static string Identifier(string id)
    {
        if (!long.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out var value) || value <= 0
            || value.ToString(CultureInfo.InvariantCulture) != id)
            throw new IntegrationProviderException("InvalidExternalIdentifier", false);
        return id;
    }
    private static string Id(JsonElement value, string name) => Identifier(value.GetProperty(name).GetInt64().ToString(CultureInfo.InvariantCulture));
    private static int? OptionalInteger(JsonElement value, string name) => value.TryGetProperty(name, out var item) && item.ValueKind != JsonValueKind.Null ? item.GetInt32() : null;
    private static decimal? OptionalDecimal(JsonElement value, string name) => value.TryGetProperty(name, out var item) && item.ValueKind != JsonValueKind.Null ? item.GetDecimal() : null;
    private static string? OptionalString(JsonElement value, string name) => value.TryGetProperty(name, out var item) && item.ValueKind != JsonValueKind.Null ? item.GetString() : null;
    private static async Task<JsonDocument> ReadJson(HttpResponseMessage response, CancellationToken ct)
    {
        // Bounded buffering also applies when Content-Length is absent.
        await response.Content.LoadIntoBufferAsync(8 * 1024 * 1024, ct);
        return JsonDocument.Parse(await response.Content.ReadAsByteArrayAsync(ct));
    }
    private static void EnsureSuccess(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.OK) return;
        var status = (int)response.StatusCode;
        var retry = response.Headers.RetryAfter?.Delta
            ?? (response.Headers.RetryAfter?.Date is { } date ? date - DateTimeOffset.UtcNow : null);
        // Never include provider bodies, URLs, or credentials in errors/audit logs.
        throw new IntegrationProviderException($"Http{status}", response.StatusCode is HttpStatusCode.RequestTimeout
            or HttpStatusCode.TooManyRequests || status >= 500, retry);
    }
}
