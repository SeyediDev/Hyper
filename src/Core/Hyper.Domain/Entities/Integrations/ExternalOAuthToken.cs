namespace Hyper.Domain.Entities.Integrations;

/// <summary>
/// نمایندگی توکن OAuth2 از سرویس‌های بیرونی مثل Basalam
/// </summary>
public sealed class ExternalOAuthToken
{
    public long Id { get; set; }

    /// <summary>
    /// Reference به ExternalIntegrationConnection
    /// </summary>
    public long ConnectionId { get; set; }

    /// <summary>
    /// شناسه فروشگاه
    /// </summary>
    public int ShopId { get; set; }

    /// <summary>
    /// شناسه کرایه (Tenant)
    /// </summary>
    public string TenantId { get; set; } = null!;

    /// <summary>
    /// سرویس دهنده (Basalam, ...)
    /// </summary>
    public IntegrationProvider Provider { get; set; }

    /// <summary>
    /// Access Token (رمزنگاری شده)
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// Refresh Token (رمزنگاری شده)
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// نوع توکن (Bearer, etc.)
    /// </summary>
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// تاریخ انقضای توکن (UTC)
    /// </summary>
    public DateTime? ExpiresAtUtc { get; set; }

    /// <summary>
    /// اسکوپ‌های اعطاءشده (comma-separated)
    /// </summary>
    public string? Scopes { get; set; }

    /// <summary>
    /// تاریخ ایجاد توکن
    /// </summary>
    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// آخرین به‌روزرسانی توکن
    /// </summary>
    public DateTime? UpdatedAtUtc { get; set; }

    /// <summary>
    /// آیا توکن فعال است
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// داده‌های اضافی (JSON)
    /// </summary>
    public string? RawTokenResponse { get; set; }
}
