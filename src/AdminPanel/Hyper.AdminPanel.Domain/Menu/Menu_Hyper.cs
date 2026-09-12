namespace Hyper.AdminPanel.Domain.Domain;

public partial class HyperMenuDefinitions : MenuDefinition
{
    private void AddMenu_Hyper()
    {
        AddMenu("نمای کلی", "home-dashboard", "AdminDashboard", "Index");
        AddMenu("مغازه‌دار و اتصال", "building-organization", "MerchantSimulation", "Index");
        AddMenu("عملیات یکسان‌سازی", "activity-monitor", "MerchantSimulation", "Dashboard");
    }
}
