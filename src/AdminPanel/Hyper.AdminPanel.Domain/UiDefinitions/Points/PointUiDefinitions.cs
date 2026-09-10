namespace Hyper.AdminPanel.Domain.UiDefinitions.Points;

public partial class PointUiDefinitions : CRUDDefinition<Point>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-star";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(Point.Tenant),
                        nameof(Point.Title),
                        nameof(Point.Key),
                        nameof(Point.PointType));
        form.AddOrderBy(nameof(Point.Tenant));
        form.AddOrderBy(nameof(Point.PointType));
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Point.Tenant)
                     , nameof(Point.Title)
                     , nameof(Point.Key)
                     , nameof(Point.PointType)
                     , nameof(Point.AutoVisit)
                     , nameof(Point.Visible)
                     , nameof(Point.ShowInLeaderboard)
                     , nameof(Point.Transferable)
                     , nameof(Point.HasExpiration)
                     , nameof(Point.ExpirationDays)
                     , nameof(Point.AllowNegativeBalance)
                        );
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;

        public class PointsByTypeConfig() : ChartConfigDefinition(ChartType.Pie)
        {
            protected override List<string> Roles => [Neo.Domain.Constants.Roles.Admin, HyperRoles.Manager];
            protected override string Name => "امتیازات به تفکیک نوع";

            protected override void DefineGroupBy()
            {
                GroupBy(nameof(Point.PointType), "نوع امتیاز");
                Count(null, "تعداد");
            }
        }
    }
}