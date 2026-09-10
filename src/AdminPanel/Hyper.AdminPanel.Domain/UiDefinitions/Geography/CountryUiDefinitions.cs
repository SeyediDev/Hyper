namespace Hyper.AdminPanel.Domain.UiDefinitions.Geography;

public class CountryUiDefinitions : CRUDDefinition<Country>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(Country.Title),
                        nameof(Country.EnglishTitle),
                        nameof(Country.Iso2),
                        nameof(Country.Iso3),
                        nameof(Country.PhoneCode));
        form.AddOrderBy(nameof(Country.Title));
        AddSubjectColumn<CountryProvinces>();
        AddSubjectColumn<CountryCities>();
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Country.Title),
                       nameof(Country.EnglishTitle),
                       nameof(Country.Iso2),
                       nameof(Country.Iso3),
                       nameof(Country.IsoNumeric),
                       nameof(Country.PhoneCode),
                       nameof(Country.Latitude),
                       nameof(Country.Longitude),
                       nameof(Country.Pop),
                       nameof(Country.Tam),
                       nameof(Country.Sam),
                       nameof(Country.Som));
    }

    public class CountryProvinces : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(CountryProvinces);
        public override string Name => "استان‌ها";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddSubTable(nameof(Province), nameof(Province.Country), null, "استان‌ها", null, false, ContainerControl.None);
        }
    }

    public class CountryCities : SubjectEditForm2<CountryCities>
    {
        public override string Name => "شهرها";
        public override List<string>? Roles => DefaultRoles;

        protected override void ViewModel()
        {
            AddSubTable(nameof(City), nameof(City.Country), null, "شهرها", null, false, ContainerControl.None);
        }
    }
}