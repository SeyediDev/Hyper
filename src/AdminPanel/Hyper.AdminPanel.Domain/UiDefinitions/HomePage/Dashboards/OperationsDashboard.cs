using Hyper.AdminPanel.Domain.UiDefinitions.CustomerTenants.CustomerTransactions.Base;
using Hyper.AdminPanel.Domain.UiDefinitions.Events.Base;
using CustomerTransactionUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.Customers.CustomerTransactionUiDefinitions;
using EventLogUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.Events.EventLogUiDefinitions;
using TenantAttributeValueUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.Tenants.TenantAttributeValueUiDefinitions;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    public partial class HomePageDashboard
    {
        /// <summary>
        /// تب پایداری عملیات و سلامت سامانه برای پایش لحظه‌ای عملکرد
        /// </summary>
        public class OperationsHealthDashboard : DashboardConfigDefinition
        {
            protected override string Title => "سلامت عملیات و سامانه";
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];
            protected override bool IsDefault => false;
            protected override string Icon => "cog-wheel";

            /// <summary>
            /// KPI: امتیازات اعطا شده امروز
            /// </summary>
            public class OperationsPointsIssuedWidget : KpiWidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.PointsIssuedStatsConfig>
            {
                public override string Title => "امتیازات اعطا شده";
                protected override string IconClass => "fal fa-arrow-circle-up";
                protected override string IconId => "chart-bar";
            }

            /// <summary>
            /// KPI: امتیازات مصرف شده
            /// </summary>
            public class OperationsPointsRedeemedWidget : KpiWidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.PointsRedeemedStatsConfig>
            {
                public override string Title => "امتیازات مصرف شده";
                protected override string IconClass => "fal fa-arrow-circle-down";
                protected override string IconId => "chart-bar";
            }

            /// <summary>
            /// KPI: میانگین ارزش تراکنش
            /// </summary>
            public class OperationsAverageTransactionWidget : KpiWidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.AverageTransactionValueConfig>
            {
                public override string Title => "میانگین ارزش تراکنش";
                protected override string IconClass => "fal fa-coins";
                protected override string IconId => "coins-money";
            }

            /// <summary>
            /// KPI: تعداد کل رویدادهای ثبت‌شده
            /// </summary>
            public class OperationsTotalEventsWidget : KpiWidgetEventLogBase<EventLogUiDefinitions.PublicReport.TotalEventsConfig>
            {
                public override string Title => "رویدادهای ثبت‌شده";
                protected override string IconClass => "fal fa-analytics";
                protected override string IconId => "calendar-event";
            }

            public class TransactionsByMonthWidget : WidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.TransactionsByMonthConfig>
            {
                public override string Title => "روند ماهانه تراکنش‌ها";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 260;
                public override int Width => 8;
                protected override string Icon => "chart-bar";
            }

            public class TransactionsByTypeWidget : WidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.TransactionsByTypeConfig>
            {
                public override string Title => "توزیع تراکنش‌ها به تفکیک نوع";
                protected override int? HeightInPixels => 260;
                public override int Width => 4;
                protected override string Icon => "grid-layout";
            }

            public class EventLogByEventTypeWidget : WidgetEventLogBase<EventLogUiDefinitions.PublicReport.EventLogByEventTypeConfig>
            {
                public override string Title => "رویدادها به تفکیک نوع";
                protected override int? HeightInPixels => 260;
                public override int Width => 6;
                protected override string Icon => "list-checklist";
            }

            public class DailyActiveUsersWidget : WidgetEventLogBase<EventLogUiDefinitions.PublicReport.DailyActiveUsersConfig>
            {
                public override string Title => "کاربران فعال روزانه (DAU)";
                protected override int? MaxRecordCount => 30;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
                protected override string Icon => "users-group";
            }

            public class RewardPlanChannelMatrixWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.RewardChannelPlanPivotConfig>
            {
                public override string Title => "تقاطع کانال و پاداش/طرح";
                protected override int? MaxRecordCount => 30;
                protected override int? HeightInPixels => 400;
                public override int Width => 12;
                protected override string Icon => "list-checklist";
            }

            public class TenantAttributeValueCalendarWidget : DashboardDivWidgetDefinition<TenantAttributeValue, TenantAttributeValueUiDefinitions.PublicReport, TenantAttributeValueUiDefinitions.PublicReport.DailyCalendarConfig>
            {
                public override string Title => "تقویم روزانه ویژگی‌ها";
                protected override int? HeightInPixels => 360;
                public override int Width => 12;
                protected override string Icon => "calendar-days";
            }
/*
            public class EventLogByDayCalendarWidget : WidgetEventLogBase<EventLogUiDefinitions.PublicReport.EventLogByDayCalendarConfig>
            {
                public override string Title => "تقویم رویدادهای روزانه";
                protected override int? HeightInPixels => 350;
                public override int Width => 6;
                protected override string Icon => "calendar-days";
            }

            public class TransactionsByDayCalendarWidget : WidgetCustomerTransactionBase<CustomerTransactionUiDefinitions.PublicReport.TransactionsByDayCalendarConfig>
            {
                public override string Title => "تقویم تراکنش‌های روزانه";
                protected override int? HeightInPixels => 350;
                public override int Width => 6;
                protected override string Icon => "calendar-days";
            }
*/            
        }
    }
}

