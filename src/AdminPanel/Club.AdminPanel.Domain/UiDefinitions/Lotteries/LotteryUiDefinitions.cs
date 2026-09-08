using Hyper.Domain.Entities.Lotteries;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Lotteries;

public partial class LotteryUiDefinitions : CRUDDefinition<Lottery>
{
    public override List<string>? Roles =>
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.MarketingManager
    ];
    public override string? Icon => "fa fa-random";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(Lottery.Promotion)
                 , nameof(Lottery.Title)
                 , nameof(Lottery.LotteryType)
                 , nameof(Lottery.FromDate)
                 , nameof(Lottery.ToDate)
                 , nameof(Lottery.IsScheduled)
                 );
        AddSubjectColumn<LotteryRelations>();
    }
    
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Lottery.Title)
                , nameof(Lottery.Promotion)
                , nameof(Lottery.LotteryType)
                , nameof(Lottery.FromDate)
                , nameof(Lottery.ToDate)
                , nameof(Lottery.IsScheduled)
                , nameof(Lottery.SchedulingKind)
                , nameof(Lottery.DayOfWeek)
                , nameof(Lottery.DayOfMonth)
                , nameof(Lottery.Month)
                , nameof(Lottery.DayOfYear)
                , nameof(Lottery.Hour)
                , nameof(Lottery.Minute)
                );
    }
    
    public class LotteryRelations() : SubjectEditForm<LotteryRelations>("پاداش‌ها و شرکت‌کنندگان")
    {
        protected override void ViewModel()
        {
            AddSubTable<LotteryReward>(nameof(LotteryReward.Lottery), "پاداش‌ها");
            AddSubTable<LotteryParticipant>(nameof(LotteryParticipant.Lottery), "شرکت‌کنندگان");
        }
    }

    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        /// <summary>
        /// گزارش قرعه‌کشی‌های فعال
        /// </summary>
        public partial class ActiveLotteriesConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(Lottery.ToDate)} >= DateTime.UtcNow";
            protected override string Name => "قرعه‌کشی‌های فعال";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Lottery.Title), "عنوان");
                DisplayColumn(nameof(Lottery.LotteryType), "نوع");
                DisplayColumn(nameof(Lottery.FromDate), "از تاریخ");
                DisplayColumn(nameof(Lottery.ToDate), "تا تاریخ");
                OrderByDesc(nameof(Lottery.FromDate)); // نزولی - جدیدترین
            }
        }

        /// <summary>
        /// گزارش مشارکت در قرعه‌کشی‌ها
        /// </summary>
        public partial class LotteryParticipationStatsConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "آمار مشارکت در قرعه‌کشی";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Lottery.Title), "قرعه‌کشی");
                Count(null, "تعداد شرکت‌کنندگان");
            }
        }

        /// <summary>
        /// گزارش قرعه‌کشی‌ها به تفکیک نوع
        /// </summary>
        public partial class LotteriesByTypeConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "قرعه‌کشی‌ها به تفکیک نوع";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Lottery.LotteryType), "نوع قرعه‌کشی");
                Count(null, "تعداد");
            }
        }
    }
}