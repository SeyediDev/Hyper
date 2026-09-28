using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Basalam.SDK.Services;

public sealed record ProductDraftRequest(string Name, long PrimaryPrice, int CategoryId,
    int PreparationDays, int PackageWeight, string? Description = null, long? PhotoId = null);
public interface IProductDraftService
{
    Task<int> CreateDraftAsync(int vendorId, ProductDraftRequest draft, CancellationToken ct = default);
}

public sealed partial class ProductService : IProductDraftService
{
    public async Task<int> CreateDraftAsync(int vendorId, ProductDraftRequest draft, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(draft);
        if (vendorId <= 0 || string.IsNullOrWhiteSpace(draft.Name) || draft.Name.Length > 500
            || draft.PrimaryPrice < 0 || draft.CategoryId <= 0 || draft.PreparationDays < 0
            || draft.PackageWeight <= 0 || draft.PhotoId is <= 0 || draft.Description?.Length > 10000)
            throw new ArgumentException("InvalidProductDraft");
        using var request = new HttpRequestMessage(HttpMethod.Post,
            $"/v1/vendors/{vendorId.ToString(CultureInfo.InvariantCulture)}/products")
        {
            Content = JsonContent.Create(new
            {
                name = draft.Name, primary_price = draft.PrimaryPrice, category_id = draft.CategoryId,
                preparation_days = draft.PreparationDays, package_weight = draft.PackageWeight,
                description = draft.Description, photo = draft.PhotoId,
                status = 3790, stock = 0, is_wholesale = false
            }, options: new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull })
        };
        request.Options.Set(BasalamHttpClient.DisableRetries, true);
        using var response = await client.SendAsync(request, ct);
        // Creation has no documented idempotency key: never repeat POST here.
        // Even 5xx/timeout may have committed remotely. Do not retain raw errors.
        if (response.StatusCode != System.Net.HttpStatusCode.Created)
            throw new BasalamAPIError("Product creation requires investigation", (int)response.StatusCode, null);
        using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
        var root = json.RootElement;
        if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("id", out var id)
            || !id.TryGetInt32(out var value) || value <= 0) throw new JsonException("Invalid created identity");
        return value;
    }
}
