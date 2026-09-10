namespace Hyper.Domain.Features.Themes;

/// <summary>
/// کلاس تم رنگی - شامل تمام رنگ‌های مورد نیاز برای UI
/// </summary>
public class Theme
{
    /// <summary>
    /// نام تم (عنوان فارسی)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// نوع تم (از enum ThemePreference)
    /// </summary>
    public ThemePreference Preference { get; set; }

    // ===== Primary Colors =====
    /// <summary>
    /// رنگ اصلی
    /// </summary>
    public string Primary { get; set; } = string.Empty;

    /// <summary>
    /// رنگ اصلی در حالت hover
    /// </summary>
    public string PrimaryHover { get; set; } = string.Empty;

    /// <summary>
    /// رنگ اصلی در حالت active
    /// </summary>
    public string PrimaryActive { get; set; } = string.Empty;

    /// <summary>
    /// رنگ اصلی روشن (با شفافیت)
    /// </summary>
    public string PrimaryLight { get; set; } = string.Empty;

    // ===== Secondary Colors =====
    /// <summary>
    /// رنگ ثانویه
    /// </summary>
    public string Secondary { get; set; } = string.Empty;

    /// <summary>
    /// رنگ ثانویه در حالت hover
    /// </summary>
    public string SecondaryHover { get; set; } = string.Empty;

    /// <summary>
    /// رنگ ثانویه در حالت active
    /// </summary>
    public string SecondaryActive { get; set; } = string.Empty;

    // ===== Semantic Colors =====
    /// <summary>
    /// رنگ موفقیت (سبز)
    /// </summary>
    public string Success { get; set; } = string.Empty;

    /// <summary>
    /// رنگ هشدار (نارنجی/زرد)
    /// </summary>
    public string Warning { get; set; } = string.Empty;

    /// <summary>
    /// رنگ خطا (قرمز)
    /// </summary>
    public string Danger { get; set; } = string.Empty;

    /// <summary>
    /// رنگ اطلاعات (آبی)
    /// </summary>
    public string Info { get; set; } = string.Empty;

    // ===== Background Colors =====
    /// <summary>
    /// رنگ پس‌زمینه اصلی
    /// </summary>
    public string BackgroundPrimary { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه ثانویه
    /// </summary>
    public string BackgroundSecondary { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه سوم
    /// </summary>
    public string BackgroundTertiary { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه کارت
    /// </summary>
    public string BackgroundCard { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه در حالت hover
    /// </summary>
    public string BackgroundHover { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه در حالت active
    /// </summary>
    public string BackgroundActive { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه روشن (با شفافیت)
    /// </summary>
    public string BackgroundLight { get; set; } = string.Empty;

    // ===== Text Colors =====
    /// <summary>
    /// رنگ متن اصلی
    /// </summary>
    public string TextPrimary { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن ثانویه
    /// </summary>
    public string TextSecondary { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن خاموش (muted)
    /// </summary>
    public string TextMuted { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن روی پس‌زمینه تیره
    /// </summary>
    public string TextOnDark { get; set; } = string.Empty;

    // ===== Border Colors =====
    /// <summary>
    /// رنگ حاشیه روشن
    /// </summary>
    public string BorderLight { get; set; } = string.Empty;

    /// <summary>
    /// رنگ حاشیه متوسط
    /// </summary>
    public string BorderMedium { get; set; } = string.Empty;

    /// <summary>
    /// رنگ حاشیه تیره
    /// </summary>
    public string BorderDark { get; set; } = string.Empty;

    // ===== Chart Colors =====
    /// <summary>
    /// رنگ متن چارت (برای عنوان، label ها و legend)
    /// </summary>
    public string ChartText { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن چارت ثانویه (برای axis labels)
    /// </summary>
    public string ChartTextSecondary { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه چارت
    /// </summary>
    public string ChartBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه plot area چارت
    /// </summary>
    public string ChartPlotBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ خطوط grid چارت
    /// </summary>
    public string ChartGridLine { get; set; } = string.Empty;

    /// <summary>
    /// رنگ خطوط axis چارت
    /// </summary>
    public string ChartAxisLine { get; set; } = string.Empty;

    // ===== Report Colors =====
    /// <summary>
    /// رنگ متن گزارش
    /// </summary>
    public string ReportText { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن گزارش ثانویه
    /// </summary>
    public string ReportTextSecondary { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه گزارش
    /// </summary>
    public string ReportBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه header گزارش
    /// </summary>
    public string ReportHeaderBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ border جدول گزارش
    /// </summary>
    public string ReportTableBorder { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه row های جدول گزارش (alternate)
    /// </summary>
    public string ReportTableRowAlternate { get; set; } = string.Empty;

    // ===== Header & Navigation Colors =====
    /// <summary>
    /// رنگ پس‌زمینه هدر اصلی
    /// </summary>
    public string HeaderBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن هدر
    /// </summary>
    public string HeaderText { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه منوی کاربر در هدر
    /// </summary>
    public string HeaderUserBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن منوی کاربر در هدر
    /// </summary>
    public string HeaderUserText { get; set; } = string.Empty;

    // ===== Dashboard Colors =====
    /// <summary>
    /// رنگ پس‌زمینه تب‌های داشبورد (پررنگ‌تر از ویجت‌ها)
    /// </summary>
    public string DashboardTabBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن تب‌های داشبورد
    /// </summary>
    public string DashboardTabText { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه ویجت‌های داشبورد
    /// </summary>
    public string DashboardWidgetBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ حاشیه ویجت‌های داشبورد
    /// </summary>
    public string DashboardWidgetBorder { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن تیتر ویجت‌های داشبورد (تضمین کنتراست روی بک‌گراند ویجت)
    /// </summary>
    public string DashboardWidgetTitleText { get; set; } = string.Empty;

    // ===== Form Colors =====
    /// <summary>
    /// رنگ پس‌زمینه فرم‌ها
    /// </summary>
    public string FormBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه input‌ها
    /// </summary>
    public string FormInputBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن input‌ها
    /// </summary>
    public string FormInputText { get; set; } = string.Empty;

    /// <summary>
    /// رنگ حاشیه input‌ها
    /// </summary>
    public string FormInputBorder { get; set; } = string.Empty;

    /// <summary>
    /// رنگ حاشیه input‌ها در حالت focus
    /// </summary>
    public string FormInputBorderFocus { get; set; } = string.Empty;

    /// <summary>
    /// رنگ پس‌زمینه دکمه‌ها
    /// </summary>
    public string FormButtonBackground { get; set; } = string.Empty;

    /// <summary>
    /// رنگ متن دکمه‌ها
    /// </summary>
    public string FormButtonText { get; set; } = string.Empty;

    // ===== Material Design Elevation =====
    /// <summary>
    /// سایه سطح 1 (کمترین)
    /// </summary>
    public string Shadow1 { get; set; } = string.Empty;

    /// <summary>
    /// سایه سطح 2
    /// </summary>
    public string Shadow2 { get; set; } = string.Empty;

    /// <summary>
    /// سایه سطح 3
    /// </summary>
    public string Shadow3 { get; set; } = string.Empty;

    /// <summary>
    /// سایه سطح 4 (بیشترین)
    /// </summary>
    public string Shadow4 { get; set; } = string.Empty;
}