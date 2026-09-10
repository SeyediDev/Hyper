using Hyper.Domain.Entities.Events.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Events;

public partial class EventLogUiDefinitions : CRUDDefinition<EventLog>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-list-alt";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(EventLog.Tenant),
                        nameof(EventLog.CustomerTenant),
                        nameof(EventLog.EventType),
                        nameof(EventLog.ReceiveEventType),
                        nameof(EventLog.EventChannel),
                        nameof(EventLog.CreateDate)
                        );
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(EventLog.Tenant),
                       nameof(EventLog.CustomerTenant),
                       nameof(EventLog.EventType),
                       nameof(EventLog.ReceiveEventType),
                       nameof(EventLog.EventChannel),
                       nameof(EventLog.Promotion),
                       nameof(EventLog.PointLevel),
                       nameof(EventLog.Reward),
                       nameof(EventLog.Product),
                       nameof(EventLog.Asset)
                       );
    }

    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        // =====================================================
        // Event Analytics Reports
        // =====================================================

        /// <summary>
        /// KPI: تعداد کل رویدادها
        /// </summary>
        public class TotalEventsConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];
            protected override string Name => "تعداد کل رویدادها";

            protected override void DefineGroupBy()
            {
                Count();
            }
        }

        /// <summary>
        /// گزارش رویدادها به تفکیک ماه
        /// </summary>
        public class EventLogByMonthConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "رویدادهای ماهانه";

            protected override void DefineGroupBy()
            {
                GroupBy("CreatedAtMonth", "ماه");
                Count(null, "تعداد رویدادها");
            }
        }

        /// <summary>
        /// گزارش رویدادها به تفکیک کانال
        /// </summary>
        public class EventLogByChannelConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "رویدادها به تفکیک کانال";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(EventLog.EventChannel), "کانال");
                Count(null, "تعداد");
            }
        }

        /// <summary>
        /// گزارش رویدادها به تفکیک نوع رویداد
        /// </summary>
        public class EventLogByEventTypeConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "رویدادها به تفکیک نوع";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(EventLog.EventType), "نوع رویداد");
                Count(null, "تعداد");
            }
        }

		/// <summary>
		/// گزارش رویدادهای مرتبط با پویش‌ها و کمپین‌ها
		/// </summary>
		public class EventLogByPromotionConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.MarketingManager];
            protected override string Name => "رویدادهای مرتبط با پویش‌ها و کمپین‌ها";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(EventLog.Promotion), "پویش‌/کمپین‌");
                DisplayColumn(nameof(EventLog.CustomerTenant), "مشتری");
                DisplayColumn(nameof(EventLog.CreateDate), "تاریخ");
                DisplayColumn(nameof(EventLog.EventType), "نوع رویداد");
                
                OrderByDesc(nameof(EventLog.CreateDate)); // نزولی - جدیدترین
            }
        }

        /// <summary>
        /// گزارش فعال‌ترین مشتریان (بر اساس تعداد رویدادها)
        /// </summary>
        public class MostActiveCustomersConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "فعال‌ترین مشتریان";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(EventLog.CustomerTenant), "مشتری");
                Count(null, "تعداد رویدادها");
                OrderByDesc("COUNT"); // نزولی - بیشترین رویداد
            }
        }

        /// <summary>
        /// گزارش رویدادها به تفکیک نوع فعال‌سازی
        /// </summary>
        public class EventLogByTriggerTypeConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "رویدادها به تفکیک نوع فعال‌سازی";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(EventLog.ReceiveEventType), "نوع فعال‌سازی");
                Count(null, "تعداد");
            }
        }

        /// <summary>
        /// گزارش رویدادهای مرتبط با پاداش‌ها
        /// </summary>
        public class EventLogByRewardConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "رویدادهای مرتبط با پاداش";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(EventLog.Reward), "پاداش");
                Count(null, "تعداد دریافت");
                OrderByDesc("COUNT"); // نزولی - پرطرفدارترین پاداش
            }
        }

        /// <summary>
        /// گزارش رویدادهای مرتبط با محصولات
        /// </summary>
        public class EventLogByProductConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "رویدادهای محصولات";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(EventLog.Product), "محصول");
                Count(null, "تعداد رویداد");
                OrderByDesc("COUNT"); // نزولی - پرطرفدارترین
            }
        }

        /// <summary>
        /// گزارش فعالیت سیستم روزانه
        /// </summary>
        public class SystemActivityDailyConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin];
            protected override string Name => "فعالیت سیستم روزانه";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Day({nameof(EventLog.CreateDate)})", "روز");
                Count(null, "تعداد رویدادها");
            }
        }

        /// <summary>
        /// گزارش کاربران فعال روزانه (DAU)
        /// </summary>
        public class DailyActiveUsersConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "کاربران فعال روزانه (DAU)";

            protected override void DefineGroupBy()
            {
                GroupByFormula($"Day({nameof(EventLog.CreateDate)})", "روز");
                Count(nameof(EventLog.CustomerTenant), "تعداد کاربران منحصر به فرد");
            }
        }

        /// <summary>
        /// گزارش کاربران فعال ماهانه (MAU)
        /// </summary>
        public class MonthlyActiveUsersConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "کاربران فعال ماهانه (MAU)";

            protected override void DefineGroupBy()
            {
                GroupBy("CreatedAtMonth", "ماه");
                Count(nameof(EventLog.CustomerTenant), "تعداد کاربران منحصر به فرد");
            }
        }

        /// <summary>
        /// گزارش رویدادها به تفکیک ماه شمسی
        /// </summary>
        public class EventLogByShamsiMonthConfig() : ChartConfigDefinition(ChartType.Column)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "رویدادها به تفکیک ماه شمسی";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(EventLog.ShamsiMonth), "ماه شمسی");
                Count(null, "تعداد رویدادها");
            }
        }

        /// <summary>
        /// تقویم رویدادهای روزانه - نمایش تعداد رویدادها برای هر روز
        /// </summary>
        public class EventLogByDayCalendarConfig() : ChartConfigDefinition(ChartType.Calendar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager, HyperRoles.Analyst];
            protected override string Name => "تقویم رویدادهای روزانه";

            protected override void DefineGroupBy()
            {
                GroupBy("CreatedAtDate", "تاریخ");
                Count(null, "تعداد رویدادها");
            }
        }

        /// <summary>
        /// گزارش آخرین رویدادها
        /// </summary>
        public class RecentEventsConfig() : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "آخرین رویدادها";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(EventLog.CustomerTenant), "مشتری");
                DisplayColumn(nameof(EventLog.EventType), "نوع رویداد");
                DisplayColumn(nameof(EventLog.ReceiveEventType), "نوع فعال‌سازی");
                DisplayColumn(nameof(EventLog.CreateDate), "تاریخ");
                OrderByDesc(nameof(EventLog.CreateDate)); // نزولی - جدیدترین
            }
        }

        /// <summary>
        /// گزارش ماتریس محصول × مشتری (بر اساس EventLog)
        /// </summary>
        public class ProductCustomerMatrixConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "ماتریس محصول × مشتری";

			protected override GroupByViewType GroupByViewType => GroupByViewType.Matrix;

            protected override void DefineGroupBy()
            {
                // محصول در ستون عمودی
                GroupByFormula($"{nameof(EventLog.Product)}.{nameof(Product.Title)}", "محصول", true, ConfiguredReport.ReportMatrixType.Vertical);
                // مشتری در ستون افقی (از CustomerTenant استفاده می‌کنیم)
                GroupByFormula($"{nameof(EventLog.CustomerTenant)}.{nameof(CustomerTenant.Customer)}", "مشتری", true, ConfiguredReport.ReportMatrixType.Horizontal);
                // مقادیر: تعداد رویدادها
                Count(nameof(EventLog.Id), "تعداد رویدادها");
            }
        }

        /// <summary>
        /// گزارش ماتریس محصول × کمپین
        /// </summary>
        public class ProductPromotionMatrixConfig() : GroupByConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst, HyperRoles.MarketingManager];
            protected override string Name => "ماتریس محصول × کمپین";
            protected override GroupByViewType GroupByViewType => GroupByViewType.Matrix;
            protected override string WhereCondition => $"{nameof(EventLog.ProductId)} != null AND {nameof(EventLog.PromotionId)} != null";

            protected override void DefineGroupBy()
            {
                // محصول در ستون عمودی
                GroupByFormula($"{nameof(EventLog.Product)}.{nameof(Product.Title)}", "محصول");
                LayoutColumn(false, $"{nameof(EventLog.Product)}.{nameof(Product.Title)}", ConfiguredReport.ReportMatrixType.Vertical);

                // کمپین در ستون افقی
                GroupByFormula($"{nameof(EventLog.Promotion)}.{nameof(Promotion.Title)}", "کمپین");
                LayoutColumn(false, $"{nameof(EventLog.Promotion)}.{nameof(Promotion.Title)}", ConfiguredReport.ReportMatrixType.Horizontal);

                // مقادیر: تعداد رویدادها، تعداد مشتریان
                // توجه: CAC از PromotionMetrics قابل دسترسی نیست - برای CAC از گزارشات PromotionMetrics استفاده کنید
                Count(nameof(EventLog.Id), "تعداد رویدادها");
                Count(nameof(EventLog.CustomerTenant), "تعداد مشتریان");
            }
        }
    }
}

