namespace Hyper.Domain.Entities.Integrations;

public sealed class IntegrationAdminSimulation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AdminUserId { get; set; } = null!;
    public int ShopId { get; set; }
    public string MerchantIdentifier { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string ShopName { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
}

public sealed class IntegrationTokenRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SimulationId { get; set; }
    public IntegrationProvider Provider { get; set; }
    public IntegrationCredentialType CredentialType { get; set; }
    public byte Status { get; set; } // 0 = prepared simulation; no provider token has been issued.
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
}
