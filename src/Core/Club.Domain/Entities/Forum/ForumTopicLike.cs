namespace Hyper.Domain.Entities.Forum;

/// <summary>
/// لایک موضوع انجمن
/// </summary>
[DisplayName("لایک موضوع")]
public class ForumTopicLike : HyperBaseCoreAuditableEntity<int>
{
    /// <summary>
    /// شناسه موضوع
    /// </summary>
    [DisplayName("شناسه موضوع")]
    public int TopicId { get; set; }

    /// <summary>
    /// موضوع
    /// </summary>
    [DisplayName("موضوع")]
    public ForumTopic Topic { get; set; } = null!;

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



