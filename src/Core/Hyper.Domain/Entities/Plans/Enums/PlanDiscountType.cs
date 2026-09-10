namespace Hyper.Domain.Entities.Plans.Enums;

/// <summary>
/// نوع تخفیف طرح
/// </summary>
public enum PlanDiscountType
{
    /// <summary>
    /// درصدی - تخفیف به صورت درصد از قیمت اصلی
    /// </summary>
    [Display(Name = "درصدی")]
    Percentage = 1,

    /// <summary>
    /// مقداری - تخفیف به صورت مقدار ثابت از قیمت
    /// </summary>
    [Display(Name = "مقداری")]
    FixedAmount = 2
}

