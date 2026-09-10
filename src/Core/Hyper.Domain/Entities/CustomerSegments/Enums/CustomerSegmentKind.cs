namespace Hyper.Domain.Entities.CustomerSegments.Enums;

public enum CustomerSegmentKind
{
    [Description("جمعیت‌شناختی (Demographic) - بر اساس سن، جنسیت، درآمد، تحصیلات، شغل و وضعیت خانوادگی")]
    Demographic = 1,

    [Description("جغرافیایی (Geographic) - بر اساس موقعیت مکانی، اقلیم، منطقه، زبان یا کشور")]
    Geographic = 2,

    [Description("روان‌شناختی (Psychographic) - بر اساس شخصیت، ارزش‌ها، نگرش‌ها و سبک زندگی")]
    Psychographic = 3,

    [Description("رفتاری (Behavioral) - بر اساس عادات خرید، وفاداری، زمان خرید و مزایای مورد انتظار")]
    Behavioral = 4,

    [Description("الگوی مصرف (Usage-based) - بر اساس میزان و نوع استفاده از محصولات یا خدمات")]
    UsageBased = 5,

    [Description("فناورانه (Technographic) - بر اساس رفتار دیجیتال، نوع دستگاه، سیستم‌عامل و آشنایی با فناوری")]
    Technographic = 6,

    [Description("سازمانی (Firmographic) - مخصوص بازار B2B؛ بر اساس صنعت، اندازه شرکت، درآمد و ساختار سازمانی")]
    Firmographic = 7,

    [Description("ارزش مشتری (Value-based) - بر اساس ارزش طول عمر مشتری (CLV) و سودآوری او برای کسب‌وکار")]
    ValueBased = 8,

    [Description("سفارشی (Custom) - تقسیم‌بندی خاص تعریف‌شده توسط سازمان یا مدیر بازاریابی")]
    Custom = 9
}
