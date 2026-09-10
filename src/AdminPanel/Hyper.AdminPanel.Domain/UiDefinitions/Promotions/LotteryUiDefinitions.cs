using Hyper.Domain.Entities.Lotteries;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public partial class LotteryUiDefinitions : CRUDDefinition<Lottery>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.MarketingManager
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(Lottery.Title),
                        nameof(Lottery.LotteryType),
                        nameof(Lottery.FromDate),
                        nameof(Lottery.ToDate),
                        nameof(Lottery.IsScheduled),
                        nameof(Lottery.Tenant),
                        nameof(Lottery.CustomerSegment)
                        );
        form.AddSubjectColumn<LotteryRelations>();
    }
    
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Lottery.Title),
                       nameof(Lottery.Tenant),
                       nameof(Lottery.CustomerSegment),
                       nameof(Lottery.LotteryType),
                       nameof(Lottery.FromDate),
                       nameof(Lottery.ToDate),
                       nameof(Lottery.IsScheduled),
                       nameof(Lottery.SchedulingKind),
                       nameof(Lottery.DayOfWeek),
                       nameof(Lottery.DayOfMonth),
                       nameof(Lottery.Month),
                       nameof(Lottery.DayOfYear),
                       nameof(Lottery.Hour),
                       nameof(Lottery.Minute)
                       );
    }
    
    public class LotteryRelations : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(LotteryRelations);
        public override string Name => "پاداش‌ها و شرکت‌کنندگان";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(Lottery.Title), eControlPropertyId.ReadOnly);
            AddSubTable(nameof(LotteryReward), nameof(LotteryReward.Lottery), "Sub",
                "پاداش‌ها", null, true, eControlTypeId.MultiTab);

            AddSubTable(nameof(LotteryParticipant), nameof(LotteryParticipant.Lottery), "Sub",
                "شرکت‌کنندگان", null, false, eControlTypeId.MultiTab);
        }
    }

    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        /// <summary>
        /// گزارش قرعه‌کشی‌های فعال
        /// </summary>
        public partial class ActiveLotteriesConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(Lottery.ToDate)} >= DateTime.Now";
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

