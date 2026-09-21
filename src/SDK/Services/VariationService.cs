namespace Hyper.SDK.Services;

public interface IVariationService
{
    Task<Variation?> GetVariationAsync(int variationId, CancellationToken ct = default);
    Task PatchStockAsync(int variationId, int stock, CancellationToken ct = default);
}

public sealed class VariationService(IBasalamHttpClient client, ILogger<VariationService>? logger = null) : IVariationService
{
    public async Task<Variation?> GetVariationAsync(int variationId, CancellationToken ct = default)
    {
        logger?.LogInformation("Getting variation {VariationId}", variationId);
        return await client.GetAsync<Variation>($"/v1/variations/{variationId}", ct);
    }

    public async Task PatchStockAsync(int variationId, int stock, CancellationToken ct = default)
    {
        logger?.LogInformation("Patching stock for variation {VariationId} to {Stock}", variationId, stock);
        if (stock < 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            {
                ["stock"] = ["Stock must be non-negative"]
            });
        await client.PatchAsync<object>($"/v1/variations/{variationId}", new { stock }, ct);
    }
}
