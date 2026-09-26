namespace Basalam.SDK.Services;

public interface IVariationService
{
    Task<Variation?> GetVariationAsync(int variationId, CancellationToken ct = default);
    Task<Variation?> CreateVariationAsync(VariationWriteRequest variation, CancellationToken ct = default);
    Task<Variation?> UpdateVariationAsync(int variationId, VariationWriteRequest variation, CancellationToken ct = default);
    Task PatchStockAsync(int variationId, int stock, CancellationToken ct = default);
}

public sealed class VariationService(IBasalamHttpClient client, ILogger<VariationService>? logger = null) : IVariationService
{
    public async Task<Variation?> GetVariationAsync(int variationId, CancellationToken ct = default)
    {
        ValidateId(variationId);
        logger?.LogInformation("Getting variation {VariationId}", variationId);
        return await client.GetAsync<Variation>($"/v1/variations/{variationId}", ct);
    }

    public Task<Variation?> CreateVariationAsync(VariationWriteRequest variation, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(variation); ValidateVariation(variation);
        return client.PostAsync<Variation>("/v1/variations", ToWire(variation), ct);
    }

    public Task<Variation?> UpdateVariationAsync(int variationId, VariationWriteRequest variation, CancellationToken ct = default)
    {
        ValidateId(variationId); ArgumentNullException.ThrowIfNull(variation); ValidateVariation(variation);
        return client.PatchAsync<Variation>($"/v1/variations/{variationId}", ToWire(variation), ct);
    }

    public async Task PatchStockAsync(int variationId, int stock, CancellationToken ct = default)
    {
        ValidateId(variationId);
        logger?.LogInformation("Patching stock for variation {VariationId} to {Stock}", variationId, stock);
        if (stock < 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            {
                ["stock"] = ["Stock must be non-negative"]
            });
        await client.PatchAsync<object>($"/v1/variations/{variationId}", new { stock }, ct);
    }

    private static void ValidateVariation(VariationWriteRequest variation)
    {
        if (variation.ProductId <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["productId"] = ["ProductId must be positive"] });
        if (variation.Stock < 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["stock"] = ["Stock must be non-negative"] });
    }

    private static object ToWire(VariationWriteRequest variation) => new
    {
        product_id = variation.ProductId,
        title = variation.Title,
        primary_price = variation.Price,
        sku = variation.Sku,
        barcode = variation.Barcode,
        stock = variation.Stock
    };
    private static void ValidateId(int id)
    {
        if (id <= 0) throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            { ["id"] = ["Identifier must be positive"] });
    }
}
