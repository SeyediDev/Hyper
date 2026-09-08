namespace Hyper.AdminPanel.Domain.UiDefinitions.Points;

public class PointLevelUiDefinitions : SubCRUDDefinition<PointLevel>
{
    public override string? Icon => "fa fa-level-up";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(PointLevel.Title),
                        nameof(PointLevel.Key),
                        nameof(PointLevel.Point),
                        nameof(PointLevel.Level),
                        nameof(PointLevel.MinXp),
                        nameof(PointLevel.MaxXp)
                        );
        form.AddOrderBy(nameof(PointLevel.Point));
        form.AddOrderBy(nameof(PointLevel.Level));
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(
            nameof(PointLevel.Title),
            nameof(PointLevel.Key),
            nameof(PointLevel.Point),
            nameof(PointLevel.Level),
            nameof(PointLevel.MinXp),
            nameof(PointLevel.MaxXp)
            );
    }
    public override string SubjectId => "Sub";
    public override void SubIndexViewModel()
    {
        AddColumns(nameof(PointLevel.Title),
            nameof(PointLevel.Key),
            nameof(PointLevel.Point),
            nameof(PointLevel.Level),
            nameof(PointLevel.MinXp),
            nameof(PointLevel.MaxXp)
            );
        
        form.AddOrderBy(nameof(PointLevel.Point));
        form.AddOrderBy(nameof(PointLevel.Level));
    }

    public override void SubViewModel()
    {
        AddFields(
            nameof(PointLevel.Title),
            nameof(PointLevel.Key),
            nameof(PointLevel.Point),
            nameof(PointLevel.Level),
            nameof(PointLevel.MinXp),
            nameof(PointLevel.MaxXp)
            );
    }
}
