using Hyper.Domain.Features.Themes;
using System.Text.Json;

var service = new ThemeService();
var themes = service.GetAllThemes();
var checks = 0;
void Check(bool condition, string message) { checks++; if (!condition) throw new Exception(message); }
static double Luminance(string hex) {
    var rgb = new[] { 1, 3, 5 }.Select(i => Convert.ToInt32(hex.Substring(i, 2), 16) / 255d)
        .Select(c => c <= .04045 ? c / 12.92 : Math.Pow((c + .055) / 1.055, 2.4)).ToArray();
    return .2126 * rgb[0] + .7152 * rgb[1] + .0722 * rgb[2];
}
void Contrast(string foreground, string background, string label) {
    var f = Luminance(foreground); var b = Luminance(background);
    var ratio = (Math.Max(f, b) + .05) / (Math.Min(f, b) + .05);
    Check(ratio >= 4.5, $"{label}: {ratio:F2}:1 contrast ({foreground} / {background})");
}
Check(themes.Count == Enum.GetValues<ThemePreference>().Length, "All persisted preferences covered");
Check(service.GetTheme((ThemePreference)999).Preference == ThemePreference.Simotek, "Unknown preference fallback");
Check(themes.Select(t => t.Primary).Distinct().Count() == themes.Count, "Color identities stay distinct");
foreach (var theme in themes) {
    foreach (var surface in new[] { theme.BackgroundPrimary, theme.BackgroundSecondary, theme.BackgroundTertiary,
        theme.BackgroundCard, theme.HeaderBackground, theme.DashboardTabBackground, theme.DashboardWidgetBackground,
        theme.FormBackground, theme.FormInputBackground, theme.ChartBackground, theme.ChartPlotBackground,
        theme.ReportBackground, theme.ReportHeaderBackground, theme.ReportTableRowAlternate }) {
        Check(surface.Length == 7 && surface[0] == '#', $"{theme.Name}: opaque surface {surface}");
        Check(Luminance(surface) >= .75, $"{theme.Name}: light surface {surface}");
        foreach (var text in new[] { theme.TextPrimary, theme.TextSecondary, theme.TextMuted })
            Contrast(text, surface, theme.Name + " content");
    }
    foreach (var pair in new[] {
        (theme.Primary, theme.BackgroundCard), (theme.Primary, theme.BackgroundSecondary),
        (theme.Primary, theme.BackgroundTertiary),
        (theme.FormButtonText, theme.FormButtonBackground), (theme.FormButtonText, theme.PrimaryHover),
        (theme.FormButtonText, theme.PrimaryActive), (theme.FormInputText, theme.FormInputBackground),
        (theme.HeaderText, theme.HeaderBackground), (theme.HeaderUserText, theme.HeaderUserBackground),
        (theme.DashboardTabText, theme.DashboardTabBackground),
        (theme.DashboardWidgetTitleText, theme.BackgroundTertiary),
        (theme.ChartText, theme.ChartBackground), (theme.ChartTextSecondary, theme.ChartPlotBackground),
        (theme.ReportText, theme.ReportBackground), (theme.ReportTextSecondary, theme.ReportHeaderBackground)
    }) Contrast(pair.Item1, pair.Item2, theme.Name);
}
Console.WriteLine($"PASS: {checks} palette checks across {themes.Count} themes.");
if (args.Length > 0) File.WriteAllText(args[0], JsonSerializer.Serialize(themes));
