using Neo.Bpms.Domain.Models.Cmmn.UI;

namespace Hyper.AdminPanel.Domain.Domain;

public partial class HyperMenuDefinitions : MenuDefinition
{
    public override MenuItem Identify()
    {
        return AddMenu(nameof(DomainProvider.Domain), "", "", "");
    }

    public override bool DefineMenuItems()
    {
        var homeMenuItem = AddMenu("صفحه اول", "home-dashboard", "Home", "Index");
        homeMenuItem.IsPublic = true;
        /*AddMenu("کارتابل", "cartable-menu", "", "");
        {
            StartSubMenus();
            AddMenu("کارهای ورودی", "cartable-menu", "Process", "MyProcesses");
            //AddMenuForProcess("کارهای فرآیند درخواست بررسی دسترسی پرسنل", "cartable-menu",
            //    nameof(RequestToReviewPersonnelAccess));
            //AddMenuForProcess("کارهای فرآیند درخواست مجوز موقت", "cartable-menu",
            //    nameof(RequestToChangeProfile));
            /*AddMenu("گزارش فرآیند", "cartable-menu", "", "");
            {
                StartSubMenus();
                //AddMenuForReport("گزارش فرآیند درخواست دسترسی", "cartable-menu", "IUM", nameof(PersonnelProfile), nameof(PersonnelAccessDefinitions.ProcessReport));
                //AddMenuForReport("گزارش فرآیند مجوز موقت", "cartable-menu", "IUM", nameof(PersonnelProfile), nameof(PersonnelAccessDefinitions.ProcessReport));
                EndSubMenus();
            }
            AddMenu("نظارت بر کسب و کار", "cartable-menu", "", "");
            {
                StartSubMenus();
                AddMenu<ActivityInstanceRecordDefinitions.ProcessDashboard>("داشبورد", null, null, null, "WorkflowId=ServicePurchaseProcess", "cartable-menu");
                AddMenu<ProcessInstanceRecordDefinitions.TotalProcessAnalysis>("نظارت بر فرآیندها", "cartable-menu");
                AddMenu<ProcessInstanceRecordDefinitions.ProcessAnalysis>("نظارت بر فرآیندهای غیرفعال", "cartable-menu");
                AddMenu<ProcessInstanceRecordDefinitions.ActiveProcessAnalysis>("نظارت بر فرآیندهای فعال", "cartable-menu");
                AddMenuDivider();
                AddMenu<ActivityInstanceRecordDefinitions.ProcessAnalysis>("نظارت بر فعالیت‌های کسب و کار", "cartable-menu");
                AddMenu<ActivityInstanceRecordDefinitions.OneProcessAnalysis>("نظارت بر فعالیت‌های یک فرآیند", "cartable-menu");
                AddMenu<ActivityInstanceRecordDefinitions.ReportOfMyWork>("نظارت بر فعالیت‌های من", "cartable-menu");
                EndSubMenus();
            }* /
            EndSubMenus();
        }*/
        /*AddMenu("شروع فرآیند", "process-menu", "", "");
        {
            StartSubMenus();
            //AddMenuForProcessTask("درخواست بررسی دسترسی پرسنل", "process-menu",
            //    nameof(RequestToReviewPersonnelAccess), "RequestToReviewPersonnelAccessTask");
            //AddMenuForProcessTask("درخواست مجوز موقت", "process-menu",
            //    nameof(RequestToChangeProfile), "RequestToChangeProfileTask");
            EndSubMenus();
        }*/
        AddMenu_Hyper();
        
        // System Monitoring - Fixed position, Public access for admins
        var monitoringMenu = AddMenu("مانیتورینگ سیستم", "activity-monitor", "Monitoring", "Index");
        monitoringMenu.IsPublic = true;
        
        // تنظیم target="_blank" برای باز شدن در تب جدید
        // اگر MenuItem property ای برای LinkTarget یا Attributes دارد، استفاده می‌کنیم
        try
        {
            // بررسی وجود property LinkTarget
            var linkTargetProperty = monitoringMenu.GetType().GetProperty("LinkTarget");
            if (linkTargetProperty != null && linkTargetProperty.CanWrite)
            {
                linkTargetProperty.SetValue(monitoringMenu, "_blank");
            }
            
            // بررسی وجود property LinkAttributes
            var linkAttributesProperty = monitoringMenu.GetType().GetProperty("LinkAttributes");
            if (linkAttributesProperty != null && linkAttributesProperty.CanWrite)
            {
                var attributes = linkAttributesProperty.GetValue(monitoringMenu) as System.Collections.Generic.Dictionary<string, string>;
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
            // اگر property وجود نداشت، JavaScript در _Layout.cshtml این کار را انجام می‌دهد
        }
        
        AddMenu("طراحی", "grid-layout", "", "");
        {
            StartSubMenus();
            AddMenu("طراحی فرآیند", "rule-checklist", "Process", "BpmnDesign");
            AddMenu("طراحی مدل اطلاعات و رابط کاربری", "cube-3d", "MetaDesign/App", "entity");
            AddMenu("طراحی مجموعه‌های پایه", "grid-layout", "MetaDesign/App", "enum");
            AddMenu("طراحی منو", "list-checklist", "MetaDesign/App", "menu");
            AddMenu("یکسان‌سازی پایگاه داده", "cog-wheel", "Migration", "Index");
            EndSubMenus();
        }
        return true;
    }
}
