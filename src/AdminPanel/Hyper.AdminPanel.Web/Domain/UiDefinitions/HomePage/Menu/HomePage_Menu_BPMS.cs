using Neo.Bpms.Domain.Features.UiDefinitions.ProcessData;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class Default
    {
        private void AddBPMSMenu()
        {
            AddMultiTabItem("تنظیمات", "Setting", ContextualStyle.Secondary);
            AddFormPart("طراحی", "Design");
            {
                AddMenu("طراحی فرآیند", "flaticon bpms-flaticon-flow-chart", "Process", "BpmnDesign");
                AddMenu("طراحی مدل اطلاعات و رابط کاربری", "flaticon bpms-flaticon-high-design", "MetaDesign/App", "entity");
                AddMenu("طراحی مجموعه‌های پایه", "flaticon bpms-flaticon-high-design", "MetaDesign/App", "enum");
                AddMenu("طراحی منو", "fa fa-bars", "MetaDesign/App", "menu");
                AddMenu("یکسان‌سازی پایگاه داده", "fa fa-database", "Migration", "Index");
                EndPart();
            }
            AddFormPart("نظارت بر کسب و کار", "Business Servey");
            {
                AddMenu("کارتابل", "flaticon bpms-flaticon-cartable-computer-work", "Process", "MyProcesses");
                AddMenu<ActivityInstanceRecordDefinitions.ProcessDashboard>("داشبورد", null, null, null, "WorkflowId=ServicePurchaseProcess");
                AddMenu<ProcessInstanceRecordDefinitions.TotalProcessAnalysis>("نظارت بر فرآیندها");
                AddMenu<ProcessInstanceRecordDefinitions.ProcessAnalysis>("نظارت بر فرآیندهای غیرفعال");
                AddMenu<ProcessInstanceRecordDefinitions.ActiveProcessAnalysis>("نظارت بر فرآیندهای فعال");
                AddMenu<ActivityInstanceRecordDefinitions.ProcessAnalysis>("نظارت بر فعالیت‌های کسب و کار");
                AddMenu<ActivityInstanceRecordDefinitions.OneProcessAnalysis>("نظارت بر فعالیت‌های یک فرآیند");
                AddMenu<ActivityInstanceRecordDefinitions.ReportOfMyWork>("نظارت بر فعالیت‌های من");
                EndPart();
            }
            //AddFormPart("دسترسی", "Access");
            //{
            //    AddPartForm<SystemUserGroup>("گروه کاربران");
            //    AddPartForm<SystemUser>("کاربران", "Admin");
            //    AddMenu("فهرست دسترسی‌های من", "flaticon bpms-flaticon-authorization-1", "Account", "MyAccess");
            //    EndPart();
            //}

            EndPart();
        }

        private void AddBPMSPart(string name, string enName)
        {
            AddPart(name, "BPMS " + enName, "#FFF"); //"#C3E2FF"
        }
    }
}
