namespace Hyper.AdminPanel.Domain.UiDefinitions.Geography;

public class ProvinceUiDefinitions : CRUDDefinition<Province>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(Province.Country),
                        nameof(Province.Title),
                        nameof(Province.EnglishTitle),
                        nameof(Province.Iso2),
                        nameof(Province.CreateDate));
        form.AddOrderBy(nameof(Province.Country));
        form.AddOrderBy(nameof(Province.Title));
        AddSubjectColumn<ProvinceCities>();
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Province.Country),
                       nameof(Province.Title),
                       nameof(Province.EnglishTitle),
                       nameof(Province.Iso2),
                       nameof(Province.Latitude),
                       nameof(Province.Longitude),
                       nameof(Province.Pop),
                       nameof(Province.Tam),
                       nameof(Province.Sam),
                       nameof(Province.Som));
    }

    public class ProvinceCities : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(ProvinceCities);
        public override string Name => "شهرها";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddSubTable(nameof(City), nameof(City.Province), null,
                "شهرها", null, false, ContainerControl.MultiTab);
        }
    }
}