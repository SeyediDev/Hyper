using Neo.Bpms.Domain.Models.Cmmn.UI;

namespace Hyper.AdminPanel.Domain.Domain;

public partial class HyperMenuDefinitions : MenuDefinition
{
    public override MenuItem Identify() => AddMenu(nameof(DomainProvider.Domain), "", "", "");

    public override bool DefineMenuItems()
    {
        var home = AddMenu("صفحهٔ اصلی", "home-dashboard", "Home", "Index");
        home.IsPublic = true;
        AddMenu_Hyper();

        // System Monitoring - keep the monitoring dashboard accessible from the admin menu.
        var monitoringMenu = AddMenu("مانیتورینگ سیستم", "pulse-heart", "Monitoring", "Index");
        monitoringMenu.IsPublic = true;

        // Open the dashboard in a separate tab, matching the Club admin panel behavior.
        try
        {
            var linkTargetProperty = monitoringMenu.GetType().GetProperty("LinkTarget");
            if (linkTargetProperty != null && linkTargetProperty.CanWrite)
            {
                linkTargetProperty.SetValue(monitoringMenu, "_blank");
            }

            var linkAttributesProperty = monitoringMenu.GetType().GetProperty("LinkAttributes");
            if (linkAttributesProperty != null && linkAttributesProperty.CanWrite)
            {
                var attributes = linkAttributesProperty.GetValue(monitoringMenu)
                    as System.Collections.Generic.Dictionary<string, string>;
                if (attributes == null)
                {
                    attributes = [];
                    linkAttributesProperty.SetValue(monitoringMenu, attributes);
                }

                attributes["target"] = "_blank";
                attributes["rel"] = "noopener noreferrer";
            }
        }
        catch
        {
            // Older MenuItem implementations may not expose link attributes.
        }

        return true;
    }
}
