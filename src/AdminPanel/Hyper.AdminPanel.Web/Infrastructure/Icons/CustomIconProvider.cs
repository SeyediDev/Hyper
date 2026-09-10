using Neo.Bpms.UI.MVC.Controls;

namespace Hyper.AdminPanel.Web.Infrastructure.Icons;

/// <summary>
/// Provides custom icon names for Hyper platform menu system
/// </summary>
public class CustomIconProvider : ICustomIconProvider
{
    private static readonly HashSet<string> CustomIcons =
    [
        // Modern Hyper icons - maintain alphabetically for easy lookup
        "bolt-lightning", "box-package", "building-organization",
        "calendar-event", "chart-bar", "cog-settings", "cog-wheel", "coins-money", "cube-3d",
        "file-document", "flag-trigger", "gift-present", "grid-layout",
        "home-dashboard", "info-circle", "list-checklist", "medal-reward",
        "piggy-bank", "question-circle", "rule-checklist", "rss-signal",
        "sliders-h", "star-badge", "store-shop", "ticket-lottery", "trophy-star",
        "user-admin", "user-badge", "user-circle", "user-settings",
        "users-analysis", "users-group", "users-info", "users-network",
        "wallet-money", "chart-mixed"
    ];

    /// <summary>
    /// Checks if an icon name is a custom icon
    /// </summary>
    /// <param name="iconName">The icon name to check</param>
    /// <returns>True if the icon is a custom icon, false otherwise</returns>
    public bool IsCustomIcon(string iconName)
    {
        if (string.IsNullOrEmpty(iconName))
            return false;
            
        return CustomIcons.Contains(iconName);
    }
}