namespace Hyper.AdminPanel.Domain.UiDefinitions.Forum;

public class ForumTopicLikeUiDefinitions : CRUDDefinition<ForumTopicLike>
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
            nameof(ForumTopicLike.Topic),
            nameof(ForumTopicLike.CustomerTenant),
            nameof(ForumTopicLike.LikedDate),
            nameof(ForumTopicLike.CreateDate)
        );

        form.AddOrderBy(nameof(ForumTopicLike.CreateDate), SortType.Descending);
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(ForumTopicLike.Topic), eControlPropertyId.ReadOnly);
        AddField(nameof(ForumTopicLike.CustomerTenant), eControlPropertyId.ReadOnly);
        AddField(nameof(ForumTopicLike.LikedDate));
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}
