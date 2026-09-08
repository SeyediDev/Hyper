namespace Hyper.Domain.Entities.Promotions.Enums;

/// <summary>
/// مقایسه شرط با چه چیزی
/// </summary>
public enum PromotionConditionCompareWith
{
    [Description("امتیاز")]
    Point,
    
    [Description("پارامتر")]
    Parameter
}

