using Hyper.Domain.Entities.Metrics.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Metrics;

public partial class MetricDefinitionUiDefinitions : CRUDDefinition<MetricDefinition>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-line-chart";

    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(MetricDefinition.Tenant),
            nameof(MetricDefinition.Code),
            nameof(MetricDefinition.Name),
            nameof(MetricDefinition.MetricCategory),
            nameof(MetricDefinition.SubCategory),
            nameof(MetricDefinition.Level),
            nameof(MetricDefinition.DataType),
            nameof(MetricDefinition.Unit),
            nameof(MetricDefinition.IsActive)
        );
        form.AddOrderBy(nameof(MetricDefinition.TenantId));
        form.AddOrderBy(nameof(MetricDefinition.MetricCategory));
        AddSubjectColumn<FormulaConfiguration>();
        AddSubjectColumn<DependenciesConfiguration>();
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(
            nameof(MetricDefinition.Tenant),
            nameof(MetricDefinition.Code),
            nameof(MetricDefinition.Name),
            nameof(MetricDefinition.Description),
            nameof(MetricDefinition.MetricCategory),
            nameof(MetricDefinition.SubCategory),
            nameof(MetricDefinition.Level),
            nameof(MetricDefinition.DataType),
            nameof(MetricDefinition.Unit),
            nameof(MetricDefinition.MinValue),
            nameof(MetricDefinition.MaxValue),
            nameof(MetricDefinition.IsActive)
        );

        form.AddSubTable(nameof(MetricDependency), nameof(MetricDependency.MetricDefinition), "Sub",
            "وابستگی‌های شاخص", null, false, ContainerControl.MultiTab);
        form.AddSubTable(nameof(MetricValue), nameof(MetricValue.MetricDefinition), "Sub",
            "مقادیر محاسبه شده", null, false, ContainerControl.MultiTab);
    }

    /// <summary>
    /// تنظیمات فرمول
    /// </summary>
    public class FormulaConfiguration : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(FormulaConfiguration);
        public override string Name => "تنظیمات فرمول";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddField(nameof(MetricDefinition.Tenant), eControlPropertyId.ReadOnly);
            AddField(nameof(MetricDefinition.Code), eControlPropertyId.ReadOnly);
			AddField(nameof(MetricDefinition.Name), eControlPropertyId.ReadOnly);
			AddField(nameof(MetricDefinition.FormulaType));
            AddField(nameof(MetricDefinition.Formula));
            AddField(nameof(MetricDefinition.Metadata));
            AddField(nameof(MetricDefinition.CalculationIntervalHours));
            AddField(nameof(MetricDefinition.IsRealTime));
        }
    }

    /// <summary>
    /// تنظیمات وابستگی‌ها
    /// </summary>
    public class DependenciesConfiguration : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(DependenciesConfiguration);
        public override string Name => "وابستگی‌ها";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            // وابستگی‌ها از طریق SubTable مدیریت می‌شوند
            AddField(nameof(MetricDefinition.Code), eControlPropertyId.ReadOnly);
            AddField(nameof(MetricDefinition.Name), eControlPropertyId.ReadOnly);
        }
    }

    /// <summary>
    /// گزارش عمومی تعریف شاخص‌ها
    /// </summary>
    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}

