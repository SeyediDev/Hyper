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
        return true;
    }
}
