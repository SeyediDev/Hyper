namespace Hyper.Domain.Features.Channels;

public interface IEventRequest
{
    public int TenantId { get; }
    public int? ChannelId { get; }
    public int? EventTypeId { get; }
    public int? ProductCategoryId { get; }
    public int? ProductId { get; }

    public string CustomerMobile { get; }
    /// <summary>
    /// کد معرف (Referrer Code) - برای رویدادهای ثبت معرف
    /// </summary>
    public string? ReferrerCode { get; }
    public AttributesValuesList? Attributes { get; }
}
public record EventRequest: IEventRequest
{
    public int TenantId { get; set; }
    public int? ChannelId { get; set; }
    public int? EventTypeId { get; set; }
    public int? ProductCategoryId { get; set; }
    public int? ProductId { get; set; }
    public string CustomerMobile { get; set; } = null!;
    /// <summary>
    /// کد معرف (Referrer Code) - برای رویدادهای ثبت معرف
    /// </summary>
    public string? ReferrerCode { get; set; }
    public AttributesValuesList? Attributes { get; set; }

    public bool IsInquiry { get; set; }
    public ReceiveEventType ReceiveEventType { get; set; }
    public int? PromotionId { get; set; }
    public int? PointLevelId { get; set; }
    public int? RewardId { get; set; }
    public int? AssetId { get; set; }
}
public record EventResponse
{
    public long EventLogId { get; set; }
    public int CustomerTenantId { get; set; }
    /// <summary>
    /// نتیجه ارزیابی کمپین‌ها (فقط در حالت استعلام)
    /// </summary>
    public PromotionEvaluationResponse? PromotionEvaluation { get; set; }
    public List<CustomerSegmentInfo> Segments { get; set; } = null!;
    public PromotionProcessingResponse? PromotionProcessingResponse { get; internal set; }
    public AttributesValues AttributeValues { get; internal set; } = [];
}
