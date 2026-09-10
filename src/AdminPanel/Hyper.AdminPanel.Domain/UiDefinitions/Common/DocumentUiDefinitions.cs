namespace Hyper.AdminPanel.Domain.UiDefinitions.Common;

public class DocumentUiDefinitions : CRUDDefinition
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager
    ];

    public override List<string>? Roles => DefaultRoles;

    public override Type DefinitionEntity => typeof(Document);
    protected override void IndexFormViewModel()
    {
        
        AddColumns(nameof(Document.DocumentType),
                        nameof(Document.SubjectTitle),
                        nameof(Document.SubjectId)
                        );
    }
}