using Hyper.Domain.Features.Attributes;

namespace Hyper.Channel.Api.Controllers.Dto.Events;

public sealed record ReceiveDynamicEventRequestDto
{
    [MaxLength(128)]
    public string? IdempotencyKey { get; init; }

    /// <summary>
    /// آیا این درخواست استعلام است یا ارسال واقعی؟
    /// true = استعلام (فقط بررسی صحت پارامترها و محاسبه امتیاز احتمالی)
    /// false = ارسال واقعی رویداد
    /// </summary>
    public bool IsInquiry { get; init; } = false;

    [Required]
    [MaxLength(15)]
    public string CustomerMobile { get; init; } = null!;

    [Required]
    [MaxLength(128)]
    public string EventTypeKey { get; init; } = null!;

    [MaxLength(128)]
    public string? ProductCategoryKey { get; init; }

    [MaxLength(128)]
    public string? ProductKey { get; init; }

    /// <summary>
    /// کد معرف (Referrer Code) - برای رویدادهای ثبت معرف
    /// </summary>
    [MaxLength(128)]
    public string? ReferrerCode { get; init; }

    public AttributesValuesList? Attributes { get; init; }
}