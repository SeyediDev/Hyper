namespace Hyper.AdminPanel.Domain.UiDefinitions.Feedback;

public class FeedbackCommentUiDefinitions : CRUDDefinition<FeedbackComment>
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
            nameof(FeedbackComment.Feedback),
            nameof(FeedbackComment.Customer),
            nameof(FeedbackComment.User),
            nameof(FeedbackComment.ParentComment),
            nameof(FeedbackComment.IsOfficial),
            nameof(FeedbackComment.CreateDate)
        );

        form.AddOrderBy(nameof(FeedbackComment.CreateDate), SortType.Descending);
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(FeedbackComment.Feedback));
        AddField(nameof(FeedbackComment.Customer));
        AddField(nameof(FeedbackComment.User));
        AddField(nameof(FeedbackComment.ParentComment));
        AddField(nameof(FeedbackComment.Content), eControlTypeId.MultilineTextInput);
        AddField(nameof(FeedbackComment.IsOfficial));
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}
