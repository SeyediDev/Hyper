namespace Hyper.Integration.Domain.Entities.Integrations;

/// <summary>Explicit grant from a Hyperyek account identity to a shop; unrelated to administrator users.</summary>
public sealed class IntegrationMerchantAccess
{
    public long Id { get; set; }
    public string Issuer { get; set; } = null!;
    public string SubjectId { get; set; } = null!;
    public int ShopId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = null!;
    public DateTime? ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}

