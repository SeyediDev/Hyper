namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// روش محاسبه مقدار عملیات
/// </summary>
public enum PromotionActionAmountMethod
{
    [Description("مقدار ثابت")]
    FixAmount,
    
    [Description("از یک پارامتر")]
    FromParameter,
    
    [Description("محاسبه فرمول")]
    FromFormula,
}

