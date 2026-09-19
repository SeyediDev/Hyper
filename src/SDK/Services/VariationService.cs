using Hyper.SDK.Auth;
using Hyper.SDK.Clients;
using Hyper.SDK.Config;
using Hyper.SDK.Errors;
using Hyper.SDK.Models;
using Microsoft.Extensions.Logging;

namespace Hyper.SDK.Services;

public interface IVariationService
{
    Task<Variation?> GetVariationAsync(int variationId, CancellationToken ct = default);
    Task PatchStockAsync(int variationId, int stock, CancellationToken ct = default);
}

public sealed class VariationService : IVariationService
{
    private readonly IBasalamHttpClient _client;
    private readonly ILogger<VariationService>? _logger;

    public VariationService(IBasalamHttpClient client, ILogger<VariationService>? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<Variation?> GetVariationAsync(int variationId, CancellationToken ct = default)
    {
        _logger?.LogInformation("Getting variation {VariationId}", variationId);
        return await _client.GetAsync<Variation>($"/v1/variations/{variationId}", ct);
    }

    public async Task PatchStockAsync(int variationId, int stock, CancellationToken ct = default)
    {
        _logger?.LogInformation("Patching stock for variation {VariationId} to {Stock}", variationId, stock);
        if (stock < 0)
            throw new BasalamValidationError(new Dictionary<string, IReadOnlyList<string>>
            {
                ["stock"] = ["Stock must be non-negative"]
            });
        await _client.PatchAsync<object>($"/v1/variations/{variationId}", new { stock }, ct);
    }
}
