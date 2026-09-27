using Hyper.Domain.Entities.Events.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.HomePage;

public partial class HomePageEntityUiDefinitions
{
    /// <summary>
    /// داشبورد تخصصی عملیات برای کنترل تراکنش‌ها، قوانین و سلامت سامانه
    /// </summary>
    public class OperationsCommandDashboard : DashboardDefinition
    {
        public override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];

        protected override Form Identify()
        {
            return DefineDashboard("داشبورد عملیات و کنترل");
        }

        protected override void DataSources()
        {
            base.DataSources();
            AddReport<CustomerTransaction>();
            AddReport<Point>();
            AddReport<Promotion>();
            AddReport<EventLog>();
            AddReport<Product>();
        }

        /// <summary>
        /// تب تراکنش‌ها و امتیازات
        /// </summary>
        public class TransactionsAndPointsConfig : DashboardConfigDefinition
        {
            protected override string Title => "تراکنش‌ها و امتیازات";
            protected override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];
            protected override bool IsDefault => true;

            public class TransactionsByMonthWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.TransactionsByMonthConfig>
            {
                public override string Title => "روند ماهانه تراکنش‌ها";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 260;
                public override int Width => 8;
            }

            public class TransactionsByTypeWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.TransactionsByTypeConfig>
            {
                public override string Title => "تراکنش‌ها به تفکیک نوع";
                protected override int? HeightInPixels => 260;
                public override int Width => 4;
            }

            public class PointsByTypeWidget : DashboardDivWidgetDefinition<Point, PointUiDefinitions.PublicReport, PointUiDefinitions.PublicReport.PointsByTypeConfig>
            {
                public override string Title => "توزیع امتیازات";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class CustomerPointsBalanceWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.CustomerPointsBalanceConfig>
            {
                public override string Title => "موجودی امتیازات مشتریان";
                protected override int? MaxRecordCount => 20;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class UnvisitedTransactionsWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.UnvisitedTransactionsConfig>
            {
                public override string Title => "تراکنش‌های بازدید نشده";
                protected override int? MaxRecordCount => 30;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }
        }

        /// <summary>
        /// تب قوانین و کنترل سازگاری
        /// </summary>
        public class RulesAndComplianceConfig : DashboardConfigDefinition
        {
            protected override string Title => "قوانین و حاکمیت";
            protected override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];

            public class TransactionsByPromotionWidget : DashboardDivWidgetDefinition<CustomerTransaction, CustomerTransactionUiDefinitions.PublicReport, CustomerTransactionUiDefinitions.PublicReport.TransactionsByPromotionConfig>
            {
                public override string Title => "تراکنش‌ها بر اساس پویش";
                protected override int? MaxRecordCount => 20;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }
        }

        /// <summary>
        /// تب سلامت سامانه و کانال‌ها
        /// </summary>
        public class SystemHealthConfig : DashboardConfigDefinition
        {
            protected override string Title => "سلامت سامانه";
            protected override List<string>? Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];

            public class EventLogByMonthWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.EventLogByMonthConfig>
            {
                public override string Title => "رویدادها به تفکیک ماه";
                protected override int? MaxRecordCount => 12;
                protected override int? HeightInPixels => 260;
                public override int Width => 8;
            }

            public class EventLogByEventTypeWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.EventLogByEventTypeConfig>
            {
                public override string Title => "رویدادها به تفکیک نوع";
                protected override int? HeightInPixels => 260;
                public override int Width => 4;
            }

            public class SystemActivityWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.SystemActivityDailyConfig>
            {
                public override string Title => "فعالیت روزانه سامانه";
                protected override int? MaxRecordCount => 30;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class DailyActiveUsersWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.DailyActiveUsersConfig>
            {
                public override string Title => "کاربران فعال روزانه (DAU)";
                protected override int? MaxRecordCount => 30;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class EventLogByChannelWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.EventLogByChannelConfig>
            {
                public override string Title => "کانال‌های پر استفاده";
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }

            public class RecentEventsWidget : DashboardDivWidgetDefinition<EventLog, EventLogUiDefinitions.PublicReport, EventLogUiDefinitions.PublicReport.RecentEventsConfig>
            {
                public override string Title => "رویدادهای اخیر";
                protected override int? MaxRecordCount => 25;
                protected override int? HeightInPixels => 240;
                public override int Width => 6;
            }
        }
    }
}