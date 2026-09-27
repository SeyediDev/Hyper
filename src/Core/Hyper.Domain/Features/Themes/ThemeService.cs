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
    /// ایجاد تم سیموتک - سبز لیمویی با پس‌زمینه روشن
    /// هارمونی: Analogous با زرد-سبز
    /// </summary>
    private Theme CreateSimotekTheme(ThemePreference preference)
    {

        var theme = new Theme
        {
            Preference = preference,
            Name = "سیموتک",
            // Primary Colors - Lime Green
            Primary = "#456f0d",
            PrimaryHover = "#3f6212",
            PrimaryActive = "#365314",
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
            // Opaque light surfaces; color identity stays in accents and subtle tints.
            BackgroundPrimary = "#ffffff",
            BackgroundSecondary = "#f7faef",
            BackgroundTertiary = "#edf5db",
            BackgroundCard = "#ffffff",
            BackgroundHover = "rgba(194, 239, 3, 0.1)",
            BackgroundActive = "rgba(194, 239, 3, 0.2)",
            BackgroundLight = "rgba(194, 239, 3, 0.05)",
            // Text Colors - High contrast for readability
            TextPrimary = "#0f172a",
            TextSecondary = "#334155",
            TextMuted = "#475569",
            TextOnDark = "#ffffff",
            // Border Colors
            BorderLight = "#e2e8f0",
            BorderMedium = "#cbd5e1",
            BorderDark = "#94a3b8",
            HeaderBackground = "#ffffff",
            HeaderText = "#0f172a",
            HeaderUserBackground = "#edf5db",
            HeaderUserText = "#456f0d",
            DashboardTabBackground = "#edf5db",
            DashboardTabText = "#334155",
            DashboardWidgetBackground = "#ffffff",
            DashboardWidgetBorder = "#cbd5e1",
            FormBackground = "#ffffff",
            FormInputBackground = "#ffffff",
            FormInputText = "#0f172a",
            FormInputBorder = "#94a3b8",
            FormInputBorderFocus = "#456f0d",
            FormButtonBackground = "#456f0d",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: false);
    }

    /// <summary>
    /// ایجاد تم بنفش - بنفش روشن با پس‌زمینه روشن
    /// هارمونی: Monochromatic با صورتی
    /// </summary>
    private Theme CreatePurpleTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "بنفش",
            Primary = "#7e22ce",
            PrimaryHover = "#6b21a8",
            PrimaryActive = "#581c87",
            PrimaryLight = "rgba(168, 85, 247, 0.15)",
            Secondary = "#e879f9",
            SecondaryHover = "#f0abfc",
            SecondaryActive = "#d946ef",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#3b82f6",
            BackgroundPrimary = "#ffffff",
            BackgroundSecondary = "#faf7fd",
            BackgroundTertiary = "#f3e8ff",
            BackgroundCard = "#ffffff",
            BackgroundHover = "rgba(168, 85, 247, 0.1)",
            BackgroundActive = "rgba(168, 85, 247, 0.2)",
            BackgroundLight = "rgba(168, 85, 247, 0.05)",
            TextPrimary = "#0f172a",
            TextSecondary = "#334155",
            TextMuted = "#475569",
            TextOnDark = "#ffffff",
            BorderLight = "#e2e8f0",
            BorderMedium = "#cbd5e1",
            BorderDark = "#94a3b8",
            HeaderBackground = "#ffffff",
            HeaderText = "#0f172a",
            HeaderUserBackground = "#f3e8ff",
            HeaderUserText = "#7e22ce",
            DashboardTabBackground = "#f3e8ff",
            DashboardTabText = "#334155",
            DashboardWidgetBackground = "#ffffff",
            DashboardWidgetBorder = "#cbd5e1",
            FormBackground = "#ffffff",
            FormInputBackground = "#ffffff",
            FormInputText = "#0f172a",
            FormInputBorder = "#94a3b8",
            FormInputBorderFocus = "#7e22ce",
            FormButtonBackground = "#7e22ce",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: false);
    }

    /// <summary>
    /// ایجاد تم نارنجی - نارنجی روشن با پس‌زمینه روشن
    /// هارمونی: Warm Analogous با قرمز-نارنجی
    /// </summary>
    private Theme CreateOrangeTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "نارنجی",
            Primary = "#c2410c",
            PrimaryHover = "#9a3412",
            PrimaryActive = "#7c2d12",
            PrimaryLight = "rgba(249, 115, 22, 0.15)",
            Secondary = "#f43f5e",
            SecondaryHover = "#fb7185",
            SecondaryActive = "#e11d48",
            Success = "#22c55e",
            Warning = "#eab308",
            Danger = "#dc2626",
            Info = "#3b82f6",
            BackgroundPrimary = "#ffffff",
            BackgroundSecondary = "#fff9f5",
            BackgroundTertiary = "#ffedd5",
            BackgroundCard = "#ffffff",
            BackgroundHover = "rgba(249, 115, 22, 0.1)",
            BackgroundActive = "rgba(249, 115, 22, 0.2)",
            BackgroundLight = "rgba(249, 115, 22, 0.05)",
            TextPrimary = "#0f172a",
            TextSecondary = "#334155",
            TextMuted = "#475569",
            TextOnDark = "#ffffff",
            BorderLight = "#e2e8f0",
            BorderMedium = "#cbd5e1",
            BorderDark = "#94a3b8",
            HeaderBackground = "#ffffff",
            HeaderText = "#0f172a",
            HeaderUserBackground = "#ffedd5",
            HeaderUserText = "#c2410c",
            DashboardTabBackground = "#ffedd5",
            DashboardTabText = "#334155",
            DashboardWidgetBackground = "#ffffff",
            DashboardWidgetBorder = "#cbd5e1",
            FormBackground = "#ffffff",
            FormInputBackground = "#ffffff",
            FormInputText = "#0f172a",
            FormInputBorder = "#94a3b8",
            FormInputBorderFocus = "#c2410c",
            FormButtonBackground = "#c2410c",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: false);
    }

    /// <summary>
    /// ایجاد تم قرمز - قرمز روشن با پس‌زمینه روشن
    /// هارمونی: Complementary with cyan accents
    /// </summary>
    private Theme CreateRedTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "قرمز",
            Primary = "#b91c1c",
            PrimaryHover = "#991b1b",
            PrimaryActive = "#7f1d1d",
            PrimaryLight = "rgba(239, 68, 68, 0.15)",
            Secondary = "#06b6d4",
            SecondaryHover = "#22d3ee",
            SecondaryActive = "#0891b2",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#dc2626",
            Info = "#06b6d4",
            BackgroundPrimary = "#ffffff",
            BackgroundSecondary = "#fff7f7",
            BackgroundTertiary = "#fee2e2",
            BackgroundCard = "#ffffff",
            BackgroundHover = "rgba(239, 68, 68, 0.1)",
            BackgroundActive = "rgba(239, 68, 68, 0.2)",
            BackgroundLight = "rgba(239, 68, 68, 0.05)",
            TextPrimary = "#0f172a",
            TextSecondary = "#334155",
            TextMuted = "#475569",
            TextOnDark = "#ffffff",
            BorderLight = "#e2e8f0",
            BorderMedium = "#cbd5e1",
            BorderDark = "#94a3b8",
            HeaderBackground = "#ffffff",
            HeaderText = "#0f172a",
            HeaderUserBackground = "#fee2e2",
            HeaderUserText = "#b91c1c",
            DashboardTabBackground = "#fee2e2",
            DashboardTabText = "#334155",
            DashboardWidgetBackground = "#ffffff",
            DashboardWidgetBorder = "#cbd5e1",
            FormBackground = "#ffffff",
            FormInputBackground = "#ffffff",
            FormInputText = "#0f172a",
            FormInputBorder = "#94a3b8",
            FormInputBorderFocus = "#b91c1c",
            FormButtonBackground = "#b91c1c",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: false);
    }

    /// <summary>
    /// ایجاد تم فیروزه‌ای - فیروزه‌ای با پس‌زمینه روشن
    /// هارمونی: Analogous with blue-green
    /// </summary>
    private Theme CreateTealTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "فیروزه‌ای",
            Primary = "#0f766e",
            PrimaryHover = "#115e59",
            PrimaryActive = "#134e4a",
            PrimaryLight = "rgba(20, 184, 166, 0.15)",
            Secondary = "#0ea5e9",
            SecondaryHover = "#38bdf8",
            SecondaryActive = "#0284c7",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#0ea5e9",
            BackgroundPrimary = "#ffffff",
            BackgroundSecondary = "#f3faf9",
            BackgroundTertiary = "#dff4f0",
            BackgroundCard = "#ffffff",
            BackgroundHover = "rgba(20, 184, 166, 0.1)",
            BackgroundActive = "rgba(20, 184, 166, 0.2)",
            BackgroundLight = "rgba(20, 184, 166, 0.05)",
            TextPrimary = "#0f172a",
            TextSecondary = "#334155",
            TextMuted = "#475569",
            TextOnDark = "#ffffff",
            BorderLight = "#e2e8f0",
            BorderMedium = "#cbd5e1",
            BorderDark = "#94a3b8",
            HeaderBackground = "#ffffff",
            HeaderText = "#0f172a",
            HeaderUserBackground = "#dff4f0",
            HeaderUserText = "#0f766e",
            DashboardTabBackground = "#dff4f0",
            DashboardTabText = "#334155",
            DashboardWidgetBackground = "#ffffff",
            DashboardWidgetBorder = "#cbd5e1",
            FormBackground = "#ffffff",
            FormInputBackground = "#ffffff",
            FormInputText = "#0f172a",
            FormInputBorder = "#94a3b8",
            FormInputBorderFocus = "#0f766e",
            FormButtonBackground = "#0f766e",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: false);
    }

    /// <summary>
    /// ایجاد تم نیلی - نیلی روشن با پس‌زمینه روشن
    /// هارمونی: Monochromatic with purple
    /// </summary>
    private Theme CreateIndigoTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "نیلی",
            Primary = "#4338ca",
            PrimaryHover = "#3730a3",
            PrimaryActive = "#312e81",
            PrimaryLight = "rgba(99, 102, 241, 0.15)",
            Secondary = "#8b5cf6",
            SecondaryHover = "#a78bfa",
            SecondaryActive = "#7c3aed",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#3b82f6",
            BackgroundPrimary = "#ffffff",
            BackgroundSecondary = "#f7f8fe",
            BackgroundTertiary = "#e9eafe",
            BackgroundCard = "#ffffff",
            BackgroundHover = "rgba(99, 102, 241, 0.1)",
            BackgroundActive = "rgba(99, 102, 241, 0.2)",
            BackgroundLight = "rgba(99, 102, 241, 0.05)",
            TextPrimary = "#0f172a",
            TextSecondary = "#334155",
            TextMuted = "#475569",
            TextOnDark = "#ffffff",
            BorderLight = "#e2e8f0",
            BorderMedium = "#cbd5e1",
            BorderDark = "#94a3b8",
            HeaderBackground = "#ffffff",
            HeaderText = "#0f172a",
            HeaderUserBackground = "#e9eafe",
            HeaderUserText = "#4338ca",
            DashboardTabBackground = "#e9eafe",
            DashboardTabText = "#334155",
            DashboardWidgetBackground = "#ffffff",
            DashboardWidgetBorder = "#cbd5e1",
            FormBackground = "#ffffff",
            FormInputBackground = "#ffffff",
            FormInputText = "#0f172a",
            FormInputBorder = "#94a3b8",
            FormInputBorderFocus = "#4338ca",
            FormButtonBackground = "#4338ca",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: false);
    }

    /// <summary>
    /// ایجاد تم زمردی - زمردی روشن با پس‌زمینه روشن
    /// هارمونی: Monochromatic with green
    /// </summary>
    private Theme CreateEmeraldTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "زمردی",
            Primary = "#047857",
            PrimaryHover = "#065f46",
            PrimaryActive = "#064e3b",
            PrimaryLight = "rgba(16, 185, 129, 0.15)",
            Secondary = "#84cc16",
            SecondaryHover = "#a3e635",
            SecondaryActive = "#65a30d",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#3b82f6",
            BackgroundPrimary = "#ffffff",
            BackgroundSecondary = "#f3faf6",
            BackgroundTertiary = "#e0f2e9",
            BackgroundCard = "#ffffff",
            BackgroundHover = "rgba(16, 185, 129, 0.1)",
            BackgroundActive = "rgba(16, 185, 129, 0.2)",
            BackgroundLight = "rgba(16, 185, 129, 0.05)",
            TextPrimary = "#0f172a",
            TextSecondary = "#334155",
            TextMuted = "#475569",
            TextOnDark = "#ffffff",
            BorderLight = "#e2e8f0",
            BorderMedium = "#cbd5e1",
            BorderDark = "#94a3b8",
            HeaderBackground = "#ffffff",
            HeaderText = "#0f172a",
            HeaderUserBackground = "#e0f2e9",
            HeaderUserText = "#047857",
            DashboardTabBackground = "#e0f2e9",
            DashboardTabText = "#334155",
            DashboardWidgetBackground = "#ffffff",
            DashboardWidgetBorder = "#cbd5e1",
            FormBackground = "#ffffff",
            FormInputBackground = "#ffffff",
            FormInputText = "#0f172a",
            FormInputBorder = "#94a3b8",
            FormInputBorderFocus = "#047857",
            FormButtonBackground = "#047857",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: false);
    }

    /// <summary>
    /// ایجاد تم آبی آسمانی - آبی روشن با پس‌زمینه روشن
    /// هارمونی: Analogous with cyan-blue
    /// </summary>
    private Theme CreateCyanTheme(ThemePreference preference)
    {
        var theme = new Theme
        {
            Preference = preference,
            Name = "آبی آسمانی",
            Primary = "#0e7490",
            PrimaryHover = "#155e75",
            PrimaryActive = "#164e63",
            PrimaryLight = "rgba(6, 182, 212, 0.15)",
            Secondary = "#3b82f6",
            SecondaryHover = "#60a5fa",
            SecondaryActive = "#2563eb",
            Success = "#22c55e",
            Warning = "#f59e0b",
            Danger = "#ef4444",
            Info = "#3b82f6",
            BackgroundPrimary = "#ffffff",
            BackgroundSecondary = "#f3fafc",
            BackgroundTertiary = "#e0f2fe",
            BackgroundCard = "#ffffff",
            BackgroundHover = "rgba(6, 182, 212, 0.1)",
            BackgroundActive = "rgba(6, 182, 212, 0.2)",
            BackgroundLight = "rgba(6, 182, 212, 0.05)",
            TextPrimary = "#0f172a",
            TextSecondary = "#334155",
            TextMuted = "#475569",
            TextOnDark = "#ffffff",
            BorderLight = "#e2e8f0",
            BorderMedium = "#cbd5e1",
            BorderDark = "#94a3b8",
            HeaderBackground = "#ffffff",
            HeaderText = "#0f172a",
            HeaderUserBackground = "#e0f2fe",
            HeaderUserText = "#0e7490",
            DashboardTabBackground = "#e0f2fe",
            DashboardTabText = "#334155",
            DashboardWidgetBackground = "#ffffff",
            DashboardWidgetBorder = "#cbd5e1",
            FormBackground = "#ffffff",
            FormInputBackground = "#ffffff",
            FormInputText = "#0f172a",
            FormInputBorder = "#94a3b8",
            FormInputBorderFocus = "#0e7490",
            FormButtonBackground = "#0e7490",
            FormButtonText = "#ffffff"
        };

        return CompleteThemeWithChartAndReport(theme, isDarkBackground: false);
    }

    #endregion
}
