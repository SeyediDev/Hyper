namespace Hyper.Integration.Domain.Entities.Integrations;

/// <summary>Maps a provider customer identity to an accounting person identifier.</summary>
public sealed class IntegrationCustomerMapping
{
    public long Id { get; set; }
    public int ShopId { get; set; }
    public string TenantId { get; set; } = null!;
    public string ExternalCustomerId { get; set; } = null!;
    public int PersonId { get; set; }
}
