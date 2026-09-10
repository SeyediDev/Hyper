using Hyper.Domain.Entities.Lotteries;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public partial class LotteryParticipantUiDefinitions : CRUDDefinition<LotteryParticipant>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel(FormDefinition form)
    {
        form.AddColumns(nameof(LotteryParticipant.Lottery),
                        nameof(LotteryParticipant.CustomerTenant),
                        nameof(LotteryParticipant.IsWinner),
                        nameof(LotteryParticipant.Award),
                        nameof(LotteryParticipant.AwardAmount),
                        nameof(LotteryParticipant.ParticipatedAt),
                        nameof(LotteryParticipant.AnnouncedAt),
                        nameof(LotteryParticipant.IsAwardDistributed),
                        nameof(LotteryParticipant.AwardDistributedAt)
                        );
    }
    
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(LotteryParticipant.Lottery),
                       nameof(LotteryParticipant.CustomerTenant),
                       nameof(LotteryParticipant.IsWinner),
                       nameof(LotteryParticipant.Award),
                       nameof(LotteryParticipant.AwardAmount),
                       nameof(LotteryParticipant.ParticipatedAt),
                       nameof(LotteryParticipant.AnnouncedAt),
                       nameof(LotteryParticipant.IsAwardDistributed),
                       nameof(LotteryParticipant.AwardDistributedAt),
                       nameof(LotteryParticipant.AwardAsset)
                       );
    }
    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        // =====================================================
        // Lottery Participation Reports
        // =====================================================

        /// <summary>
        /// گزارش مشارکت در قرعه‌کشی‌ها
        /// </summary>
        public partial class LotteryParticipationConfig() : ChartConfigDefinition(ChartType.Bar)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Analyst];
            protected override string Name => "مشارکت در قرعه‌کشی";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(LotteryParticipant.Lottery), "قرعه‌کشی");
                Count(null, "تعداد شرکت‌کنندگان");
                OrderByDesc("COUNT"); // نزولی - پرمشارکت‌ترین
            }
        }

        /// <summary>
        /// گزارش برندگان قرعه‌کشی
        /// </summary>
        public partial class LotteryWinnersConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(LotteryParticipant.IsWinner)} == true";
            protected override string Name => "برندگان قرعه‌کشی";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(LotteryParticipant.Lottery), "قرعه‌کشی");
                DisplayColumn(nameof(LotteryParticipant.CustomerTenant), "مشتری");
                DisplayColumn(nameof(LotteryParticipant.Award), "پاداش");
                DisplayColumn(nameof(LotteryParticipant.AwardAmount), "مبلغ");
                DisplayColumn(nameof(LotteryParticipant.AnnouncedAt), "تاریخ اعلام");
                DisplayColumn(nameof(LotteryParticipant.IsAwardDistributed), "توزیع شده");
                
                OrderByDesc(nameof(LotteryParticipant.AnnouncedAt)); // نزولی - جدیدترین
            }
        }

        /// <summary>
        /// گزارش پاداش‌های توزیع نشده
        /// </summary>
        public partial class UndistributedAwardsConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(LotteryParticipant.IsWinner)} == true && {nameof(LotteryParticipant.IsAwardDistributed)} == false";
            protected override string Name => "پاداش‌های توزیع نشده";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(LotteryParticipant.Lottery), "قرعه‌کشی");
                DisplayColumn(nameof(LotteryParticipant.CustomerTenant), "مشتری");
                DisplayColumn(nameof(LotteryParticipant.Award), "پاداش");
                DisplayColumn(nameof(LotteryParticipant.AwardAmount), "مبلغ");
                DisplayColumn(nameof(LotteryParticipant.AnnouncedAt), "تاریخ اعلام");
                
                OrderBy(nameof(LotteryParticipant.AnnouncedAt)); // صعودی - قدیمی‌ترین (اولویت توزیع)
            }
        }
    }
}
