using Hyper.AdminPanel.Domain.UiDefinitions.HomePage;
using Hyper.Domain.Entities.Database;
using Minio.DataModel.Notification;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace Hyper.AdminPanel.Domain.Domain;

public partial class HyperMenuDefinitions : MenuDefinition
{
    private void AddMenu_Hyper()
    {
        AddMenu("بینش و داشبوردها", "home-dashboard", "Insights & Dashboards", "");
        {
            StartSubMenus();
            AddMenu<HomePageEntity, HomePageEntityUiDefinitions.HomePageDashboard>("داشبورد کلان", "home-dashboard");
            AddMenuDivider();
            AddReport<SqlActIdGroup>("گروه", "wallet-money");
            EndSubMenus();
        }
        AddMenu("اطلاعات", "calendar-event", "Tenants", "");
        {
            StartSubMenus();
            AddMenu<SqlTblShareholder>("سهامدار", "building-organization");
            AddMenu<SqlTblShop>("مغازه", "building-organization");
            AddMenu<SqlTblPerson>("شخص", "building-organization");
            AddMenu<SqlTblWarehouse>("انبار", "building-organization");
            AddMenuDivider();
            AddMenu("گزارشات", "chart-bar", "Information Reports", "");
            {
                StartSubMenus();
                AddReport<SqlTblShareholder>("گزارش سهامدار", "building-organization");
                AddReport<SqlTblShop>("گزارش مغازه", "building-organization");
                AddReport<SqlTblPerson>("گزارش شخص", "building-organization");
                AddReport<SqlTblWarehouse>("انبار", "building-organization");
                EndSubMenus();
            }
            EndSubMenus();
        }
        AddMenu("نمای کلی", "home-dashboard", "AdminDashboard", "Index");
        AddMenu("مغازه‌دار و اتصال", "building-organization", "MerchantSimulation", "Index");
        AddMenu("عملیات یکسان‌سازی", "activity-monitor", "MerchantSimulation", "Dashboard");
    }
}
