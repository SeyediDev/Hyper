namespace Hyper.Domain.Features.Themes;

/// <summary>
/// رابط سرویس مدیریت تم رنگی
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// دریافت تم بر اساس ThemePreference
    /// </summary>
    Theme GetTheme(ThemePreference themePreference);

    /// <summary>
    /// دریافت تمام تم‌های موجود
    /// </summary>
    IReadOnlyList<Theme> GetAllThemes();
}

/// <summary>
/// سرویس مدیریت تم رنگی
/// </summary>
public class ThemeService : IThemeService
{
    /// <summary>
    /// دریافت تم بر اساس ThemePreference
    /// </summary>
    public Theme GetTheme(ThemePreference themePreference)
    {
        return themePreference switch
        {
            ThemePreference.Simotek => CreateSimotekTheme(themePreference),
            ThemePreference.Purple => CreatePurpleTheme(themePreference),
            ThemePreference.Orange => CreateOrangeTheme(themePreference),
            ThemePreference.Red => CreateRedTheme(themePreference),
            ThemePreference.Teal => CreateTealTheme(themePreference),
            ThemePreference.Indigo => CreateIndigoTheme(themePreference),
            ThemePreference.Emerald => CreateEmeraldTheme(themePreference),
            ThemePreference.Cyan => CreateCyanTheme(themePreference),
            _ => CreateSimotekTheme(ThemePreference.Simotek) // Default fallback
        };
    }

    /// <summary>
    /// دریافت تمام تم‌های موجود
    /// </summary>
    public IReadOnlyList<Theme> GetAllThemes()
    {
        return Enum.GetValues<ThemePreference>()
            .Select(GetTheme)
            .ToList()
            .AsReadOnly();
    }

    #region Theme Creators

    /// <summary>
    /// ایجاد رنگ‌های متن بر اساس پس‌زمینه - Material Design
    /// </summary>
    private (string TextPrimary, string TextSecondary, string TextMuted, string TextOnDark) GetTextColors(bool isDarkBackground)
    {
        if (isDarkBackground)
        {
            return (
                TextPrimary: "#ffffff",       // سفید خالص برای متن اصلی - خوانایی 100%
                TextSecondary: "#e2e8f0",     // خاکستری روشن برای متن ثانویه - خوانایی 87%
                TextMuted: "#94a3b8",         // خاکستری متوسط برای متن خاموش - خوانایی 60%
                TextOnDark: "#ffffff"         // سفید برای متن روی پس‌زمینه تیره
            );
        }
        else
        {
            return (
                TextPrimary: "#0f172a",       // خاکستری بسیار تیره برای متن اصلی - خوانایی 100%
                TextSecondary: "#334155",     // خاکستری تیره برای متن ثانویه - خوانایی 87%
                TextMuted: "#64748b",         // خاکستری متوسط برای متن خاموش - خوانایی 60%
                TextOnDark: "#ffffff"         // سفید برای متن روی پس‌زمینه تیره
            );
        }
    }

    /// <summary>
    /// ایجاد رنگ‌های حاشیه بر اساس پس‌زمینه
    /// </summary>
    private (string BorderLight, string BorderMedium, string BorderDark) GetBorderColors(bool isDarkBackground)
    {
        if (isDarkBackground)
        {
            return (
                BorderLight: "rgba(255, 255, 255, 0.1)",   // سفید با شفافیت کم
                BorderMedium: "rgba(255, 255, 255, 0.2)", // سفید با شفافیت متوسط
                BorderDark: "rgba(255, 255, 255, 0.3)"     // سفید با شفافیت زیاد
            );
        }
        else
        {
            return (
                BorderLight: "#e2e8f0",    // خاکستری خیلی روشن
                BorderMedium: "#cbd5e1",   // خاکستری روشن
                BorderDark: "#94a3b8"      // خاکستری متوسط
            );
        }
    }

    /// <summary>
    /// ایجاد رنگ‌های چارت بر اساس پس‌زمینه - Material Design
    /// </summary>
    private (string ChartText, string ChartTextSecondary, string ChartBackground, string ChartPlotBackground, string ChartGridLine, string ChartAxisLine) GetChartColors(bool isDarkBackground)
    {
        if (isDarkBackground)
        {
            return (
                ChartText: "#ffffff",                       // سفید برای متن اصلی چارت
                ChartTextSecondary: "#cbd5e1",             // خاکستری روشن برای متن ثانویه
                ChartBackground: "#1a1f2e",                // پس‌زمینه تیره هماهنگ با BackgroundPrimary
                ChartPlotBackground: "#0f1419",             // پس‌زمینه plot area (تیره‌تر)
                ChartGridLine: "rgba(255, 255, 255, 0.08)", // خطوط grid با شفافیت کم
                ChartAxisLine: "rgba(255, 255, 255, 0.15)"  // خطوط axis با شفافیت بیشتر
            );
        }
        else
        {
            return (
                ChartText: "#0f172a",                      // خاکستری تیره برای متن اصلی
                ChartTextSecondary: "#475569",            // خاکستری متوسط برای متن ثانویه
                ChartBackground: "#ffffff",               // سفید برای پس‌زمینه
                ChartPlotBackground: "#f8fafc",            // خاکستری خیلی روشن برای plot area
                ChartGridLine: "#e2e8f0",                  // خاکستری روشن برای grid
                ChartAxisLine: "#cbd5e1"                   // خاکستری متوسط برای axis
            );
        }
    }

    /// <summary>
    /// ایجاد رنگ‌های گزارش بر اساس پس‌زمینه - Material Design
    /// </summary>
    private (string ReportText, string ReportTextSecondary, string ReportBackground, string ReportHeaderBackground, string ReportTableBorder, string ReportTableRowAlternate) GetReportColors(bool isDarkBackground)
    {
        if (isDarkBackground)
        {
            return (
                ReportText: "#ffffff",                       // سفید برای متن اصلی گزارش
                ReportTextSecondary: "#cbd5e1",             // خاکستری روشن برای متن ثانویه
                ReportBackground: "#1a1f2e",                // پس‌زمینه تیره هماهنگ با BackgroundPrimary
                ReportHeaderBackground: "#232838",          // پس‌زمینه header (تیره‌تر)
                ReportTableBorder: "rgba(255, 255, 255, 0.08)", // border جدول با شفافیت کم
                ReportTableRowAlternate: "rgba(255, 255, 255, 0.03)" // alternate row background
            );
        }
        else
        {
            return (
                ReportText: "#0f172a",                      // خاکستری تیره برای متن اصلی
                ReportTextSecondary: "#475569",            // خاکستری متوسط برای متن ثانویه
                ReportBackground: "#ffffff",               // سفید برای پس‌زمینه
                ReportHeaderBackground: "#f8fafc",        // خاکستری خیلی روشن برای header
                ReportTableBorder: "#e2e8f0",              // border جدول
                ReportTableRowAlternate: "#f8fafc"        // alternate row background
            );
        }
    }

    /// <summary>
    /// تکمیل تم با رنگ‌های چارت و گزارش و المان‌های Material Design
    /// </summary>
    private Theme CompleteThemeWithChartAndReport(Theme theme, bool isDarkBackground = true)
    {
        var chartColors = GetChartColors(isDarkBackground);
        var reportColors = GetReportColors(isDarkBackground);
        var shadows = GetMaterialDesignShadows();

        // Chart Colors
        theme.ChartText = chartColors.ChartText;
        theme.ChartTextSecondary = chartColors.ChartTextSecondary;
        theme.ChartBackground = chartColors.ChartBackground;
        theme.ChartPlotBackground = chartColors.ChartPlotBackground;
        theme.ChartGridLine = chartColors.ChartGridLine;
        theme.ChartAxisLine = chartColors.ChartAxisLine;

        // Report Colors
        theme.ReportText = reportColors.ReportText;
        theme.ReportTextSecondary = reportColors.ReportTextSecondary;
        theme.ReportBackground = reportColors.ReportBackground;
        theme.ReportHeaderBackground = reportColors.ReportHeaderBackground;
        theme.ReportTableBorder = reportColors.ReportTableBorder;
        theme.ReportTableRowAlternate = reportColors.ReportTableRowAlternate;

        // Material Design Shadows
        theme.Shadow1 = shadows.Shadow1;
        theme.Shadow2 = shadows.Shadow2;
        theme.Shadow3 = shadows.Shadow3;
        theme.Shadow4 = shadows.Shadow4;

        // Dashboard Widget Title - تضمین کنتراست
        theme.DashboardWidgetTitleText = isDarkBackground ? "#ffffff" : "#1e293b";

        return theme;
    }

    /// <summary>
    /// ایجاد سایه‌های Material Design
    /// </summary>
    private (string Shadow1, string Shadow2, string Shadow3, string Shadow4) GetMaterialDesignShadows()
    {
        return (
            Shadow1: "0 1px 3px rgba(0,0,0,0.12), 0 1px 2px rgba(0,0,0,0.24)",
            Shadow2: "0 3px 6px rgba(0,0,0,0.15), 0 2px 4px rgba(0,0,0,0.12)",
            Shadow3: "0 10px 20px rgba(0,0,0,0.15), 0 3px 6px rgba(0,0,0,0.10)",
            Shadow4: "0 15px 25px rgba(0,0,0,0.15), 0 5px 10px rgba(0,0,0,0.05)"
        );
    }

    /// <summary>
    /// ایجاد تم سیموتک - سبز لیمویی با پس‌زمینه تیره
    /// هارمونی: Analogous با زرد-سبز
    /// </summary>
    private Theme CreateSimotekTheme(ThemePreference preference)
    {
        var textColors = GetTextColors(isDarkBackground: true);
        var borderColors = GetBorderColors(isDarkBackground: true);

        var theme = new Theme
        {
            Preference = preference,
            Name = "سیموتک",
            // Primary Colors - Lime Green
            Primary = "#c2ef03",
            PrimaryHover = "#d8f54a",
            PrimaryActive = "#b8d900",
            PrimaryLight = "rgba(194, 239, 3, 0.15)",
            // Secondary Colors - Analogous Yellow
            Secondary = "#eab308",
            SecondaryHover = "#facc15",
            SecondaryActive = "#ca8a04",
            // Semantic Colors
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#3b82f6",
            // Background Colors - Material Design Dark
            BackgroundPrimary = "#1a1f2e",      // Very dark blue-gray
            BackgroundSecondary = "#0f1419",    // Even darker
            BackgroundTertiary = "#505563",     // Very subtle white for minimal design
            BackgroundCard = "#f7f3f330",         // Card background
            BackgroundHover = "rgba(194, 239, 3, 0.1)",
            BackgroundActive = "rgba(194, 239, 3, 0.2)",
            BackgroundLight = "rgba(194, 239, 3, 0.05)",
            // Text Colors - High contrast for readability
            TextPrimary = "#ffffff",
            TextSecondary = "#cbd5e1",
            TextMuted = "#64748b",
            TextOnDark = "#ffffff",
            // Border Colors
            BorderLight = "rgba(255, 255, 255, 0.1)",
            BorderMedium = "rgba(255, 255, 255, 0.2)",
            BorderDark = "rgba(255, 255, 255, 0.3)",
            // Header & Navigation - Matching theme
            HeaderBackground = "#0f1419",
            HeaderText = "#ffffff",
            HeaderUserBackground = "#1a1f2e",
            HeaderUserText = "#c2ef03",
            // Dashboard - Tabs darker than widgets
            DashboardTabBackground = "#0f1419",      // Darker
            DashboardTabText = "#ffffff",
            DashboardWidgetBackground = "#232838",   // Lighter than tabs
            DashboardWidgetBorder = "rgba(194, 239, 3, 0.3)",
            // Forms - Material Design
            FormBackground = "#1a1f2e",
            FormInputBackground = "#0f1419",
            FormInputText = "#ffffff",
            FormInputBorder = "rgba(255, 255, 255, 0.2)",
            FormInputBorderFocus = "#c2ef03",
            FormButtonBackground = "#c2ef03",
            FormButtonText = "#0f1419"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: true);
    }

    /// <summary>
    /// ایجاد تم بنفش - بنفش روشن با پس‌زمینه تیره
    /// هارمونی: Monochromatic با صورتی
    /// </summary>
    private Theme CreatePurpleTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "بنفش",
            Primary = "#a855f7",
            PrimaryHover = "#c084fc",
            PrimaryActive = "#9333ea",
            PrimaryLight = "rgba(168, 85, 247, 0.15)",
            Secondary = "#e879f9",
            SecondaryHover = "#f0abfc",
            SecondaryActive = "#d946ef",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#3b82f6",
            BackgroundPrimary = "#2d1b4e",
            BackgroundSecondary = "#1a1033",
            BackgroundTertiary = "#ebd4fa",
            BackgroundCard = "#f9f9fa30",
            BackgroundHover = "rgba(168, 85, 247, 0.1)",
            BackgroundActive = "rgba(168, 85, 247, 0.2)",
            BackgroundLight = "rgba(168, 85, 247, 0.05)",
            TextPrimary = "#ffffff",
            TextSecondary = "#e9d5ff",
            TextMuted = "#a855f7",
            TextOnDark = "#ffffff",
            BorderLight = "rgba(233, 213, 255, 0.1)",
            BorderMedium = "rgba(233, 213, 255, 0.2)",
            BorderDark = "rgba(233, 213, 255, 0.3)",
            HeaderBackground = "#1a1033",
            HeaderText = "#ffffff",
            HeaderUserBackground = "#2d1b4e",
            HeaderUserText = "#e879f9",
            DashboardTabBackground = "#1a1033",
            DashboardTabText = "#ffffff",
            DashboardWidgetBackground = "#3d2169",
            DashboardWidgetBorder = "rgba(168, 85, 247, 0.3)",
            FormBackground = "#2d1b4e",
            FormInputBackground = "#1a1033",
            FormInputText = "#ffffff",
            FormInputBorder = "rgba(233, 213, 255, 0.2)",
            FormInputBorderFocus = "#a855f7",
            FormButtonBackground = "#a855f7",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: true);
    }

    /// <summary>
    /// ایجاد تم نارنجی - نارنجی روشن با پس‌زمینه تیره
    /// هارمونی: Warm Analogous با قرمز-نارنجی
    /// </summary>
    private Theme CreateOrangeTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "نارنجی",
            Primary = "#f97316",
            PrimaryHover = "#fb923c",
            PrimaryActive = "#ea580c",
            PrimaryLight = "rgba(249, 115, 22, 0.15)",
            Secondary = "#f43f5e",
            SecondaryHover = "#fb7185",
            SecondaryActive = "#e11d48",
            Success = "#22c55e",
            Warning = "#eab308",
            Danger = "#dc2626",
            Info = "#3b82f6",
            BackgroundPrimary = "#2d1a0f",
            BackgroundSecondary = "#1a0f08",
            BackgroundTertiary = "#ffdec3",
            BackgroundCard = "#f8ebe030",
            BackgroundHover = "rgba(249, 115, 22, 0.1)",
            BackgroundActive = "rgba(249, 115, 22, 0.2)",
            BackgroundLight = "rgba(249, 115, 22, 0.05)",
            TextPrimary = "#ffffff",
            TextSecondary = "#fed7aa",
            TextMuted = "#f97316",
            TextOnDark = "#ffffff",
            BorderLight = "rgba(254, 215, 170, 0.1)",
            BorderMedium = "rgba(254, 215, 170, 0.2)",
            BorderDark = "rgba(254, 215, 170, 0.3)",
            HeaderBackground = "#1a0f08",
            HeaderText = "#ffffff",
            HeaderUserBackground = "#2d1a0f",
            HeaderUserText = "#fb923c",
            DashboardTabBackground = "#1a0f08",
            DashboardTabText = "#ffffff",
            DashboardWidgetBackground = "#3d2214",
            DashboardWidgetBorder = "rgba(249, 115, 22, 0.3)",
            FormBackground = "#2d1a0f",
            FormInputBackground = "#1a0f08",
            FormInputText = "#ffffff",
            FormInputBorder = "rgba(254, 215, 170, 0.2)",
            FormInputBorderFocus = "#f97316",
            FormButtonBackground = "#f97316",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: true);
    }

    /// <summary>
    /// ایجاد تم قرمز - قرمز روشن با پس‌زمینه تیره
    /// هارمونی: Complementary with cyan accents
    /// </summary>
    private Theme CreateRedTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "قرمز",
            Primary = "#ef4444",
            PrimaryHover = "#f87171",
            PrimaryActive = "#dc2626",
            PrimaryLight = "rgba(239, 68, 68, 0.15)",
            Secondary = "#06b6d4",
            SecondaryHover = "#22d3ee",
            SecondaryActive = "#0891b2",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#dc2626",
            Info = "#06b6d4",
            BackgroundPrimary = "#2d0f0f",
            BackgroundSecondary = "#1a0808",
            BackgroundTertiary = "#fcb7b7",
            BackgroundCard = "#f8e2e230",
            BackgroundHover = "rgba(239, 68, 68, 0.1)",
            BackgroundActive = "rgba(239, 68, 68, 0.2)",
            BackgroundLight = "rgba(239, 68, 68, 0.05)",
            TextPrimary = "#ffffff",
            TextSecondary = "#fecaca",
            TextMuted = "#ef4444",
            TextOnDark = "#ffffff",
            BorderLight = "rgba(254, 202, 202, 0.1)",
            BorderMedium = "rgba(254, 202, 202, 0.2)",
            BorderDark = "rgba(254, 202, 202, 0.3)",
            HeaderBackground = "#1a0808",
            HeaderText = "#ffffff",
            HeaderUserBackground = "#2d0f0f",
            HeaderUserText = "#f87171",
            DashboardTabBackground = "#1a0808",
            DashboardTabText = "#ffffff",
            DashboardWidgetBackground = "#3d1515",
            DashboardWidgetBorder = "rgba(239, 68, 68, 0.3)",
            FormBackground = "#2d0f0f",
            FormInputBackground = "#1a0808",
            FormInputText = "#ffffff",
            FormInputBorder = "rgba(254, 202, 202, 0.2)",
            FormInputBorderFocus = "#ef4444",
            FormButtonBackground = "#ef4444",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: true);
    }

    /// <summary>
    /// ایجاد تم فیروزه‌ای - فیروزه‌ای با پس‌زمینه تیره
    /// هارمونی: Analogous with blue-green
    /// </summary>
    private Theme CreateTealTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "فیروزه‌ای",
            Primary = "#14b8a6",
            PrimaryHover = "#2dd4bf",
            PrimaryActive = "#0d9488",
            PrimaryLight = "rgba(20, 184, 166, 0.15)",
            Secondary = "#0ea5e9",
            SecondaryHover = "#38bdf8",
            SecondaryActive = "#0284c7",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#0ea5e9",
            BackgroundPrimary = "#0f2927",
            BackgroundSecondary = "#081a19",
            BackgroundTertiary = "#cbf1fd",
            BackgroundCard = "#e8faff30",
            BackgroundHover = "rgba(20, 184, 166, 0.1)",
            BackgroundActive = "rgba(20, 184, 166, 0.2)",
            BackgroundLight = "rgba(20, 184, 166, 0.05)",
            TextPrimary = "#ffffff",
            TextSecondary = "#99f6e4",
            TextMuted = "#14b8a6",
            TextOnDark = "#ffffff",
            BorderLight = "rgba(153, 246, 228, 0.1)",
            BorderMedium = "rgba(153, 246, 228, 0.2)",
            BorderDark = "rgba(153, 246, 228, 0.3)",
            HeaderBackground = "#081a19",
            HeaderText = "#ffffff",
            HeaderUserBackground = "#0f2927",
            HeaderUserText = "#2dd4bf",
            DashboardTabBackground = "#081a19",
            DashboardTabText = "#ffffff",
            DashboardWidgetBackground = "#153835",
            DashboardWidgetBorder = "rgba(20, 184, 166, 0.3)",
            FormBackground = "#0f2927",
            FormInputBackground = "#081a19",
            FormInputText = "#ffffff",
            FormInputBorder = "rgba(153, 246, 228, 0.2)",
            FormInputBorderFocus = "#14b8a6",
            FormButtonBackground = "#14b8a6",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: true);
    }

    /// <summary>
    /// ایجاد تم نیلی - نیلی روشن با پس‌زمینه تیره
    /// هارمونی: Monochromatic with purple
    /// </summary>
    private Theme CreateIndigoTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "نیلی",
            Primary = "#6366f1",
            PrimaryHover = "#818cf8",
            PrimaryActive = "#4f46e5",
            PrimaryLight = "rgba(99, 102, 241, 0.15)",
            Secondary = "#8b5cf6",
            SecondaryHover = "#a78bfa",
            SecondaryActive = "#7c3aed",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#3b82f6",
            BackgroundPrimary = "#1e1b3a",
            BackgroundSecondary = "#12102b",
            BackgroundTertiary = "#bddaf5",
            BackgroundCard = "#bddaf530",
            BackgroundHover = "rgba(99, 102, 241, 0.1)",
            BackgroundActive = "rgba(99, 102, 241, 0.2)",
            BackgroundLight = "rgba(99, 102, 241, 0.05)",
            TextPrimary = "#ffffff",
            TextSecondary = "#c7d2fe",
            TextMuted = "#6366f1",
            TextOnDark = "#ffffff",
            BorderLight = "rgba(199, 210, 254, 0.1)",
            BorderMedium = "rgba(199, 210, 254, 0.2)",
            BorderDark = "rgba(199, 210, 254, 0.3)",
            HeaderBackground = "#12102b",
            HeaderText = "#ffffff",
            HeaderUserBackground = "#1e1b3a",
            HeaderUserText = "#818cf8",
            DashboardTabBackground = "#12102b",
            DashboardTabText = "#ffffff",
            DashboardWidgetBackground = "#28234d",
            DashboardWidgetBorder = "rgba(99, 102, 241, 0.3)",
            FormBackground = "#1e1b3a",
            FormInputBackground = "#12102b",
            FormInputText = "#ffffff",
            FormInputBorder = "rgba(199, 210, 254, 0.2)",
            FormInputBorderFocus = "#6366f1",
            FormButtonBackground = "#6366f1",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: true);
    }

    /// <summary>
    /// ایجاد تم زمردی - زمردی روشن با پس‌زمینه تیره
    /// هارمونی: Monochromatic with green
    /// </summary>
    private Theme CreateEmeraldTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "زمردی",
            Primary = "#10b981",
            PrimaryHover = "#34d399",
            PrimaryActive = "#059669",
            PrimaryLight = "rgba(16, 185, 129, 0.15)",
            Secondary = "#84cc16",
            SecondaryHover = "#a3e635",
            SecondaryActive = "#65a30d",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#3b82f6",
            BackgroundPrimary = "#0f261e",
            BackgroundSecondary = "#081912",
            BackgroundTertiary = "#e3f3d7",
            BackgroundCard = "#dbffc230",
            BackgroundHover = "rgba(16, 185, 129, 0.1)",
            BackgroundActive = "rgba(16, 185, 129, 0.2)",
            BackgroundLight = "rgba(16, 185, 129, 0.05)",
            TextPrimary = "#ffffff",
            TextSecondary = "#a7f3d0",
            TextMuted = "#10b981",
            TextOnDark = "#ffffff",
            BorderLight = "rgba(167, 243, 208, 0.1)",
            BorderMedium = "rgba(167, 243, 208, 0.2)",
            BorderDark = "rgba(167, 243, 208, 0.3)",
            HeaderBackground = "#081912",
            HeaderText = "#ffffff",
            HeaderUserBackground = "#0f261e",
            HeaderUserText = "#34d399",
            DashboardTabBackground = "#081912",
            DashboardTabText = "#ffffff",
            DashboardWidgetBackground = "#15362b",
            DashboardWidgetBorder = "rgba(16, 185, 129, 0.3)",
            FormBackground = "#0f261e",
            FormInputBackground = "#081912",
            FormInputText = "#ffffff",
            FormInputBorder = "rgba(167, 243, 208, 0.2)",
            FormInputBorderFocus = "#10b981",
            FormButtonBackground = "#10b981",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: true);
    }

    /// <summary>
    /// ایجاد تم آبی آسمانی - آبی روشن با پس‌زمینه تیره
    /// هارمونی: Analogous with cyan-blue
    /// </summary>
    private Theme CreateCyanTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "آبی آسمانی",
            Primary = "#06b6d4",
            PrimaryHover = "#22d3ee",
            PrimaryActive = "#0891b2",
            PrimaryLight = "rgba(6, 182, 212, 0.15)",
            Secondary = "#3b82f6",
            SecondaryHover = "#60a5fa",
            SecondaryActive = "#2563eb",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#3b82f6",
            BackgroundPrimary = "#0d2832",
            BackgroundSecondary = "#081b22",
            BackgroundTertiary = "#c0e1fc",
            BackgroundCard = "#c0e1fc30",
            BackgroundHover = "rgba(6, 182, 212, 0.1)",
            BackgroundActive = "rgba(6, 182, 212, 0.2)",
            BackgroundLight = "rgba(6, 182, 212, 0.05)",
            TextPrimary = "#ffffff",
            TextSecondary = "#a5f3fc",
            TextMuted = "#06b6d4",
            TextOnDark = "#ffffff",
            BorderLight = "rgba(165, 243, 252, 0.1)",
            BorderMedium = "rgba(165, 243, 252, 0.2)",
            BorderDark = "rgba(165, 243, 252, 0.3)",
            HeaderBackground = "#081b22",
            HeaderText = "#ffffff",
            HeaderUserBackground = "#0d2832",
            HeaderUserText = "#22d3ee",
            DashboardTabBackground = "#081b22",
            DashboardTabText = "#ffffff",
            DashboardWidgetBackground = "#123843",
            DashboardWidgetBorder = "rgba(6, 182, 212, 0.3)",
            FormBackground = "#0d2832",
            FormInputBackground = "#081b22",
            FormInputText = "#ffffff",
            FormInputBorder = "rgba(165, 243, 252, 0.2)",
            FormInputBorderFocus = "#06b6d4",
            FormButtonBackground = "#06b6d4",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: true);
    }

    #endregion
}
