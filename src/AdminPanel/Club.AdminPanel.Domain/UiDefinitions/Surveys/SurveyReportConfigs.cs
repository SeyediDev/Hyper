namespace Hyper.AdminPanel.Domain.UiDefinitions.Surveys;

public partial class SurveyUiDefinitions
{
    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        /// <summary>
        /// نظرسنجی‌های فعال
        /// </summary>
        public partial class ActiveSurveysConfig : ReportConfigDefinition
        {
            protected override string Name => "نظرسنجی‌های فعال";
            

            protected override string WhereCondition => $"{nameof(Survey.IsActive)} == true";

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Survey.Title));
                DisplayColumn(nameof(Survey.SurveyType));
                DisplayColumn(nameof(Survey.StartDate));
                DisplayColumn(nameof(Survey.EndDate));
                DisplayColumn(nameof(Survey.TotalParticipants));
                DisplayColumn(nameof(Survey.ParticipationPoints));
            }
        }

        /// <summary>
        /// نظرسنجی‌های محبوب (بر اساس تعداد شرکت‌کنندگان)
        /// </summary>
        public partial class TopSurveysConfig : ReportConfigDefinition
        {
            protected override string Name => "نظرسنجی‌های محبوب";
            

            protected override void DefineColumns()
            {
                DisplayColumn(nameof(Survey.Title));
                DisplayColumn(nameof(Survey.SurveyType));
                DisplayColumn(nameof(Survey.TotalParticipants));
                DisplayColumn(nameof(Survey.ParticipationPoints));
            }

            protected override void DefineOrderBy()
            {
                OrderByDesc(nameof(Survey.TotalParticipants));
            }
        }

        /// <summary>
        /// توزیع نظرسنجی‌ها بر اساس نوع
        /// </summary>
        public partial class SurveyTypeDistributionConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override string Name => "توزیع بر اساس نوع";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Survey.SurveyType));
                Count();
            }
        }

        /// <summary>
        /// میزان مشارکت در نظرسنجی‌ها در طول زمان
        /// </summary>
        public partial class SurveyParticipationTrendConfig() : ChartConfigDefinition(ChartType.Line)
        {
            protected override string Name => "روند مشارکت در نظرسنجی‌ها";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Survey.CreateDate));
                Count();
            }
        }

        /// <summary>
        /// نظرسنجی‌های بر اساس محصول
        /// </summary>
        public partial class SurveysByProductConfig() : GroupByConfigDefinition
        {
            protected override string Name => "نظرسنجی‌ها بر اساس محصول";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Survey.Product));
                Count();
            }
        }

        /// <summary>
        /// میانگین مشارکت در نظرسنجی‌ها
        /// </summary>
        public partial class AverageParticipationConfig() : ChartConfigDefinition(ChartType.MetricBox)
        {
            protected override string Name => "میانگین مشارکت";

            protected override void DefineGroupBy()
            {
                Average(nameof(Survey.TotalParticipants));
            }
        }
    }
}
