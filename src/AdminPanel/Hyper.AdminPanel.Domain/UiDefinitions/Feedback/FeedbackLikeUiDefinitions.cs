namespace Hyper.AdminPanel.Domain.UiDefinitions.Feedback;

public class FeedbackLikeUiDefinitions : CRUDDefinition<FeedbackLike>
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
            nameof(FeedbackLike.Feedback),
            nameof(FeedbackLike.CustomerTenant),
            nameof(FeedbackLike.LikedDate),
            nameof(FeedbackLike.CreateDate)
        );

        form.AddOrderBy(nameof(FeedbackLike.CreateDate), SortType.Descending);
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(FeedbackLike.Feedback), eControlPropertyId.ReadOnly);
        AddField(nameof(FeedbackLike.CustomerTenant), eControlPropertyId.ReadOnly);
        AddField(nameof(FeedbackLike.LikedDate));
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}
