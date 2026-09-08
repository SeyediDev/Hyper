using Hyper.Domain.Entities.Common;
using Hyper.Domain.Enums;
using Hyper.Domain.Features.Themes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;
using Neo.Common.Extensions;
using Neo.Domain.Entities.Common;

namespace Hyper.AdminPanel.Web.Controllers;

/// <summary>
/// Controller for managing user theme preferences
/// </summary>
[Authorize]
public class ThemeController(
	ICommandRepository<User, UserId> userRepository,
    IThemeService themeService,
    ILogger<ThemeController> logger) : ControllerBaseMVC
{
    /// <summary>
    /// Safely tries to get the session, returns null if session is not configured
    /// </summary>
    private ISession? GetSessionSafely()
    {
        try
        {
            return HttpContext.Session;
        }
        catch (InvalidOperationException ex)
        {
            // Session is not configured
            var message = ex.Message;
            return null;
        }
    }

    /// <summary>
    /// Get current user's theme preference
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTheme(CancellationToken cancellationToken)
    {
        try
        {
            var identityUser = GetUser();
            if (identityUser == null)
            {
                return Json(new { theme = "Simotek", success = false, message = "User not found" });
            }

            UserId id = (UserId)identityUser.Id.ToInt32OrDefault();
			User? userRecord = await userRepository.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			if (userRecord == null)
            {
                return Json(new { theme = "Simotek", success = false, message = "User entity not found" });
            }

            var themeObj = themeService.GetTheme(userRecord.ThemePreference);
            
            return Json(new 
            { 
                theme = userRecord.ThemePreference.ToString(),
                themeName = themeObj.Name,
                success = true 
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user theme");
            return Json(new { theme = "Simotek", success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Update user's theme preference
    /// </summary>
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UpdateTheme([FromBody] UpdateThemeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (!Enum.TryParse<ThemePreference>(request.Theme, out var themePreference))
            {
                return Json(new { success = false, message = "Invalid theme" });
            }

            var identityUser = GetUser();
            if (identityUser == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

			UserId id = (UserId)identityUser.Id.ToInt32OrDefault();
			User? userRecord = await userRepository.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			if (userRecord == null)
            {
                return Json(new { success = false, message = "User entity not found" });
            }

			userRecord.ThemePreference = themePreference;
            await userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

            // Store in session for immediate effect (if session is available)
            // Note: Session storage is optional - if session is not configured, we skip it
            var session = GetSessionSafely();
            if (session != null && session.IsAvailable)
            {
                try
                {
                    session.SetString("UserTheme", themePreference.ToString());
                }
                catch (Exception ex)
                {
                    // Log session-related errors but don't fail the request
                    logger.LogWarning(ex, "Error storing theme in session, continuing without session storage");
                }
            }

            // Get theme for response
            var themeObj = themeService.GetTheme(themePreference);

            return Json(new 
            { 
                success = true, 
                theme = themePreference.ToString(),
                themeName = themeObj.Name,
				variables = new
				{
					// Primary Colors
					primary = themeObj.Primary,
					primaryHover = themeObj.PrimaryHover,
					primaryActive = themeObj.PrimaryActive,
					primaryLight = themeObj.PrimaryLight,
					// Secondary Colors
					secondary = themeObj.Secondary,
					secondaryHover = themeObj.SecondaryHover,
					secondaryActive = themeObj.SecondaryActive,
					// Semantic Colors
					success = themeObj.Success,
					warning = themeObj.Warning,
					danger = themeObj.Danger,
					info = themeObj.Info,
					// Background Colors
					backgroundPrimary = themeObj.BackgroundPrimary,
					backgroundSecondary = themeObj.BackgroundSecondary,
					backgroundTertiary = themeObj.BackgroundTertiary,
					backgroundCard = themeObj.BackgroundCard,
					backgroundHover = themeObj.BackgroundHover,
					backgroundActive = themeObj.BackgroundActive,
					backgroundLight = themeObj.BackgroundLight,
					// Text Colors
					textPrimary = themeObj.TextPrimary,
					textSecondary = themeObj.TextSecondary,
					textMuted = themeObj.TextMuted,
					textOnDark = themeObj.TextOnDark,
					// Border Colors
					borderLight = themeObj.BorderLight,
					borderMedium = themeObj.BorderMedium,
					borderDark = themeObj.BorderDark,
					// Chart Colors
					chartText = themeObj.ChartText,
					chartTextSecondary = themeObj.ChartTextSecondary,
					chartBackground = themeObj.ChartBackground,
					chartPlotBackground = themeObj.ChartPlotBackground,
					chartGridLine = themeObj.ChartGridLine,
					chartAxisLine = themeObj.ChartAxisLine,
					// Report Colors
					reportText = themeObj.ReportText,
					reportTextSecondary = themeObj.ReportTextSecondary,
					reportBackground = themeObj.ReportBackground,
					reportHeaderBackground = themeObj.ReportHeaderBackground,
					reportTableBorder = themeObj.ReportTableBorder,
					reportTableRowAlternate = themeObj.ReportTableRowAlternate,
					// Header & Navigation
					headerBackground = themeObj.HeaderBackground,
					headerText = themeObj.HeaderText,
					headerUserBackground = themeObj.HeaderUserBackground,
					headerUserText = themeObj.HeaderUserText,
					// Dashboard
					dashboardTabBackground = themeObj.DashboardTabBackground,
					dashboardTabText = themeObj.DashboardTabText,
					dashboardWidgetBackground = themeObj.DashboardWidgetBackground,
					dashboardWidgetBorder = themeObj.DashboardWidgetBorder,
					dashboardWidgetTitleText = themeObj.DashboardWidgetTitleText,
					// Form
					formBackground = themeObj.FormBackground,
					formInputBackground = themeObj.FormInputBackground,
					formInputText = themeObj.FormInputText,
					formInputBorder = themeObj.FormInputBorder,
					formInputBorderFocus = themeObj.FormInputBorderFocus,
					formButtonBackground = themeObj.FormButtonBackground,
					formButtonText = themeObj.FormButtonText
				}
			});
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user theme");
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get theme CSS variables as JSON
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetThemeVariables(CancellationToken cancellationToken)
    {
        try
        {
            var identityUser = GetUser();
            ThemePreference theme = ThemePreference.Simotek;

            if (identityUser != null)
            {
				UserId id = (UserId)identityUser.Id.ToInt32OrDefault();
				User? userRecord = await userRepository.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

				if (userRecord != null)
                {
                    theme = userRecord.ThemePreference;
                }
            }

            var themeObj = themeService.GetTheme(theme);

            return Json(new
            {
                success = true,
                theme = theme.ToString(),
                themeName = themeObj.Name,
                variables = new
                {
                    // Primary Colors
                    primary = themeObj.Primary,
                    primaryHover = themeObj.PrimaryHover,
                    primaryActive = themeObj.PrimaryActive,
                    primaryLight = themeObj.PrimaryLight,
                    // Secondary Colors
                    secondary = themeObj.Secondary,
                    secondaryHover = themeObj.SecondaryHover,
                    secondaryActive = themeObj.SecondaryActive,
                    // Semantic Colors
                    success = themeObj.Success,
                    warning = themeObj.Warning,
                    danger = themeObj.Danger,
                    info = themeObj.Info,
                    // Background Colors
                    backgroundPrimary = themeObj.BackgroundPrimary,
                    backgroundSecondary = themeObj.BackgroundSecondary,
                    backgroundTertiary = themeObj.BackgroundTertiary,
                    backgroundCard = themeObj.BackgroundCard,
                    backgroundHover = themeObj.BackgroundHover,
                    backgroundActive = themeObj.BackgroundActive,
                    backgroundLight = themeObj.BackgroundLight,
                    // Text Colors
                    textPrimary = themeObj.TextPrimary,
                    textSecondary = themeObj.TextSecondary,
                    textMuted = themeObj.TextMuted,
                    textOnDark = themeObj.TextOnDark,
                    // Border Colors
                    borderLight = themeObj.BorderLight,
                    borderMedium = themeObj.BorderMedium,
                    borderDark = themeObj.BorderDark,
                    // Chart Colors
                    chartText = themeObj.ChartText,
                    chartTextSecondary = themeObj.ChartTextSecondary,
                    chartBackground = themeObj.ChartBackground,
                    chartPlotBackground = themeObj.ChartPlotBackground,
                    chartGridLine = themeObj.ChartGridLine,
                    chartAxisLine = themeObj.ChartAxisLine,
                    // Report Colors
                    reportText = themeObj.ReportText,
                    reportTextSecondary = themeObj.ReportTextSecondary,
                    reportBackground = themeObj.ReportBackground,
                    reportHeaderBackground = themeObj.ReportHeaderBackground,
                    reportTableBorder = themeObj.ReportTableBorder,
                    reportTableRowAlternate = themeObj.ReportTableRowAlternate,
                    // Header & Navigation
                    headerBackground = themeObj.HeaderBackground,
                    headerText = themeObj.HeaderText,
                    headerUserBackground = themeObj.HeaderUserBackground,
                    headerUserText = themeObj.HeaderUserText,
                    // Dashboard
                    dashboardTabBackground = themeObj.DashboardTabBackground,
                    dashboardTabText = themeObj.DashboardTabText,
                    dashboardWidgetBackground = themeObj.DashboardWidgetBackground,
                    dashboardWidgetBorder = themeObj.DashboardWidgetBorder,
                    dashboardWidgetTitleText = themeObj.DashboardWidgetTitleText,
                    // Form
                    formBackground = themeObj.FormBackground,
                    formInputBackground = themeObj.FormInputBackground,
                    formInputText = themeObj.FormInputText,
                    formInputBorder = themeObj.FormInputBorder,
                    formInputBorderFocus = themeObj.FormInputBorderFocus,
                    formButtonBackground = themeObj.FormButtonBackground,
                    formButtonText = themeObj.FormButtonText
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting theme variables");
            return Json(new { success = false, message = ex.Message });
        }
    }

    /// <summary>
    /// Get all available themes
    /// </summary>
    [HttpGet]
    public IActionResult GetAllThemes()
    {
        try
        {
            var themes = themeService.GetAllThemes();
            return Json(new
            {
                success = true,
                themes = themes.Select(t => new
                {
                    preference = t.Preference.ToString(),
                    name = t.Name,
                    primary = t.Primary,
                    primaryHover = t.PrimaryHover,
                    primaryActive = t.PrimaryActive,
                    primaryLight = t.PrimaryLight,
                    secondary = t.Secondary,
                    secondaryHover = t.SecondaryHover,
                    secondaryActive = t.SecondaryActive,
                    success = t.Success,
                    warning = t.Warning,
                    danger = t.Danger,
                    info = t.Info,
                    backgroundPrimary = t.BackgroundPrimary,
                    backgroundSecondary = t.BackgroundSecondary,
                    backgroundTertiary = t.BackgroundTertiary,
                    backgroundCard = t.BackgroundCard,
                    backgroundHover = t.BackgroundHover,
                    backgroundActive = t.BackgroundActive,
                    backgroundLight = t.BackgroundLight,
                    textPrimary = t.TextPrimary,
                    textSecondary = t.TextSecondary,
                    textMuted = t.TextMuted,
                    textOnDark = t.TextOnDark,
                    borderLight = t.BorderLight,
                    borderMedium = t.BorderMedium,
                    borderDark = t.BorderDark,
                    chartText = t.ChartText,
                    chartTextSecondary = t.ChartTextSecondary,
                    chartBackground = t.ChartBackground,
                    chartPlotBackground = t.ChartPlotBackground,
                    chartGridLine = t.ChartGridLine,
                    chartAxisLine = t.ChartAxisLine,
                    reportText = t.ReportText,
                    reportTextSecondary = t.ReportTextSecondary,
                    reportBackground = t.ReportBackground,
                    reportHeaderBackground = t.ReportHeaderBackground,
                    reportTableBorder = t.ReportTableBorder,
                    reportTableRowAlternate = t.ReportTableRowAlternate,
                    headerBackground = t.HeaderBackground,
                    headerText = t.HeaderText,
                    headerUserBackground = t.HeaderUserBackground,
                    headerUserText = t.HeaderUserText,
                    dashboardTabBackground = t.DashboardTabBackground,
                    dashboardTabText = t.DashboardTabText,
                    dashboardWidgetBackground = t.DashboardWidgetBackground,
                    dashboardWidgetBorder = t.DashboardWidgetBorder,
                    dashboardWidgetTitleText = t.DashboardWidgetTitleText,
                    formBackground = t.FormBackground,
                    formInputBackground = t.FormInputBackground,
                    formInputText = t.FormInputText,
                    formInputBorder = t.FormInputBorder,
                    formInputBorderFocus = t.FormInputBorderFocus,
                    formButtonBackground = t.FormButtonBackground,
                    formButtonText = t.FormButtonText
                })
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all themes");
            return Json(new { success = false, message = ex.Message });
        }
    }
}

/// <summary>
/// Request model for updating theme
/// </summary>
public class UpdateThemeRequest
{
    public string Theme { get; set; } = string.Empty;
}

