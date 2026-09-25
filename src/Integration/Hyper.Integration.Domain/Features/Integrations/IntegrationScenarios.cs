namespace Hyper.Integration.Domain.Features.Integrations;

public enum IntegrationSyncItem : byte { Product = 1, Inventory = 2, Counterparty = 3, Sale = 4, Purchase = 5, Subscription = 6, Review = 7, Chat = 8 }
public enum IntegrationSyncTrigger : byte { StoreChanged = 1, BoothChanged = 2, Initial = 3, Periodic = 4, Manual = 5 }
public enum IntegrationScenarioStatus : byte { Pending = 0, Running = 1, Completed = 2, NeedsAttention = 3, DeadLetter = 4 }

public sealed record IntegrationScenarioRequest(string EventId, IntegrationSyncItem Item, IntegrationSyncTrigger Trigger);
public sealed record IntegrationScenarioConnection(long Id, string DisplayName, IntegrationProvider Provider, bool IsEnabled);
public sealed record IntegrationDifference(int? HyperProductId, string? ExternalProductId, string? VariantId, string Code);
public sealed record IntegrationScenarioResult(int Compared, IReadOnlyList<IntegrationDifference> Differences, int Enqueued = 0);

public interface IIntegrationScenarioQueue
{
    Task<long> EnqueueAsync(OwnedIntegrationShop shop, long connectionId, IntegrationScenarioRequest request, CancellationToken ct);
    Task<bool> ProcessNextAsync(CancellationToken ct);
    Task<IReadOnlyList<IntegrationScenarioConnection>> ConnectionsAsync(OwnedIntegrationShop shop, CancellationToken ct);
    Task<IReadOnlyList<IntegrationScenarioJob>> RecentAsync(OwnedIntegrationShop shop, CancellationToken ct);
}

// Neo publisher notifications hand off to durable SQL; callers must await publication and publish after commit.
public sealed class IntegrationScenarioRequested(OwnedIntegrationShop shop, long connectionId,
    IntegrationScenarioRequest request) : Neo.Domain.Entities.Base.BaseEvent
{
    public OwnedIntegrationShop Shop { get; } = shop;
    public long ConnectionId { get; } = connectionId;
    public IntegrationScenarioRequest Request { get; } = request;
}

public static class IntegrationScenarioRules
{
    public static void Validate(OwnedIntegrationShop shop, long connectionId, IntegrationScenarioRequest request)
    {
        if (shop.ShopId <= 0 || string.IsNullOrWhiteSpace(shop.TenantId) || shop.TenantId.Length > 128 || connectionId <= 0)
            throw new ArgumentException("InvalidConnectionScope");
        if (string.IsNullOrWhiteSpace(request.EventId) || request.EventId.Length > 128
            || request.EventId != request.EventId.Trim() || request.EventId.Any(char.IsControl)
            || !Enum.IsDefined(request.Item) || !Enum.IsDefined(request.Trigger))
            throw new ArgumentException("InvalidScenarioEvent");
    }
}

public sealed record IntegrationLocalProduct(int Id, string Title, decimal AvailableInventory,
    string? Sku = null, decimal? Price = null);

/// <summary>Approved mappings only. Missing or duplicate identities require action, never automatic merging.</summary>
public static class IntegrationCatalogComparison
{
    public static IntegrationScenarioResult Compare(IReadOnlyCollection<IntegrationLocalProduct> products,
        IReadOnlyCollection<ExternalCatalogItem> remote, IReadOnlyCollection<ExternalProductMapping> mappings,
        IntegrationSyncItem item)
    {
        var differences = new List<IntegrationDifference>();
        var local = products.ToDictionary(x => x.Id);
        var external = remote.ToDictionary(x => (x.ExternalProductId, x.VariantId));
        var mappedLocal = new HashSet<int>();
        var mappedRemote = new HashSet<(string, string?)>();
        var compared = 0;
        foreach (var mapping in mappings.Where(x => x.IsActive))
        {
            var key = (mapping.ExternalProductId, mapping.ExternalVariantId);
            mappedLocal.Add(mapping.HyperProductId);
            if (!mappedRemote.Add(key)) { Add("DuplicateMapping"); continue; }
            if (!local.TryGetValue(mapping.HyperProductId, out var source)) { Add("LocalProductMissing"); continue; }
            if (!external.TryGetValue(key, out var target)) { Add("ExternalProductMissing"); continue; }
            compared++;
            if (item == IntegrationSyncItem.Product && !string.Equals(source.Title.Trim(), target.Title.Trim(), StringComparison.Ordinal))
                Add("ProductTitleMismatch");
            if (item == IntegrationSyncItem.Product && !string.IsNullOrWhiteSpace(source.Sku)
                && !string.Equals(source.Sku.Trim(), target.Sku?.Trim(), StringComparison.OrdinalIgnoreCase))
                Add("ProductSkuMismatch");
            if (item == IntegrationSyncItem.Product && source.Price is { } localPrice
                && target.Price is { } remotePrice && localPrice != remotePrice)
                Add("ProductPriceMismatch");
            if (item == IntegrationSyncItem.Inventory && source.AvailableInventory != target.Inventory)
                Add(target.Inventory is null ? "ExternalInventoryUnknown" : "InventoryMismatch");
            void Add(string code) => differences.Add(new(mapping.HyperProductId, mapping.ExternalProductId, mapping.ExternalVariantId, code));
        }
        foreach (var source in products.Where(x => !mappedLocal.Contains(x.Id)))
            differences.Add(new(source.Id, null, null, "LocalProductUnmapped"));
        foreach (var target in remote.Where(x => !mappedRemote.Contains((x.ExternalProductId, x.VariantId))))
            differences.Add(new(null, target.ExternalProductId, target.VariantId, "ExternalProductUnmapped"));
        return new(compared, differences);
    }
}

