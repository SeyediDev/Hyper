using Hyper.Domain.Entities.Metrics.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Metrics;

public partial class MetricValueUiDefinitions : CRUDDefinition<MetricValue>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(MetricValue.Tenant),
            nameof(MetricValue.MetricDefinition),
            nameof(MetricValue.CustomerTenant),
            nameof(MetricValue.Product),
            nameof(MetricValue.DecimalValue),
            nameof(MetricValue.IntegerValue),
            nameof(MetricValue.StringValue),
            nameof(MetricValue.BooleanValue),
            nameof(MetricValue.CalculatedAt)
        );
        form.AddOrderBy(nameof(MetricValue.CalculatedAt), SortType.Descending);
        form.AddOrderBy(nameof(MetricValue.TenantId));
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(
            nameof(MetricValue.Tenant),
            nameof(MetricValue.MetricDefinition),
            nameof(MetricValue.CustomerTenant),
            nameof(MetricValue.Product),
            nameof(MetricValue.DecimalValue),
            nameof(MetricValue.IntegerValue),
            nameof(MetricValue.StringValue),
            nameof(MetricValue.BooleanValue),
            nameof(MetricValue.CalculatedAt),
            nameof(MetricValue.PeriodStart),
            nameof(MetricValue.PeriodEnd),
            nameof(MetricValue.CalculationMetadata)
        );
    }

    /// <summary>
    /// گزارش عمومی مقادیر شاخص‌ها
    /// </summary>
    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}

