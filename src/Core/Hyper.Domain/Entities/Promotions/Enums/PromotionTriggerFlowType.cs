namespace Hyper.Domain.Entities.Promotions.Enums;

public enum PromotionTriggerFlowType
{
    [Description("در ابتدا")]
    OnStart = 1,

    [Description("بعد از یک رویداد")]
    After = 2,

    [Description("موازی با یک رویداد")]
    Parallel = 3
}