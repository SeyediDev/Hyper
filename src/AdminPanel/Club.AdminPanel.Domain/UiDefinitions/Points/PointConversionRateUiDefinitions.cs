namespace Hyper.AdminPanel.Domain.UiDefinitions.Points;

public class PointConversionRateUiDefinitions : SubCRUDDefinition<PointConversionRate>
{
    public override string SubjectId => "Sub";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(PointConversionRate.FromPoint),
                        nameof(PointConversionRate.ToPoint),
                        nameof(PointConversionRate.ConversionRate),
                        nameof(PointConversionRate.CommissionPoint),
                        nameof(PointConversionRate.CommissionAmount),
                        nameof(PointConversionRate.IsActive)
                        );
        form.AddOrderBy(nameof(PointConversionRate.CreateDate));
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(PointConversionRate.FromPoint),
                       nameof(PointConversionRate.ToPoint),
                       nameof(PointConversionRate.ConversionRate),
                       nameof(PointConversionRate.CommissionPoint),
                       nameof(PointConversionRate.CommissionAmount),
                       nameof(PointConversionRate.IsActive)
                       );
    }

    public override void SubIndexViewModel()
    {
        AddColumns(nameof(PointConversionRate.FromPoint),
                        nameof(PointConversionRate.ToPoint),
                        nameof(PointConversionRate.ConversionRate),
                        nameof(PointConversionRate.CommissionPoint),
                        nameof(PointConversionRate.CommissionAmount),
                        nameof(PointConversionRate.IsActive)
                        );
    }

    public override void SubViewModel()
    {
        AddFields(nameof(PointConversionRate.FromPoint),
                       nameof(PointConversionRate.ToPoint),
                       nameof(PointConversionRate.ConversionRate),
                       nameof(PointConversionRate.CommissionPoint),
                       nameof(PointConversionRate.CommissionAmount),
                       nameof(PointConversionRate.IsActive)
                       );
    }
}

