namespace Hyper.Domain.Entities.Forum;

/// <summary>
/// لایک پست انجمن
/// </summary>
[DisplayName("لایک پست")]
public class ForumPostLike : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه پست
    /// </summary>
    [DisplayName("شناسه پست")]
    public int PostId { get; set; }

    /// <summary>
    /// پست
    /// </summary>
    [DisplayName("پست")]
    public ForumPost Post { get; set; } = null!;

    /// <summary>
    /// شناسه مشتری
    /// </summary>
    [DisplayName("شناسه مشتری")]
    public int CustomerTenantId { get; set; }

    /// <summary>
    /// مشتری
    /// </summary>
    [DisplayName("مشتری")]
    public CustomerTenant CustomerTenant { get; set; } = null!;

    /// <summary>
    /// تاریخ لایک
    /// </summary>
    [DisplayName("تاریخ")]
    public DateTime LikedDate { get; set; } = DateTime.UtcNow;
}



