using Hyper.Domain.Entities.Lotteries;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Lotteries;

public partial class LotteryParticipantUiDefinitions : CRUDDefinition<LotteryParticipant>
{
    public override List<string>? Roles =>
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst
    ];
    public override string? Icon => "fa fa-user-plus";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(LotteryParticipant.Lottery),
                        nameof(LotteryParticipant.CustomerTenant),
                        nameof(LotteryParticipant.IsWinner),
                        nameof(LotteryParticipant.Reward),
                        nameof(LotteryParticipant.RewardAmount),
                        nameof(LotteryParticipant.ParticipatedAt),
                        nameof(LotteryParticipant.AnnouncedAt),
                        nameof(LotteryParticipant.IsRewardDistributed),
                        nameof(LotteryParticipant.RewardDistributedAt)
                        );
    }
    
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(LotteryParticipant.Lottery),
                       nameof(LotteryParticipant.CustomerTenant),
                       nameof(LotteryParticipant.IsWinner),
                       nameof(LotteryParticipant.Reward),
                       nameof(LotteryParticipant.RewardAmount),
                       nameof(LotteryParticipant.ParticipatedAt),
                       nameof(LotteryParticipant.AnnouncedAt),
                       nameof(LotteryParticipant.IsRewardDistributed),
                       nameof(LotteryParticipant.RewardDistributedAt),
                       nameof(LotteryParticipant.RewardAsset)
                       );
    }
    // =====================================================
    // Public Reports
    // =====================================================

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
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
                DisplayColumn(nameof(LotteryParticipant.Reward), "پاداش");
                DisplayColumn(nameof(LotteryParticipant.RewardAmount), "مبلغ");
                DisplayColumn(nameof(LotteryParticipant.AnnouncedAt), "تاریخ اعلام");
                DisplayColumn(nameof(LotteryParticipant.IsRewardDistributed), "توزیع شده");
                
                OrderByDesc(nameof(LotteryParticipant.AnnouncedAt)); // نزولی - جدیدترین
            }
        }

        /// <summary>
        /// گزارش پاداش‌های توزیع نشده
        /// </summary>
        public partial class UndistributedRewardsConfig : ReportConfigDefinition
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string WhereCondition => $"{nameof(LotteryParticipant.IsWinner)} == true && {nameof(LotteryParticipant.IsRewardDistributed)} == false";
            protected override string Name => "پاداش‌های توزیع نشده";
            
            protected override void DefineColumns()
            {
                DisplayColumn(nameof(LotteryParticipant.Lottery), "قرعه‌کشی");
                DisplayColumn(nameof(LotteryParticipant.CustomerTenant), "مشتری");
                DisplayColumn(nameof(LotteryParticipant.Reward), "پاداش");
                DisplayColumn(nameof(LotteryParticipant.RewardAmount), "مبلغ");
                DisplayColumn(nameof(LotteryParticipant.AnnouncedAt), "تاریخ اعلام");
                
                OrderBy(nameof(LotteryParticipant.AnnouncedAt)); // صعودی - قدیمی‌ترین (اولویت توزیع)
            }
        }
    }
}