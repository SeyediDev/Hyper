namespace Hyper.AdminPanel.Domain.UiDefinitions.Geography;

public class CityUiDefinitions : CRUDDefinition<City>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-map-marker";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(City.Country),
                        nameof(City.Province),
                        nameof(City.Title),
                        nameof(City.EnglishTitle));
        form.AddOrderBy(nameof(City.Country));
        form.AddOrderBy(nameof(City.Province));
        form.AddOrderBy(nameof(City.Title));
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(City.Country),
                       nameof(City.Province),
                       nameof(City.Title),
                       nameof(City.EnglishTitle),
                       nameof(City.Iso2),
                       nameof(City.Latitude),
                       nameof(City.Longitude),
                       nameof(City.Pop),
                       nameof(City.Tam),
                       nameof(City.Sam),
                       nameof(City.Som));
    }
}