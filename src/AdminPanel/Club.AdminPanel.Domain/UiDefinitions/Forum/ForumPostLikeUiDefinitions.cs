namespace Hyper.AdminPanel.Domain.UiDefinitions.Forum;

public class ForumPostLikeUiDefinitions : CRUDDefinition<ForumPostLike>
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
            nameof(ForumPostLike.Post),
            nameof(ForumPostLike.CustomerTenant),
            nameof(ForumPostLike.LikedDate),
            nameof(ForumPostLike.CreateDate)
        );

        form.AddOrderBy(nameof(ForumPostLike.CreateDate), SortType.Descending);
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(ForumPostLike.Post), eControlPropertyId.ReadOnly);
        AddField(nameof(ForumPostLike.CustomerTenant), eControlPropertyId.ReadOnly);
        AddField(nameof(ForumPostLike.LikedDate));
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}
