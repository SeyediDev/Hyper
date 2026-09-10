namespace Hyper.AdminPanel.Domain.UiDefinitions.Feedback;

public class FeedbackAttachmentUiDefinitions : CRUDDefinition<FeedbackAttachment>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.MarketingManager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(FeedbackAttachment.Feedback),
            nameof(FeedbackAttachment.Document),
            nameof(FeedbackAttachment.Description),
            nameof(FeedbackAttachment.CreateDate)
        );

        form.AddOrderBy(nameof(FeedbackAttachment.CreateDate), SortType.Descending);
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(FeedbackAttachment.Feedback));
        AddField(nameof(FeedbackAttachment.Document), eControlTypeId.File);
        AddField(nameof(FeedbackAttachment.Description));
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}
