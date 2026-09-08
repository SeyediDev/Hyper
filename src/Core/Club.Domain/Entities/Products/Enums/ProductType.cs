namespace Hyper.Domain.Entities.Products.Enums;

/// <summary>
/// نوع محصول یا خدمت
/// </summary>
public enum ProductType
{
    /// <summary>
    /// محصول فیزیکی
    /// </summary>
    [Description("محصول فیزیکی")]
    PhysicalProduct = 1,

    /// <summary>
    /// خدمت
    /// </summary>
    [Description("خدمت")]
    Service = 2,

    /// <summary>
    /// محصول دیجیتالی
    /// </summary>
    [Description("محصول دیجیتالی")]
    DigitalProduct = 3,

    /// <summary>
    /// دوره یا رویداد
    /// </summary>
    [Description("دوره یا رویداد")]
    CourseOrEvent = 4,

    /// <summary>
    /// خدمات مشاوره‌ای
    /// </summary>
    [Description("خدمات مشاوره‌ای")]
    ConsultingService = 5
}
