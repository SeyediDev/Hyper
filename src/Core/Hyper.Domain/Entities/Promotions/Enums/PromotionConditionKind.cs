namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// نوع شرط پویش - برای مقایسه و ارزیابی شرط
/// </summary>
public enum PromotionConditionKind
{
    [Description("بدون شرط اضافی")]
    WithoutExtraCondition = 0,
    
    [Description("فرمول شرط")]
    Formula = 1,
    
    [Description("برابر")]
    EqualTo = 2,
    
    [Description("نابرابر")]
    NotEqualTo = 3,
    
    [Description("بزرگتر")]
    GreaterThan = 4,
    
    [Description("کوچکتر")]
    LessThan = 5,
    
    [Description("بزرگتر مساوی")]
    GreaterThanOrEqualTo = 6,
    
    [Description("کوچکتر مساوی")]
    LessThanOrEqualTo = 7
}

