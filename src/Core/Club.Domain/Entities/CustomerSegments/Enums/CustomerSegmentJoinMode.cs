namespace Hyper.Domain.Entities.CustomerSegments.Enums;

/// <summary>
/// نحوه عضویت در جامعه/بازار مشتریان
/// تعیین می‌کند که مشتری چگونه می‌تواند عضو جامعه/بازار شود
/// </summary>
public enum CustomerSegmentJoinMode
{
    /// <summary>
    /// عضویت سیستمی - فقط سامانه می‌تواند مشتری را عضو کند
    /// کاربر نمی‌تواند خودش درخواست عضویت بدهد
    /// عضویت فقط بر اساس رویدادها و شرایط خودکار انجام می‌شود
    /// </summary>
    [Description("عضویت سیستمی - فقط سامانه تشخیص عضویت می‌دهد")]
    SystemOnly = 1,

    /// <summary>
    /// عضویت با کنترل شرایط - کاربر می‌تواند درخواست دهد
    /// اما باید شرایط جامعه‌سازی را احراز کند
    /// شرایط CustomerSegmentKindCondition بررسی می‌شوند
    /// </summary>
    [Description("عضویت با کنترل شرایط - کاربر می‌تواند با احراز شرایط عضو شود")]
    WithConditionCheck = 2,

    /// <summary>
    /// عضویت آزاد - کاربر می‌تواند بدون کنترل شرایط عضو شود
    /// بدون بررسی CustomerSegmentKindCondition
    /// مناسب برای جامعه‌ها/بازارهای عمومی و باز
    /// </summary>
    [Description("عضویت آزاد - کاربر می‌تواند بدون کنترل شرایط عضو شود")]
    WithoutConditionCheck = 3
}
