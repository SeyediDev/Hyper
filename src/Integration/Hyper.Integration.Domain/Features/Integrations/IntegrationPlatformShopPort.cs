namespace Hyper.Integration.Domain.Features.Integrations;

public sealed record IntegrationPlatformShop(
    int ShopId,
    string ShopName,
    string MerchantIdentifier,
    string TenantId);

/// <summary>Read-only anti-corruption port for platform shop metadata.</summary>
public interface IIntegrationPlatformShopPort
{
    Task<IReadOnlyList<IntegrationPlatformShop>> SearchAsync(string? search, CancellationToken cancellationToken);
    Task<IReadOnlyList<IntegrationPlatformShop>> GetByIdsAsync(IReadOnlyCollection<int> shopIds,
        CancellationToken cancellationToken);
    Task<IntegrationPlatformShop?> GetAsync(int shopId, CancellationToken cancellationToken);
}
