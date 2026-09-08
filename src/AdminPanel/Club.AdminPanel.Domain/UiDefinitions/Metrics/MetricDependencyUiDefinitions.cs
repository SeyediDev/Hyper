namespace Hyper.AdminPanel.Domain.UiDefinitions.Metrics;

/// <summary>
/// UI Definition for MetricDependency - used as SubTable in MetricDefinition
/// </summary>
public partial class MetricDependencyUiDefinitions : CRUDDefinition<MetricDependency>
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
            nameof(MetricDependency.MetricDefinition),
            nameof(MetricDependency.DependsOnMetricDefinition),
            nameof(MetricDependency.IsRequired)
        );
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(
            nameof(MetricDependency.MetricDefinition),
            nameof(MetricDependency.DependsOnMetricDefinition),
            nameof(MetricDependency.IsRequired)
        );
    }
}

