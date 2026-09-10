namespace Hyper.AdminPanel.Domain.UiDefinitions.Forum;

public class ForumPostUiDefinitions : CRUDDefinition<ForumPost>
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
            nameof(ForumPost.Topic),
            nameof(ForumPost.CustomerTenant),
            nameof(ForumPost.ParentPost),
            nameof(ForumPost.IsBestAnswer),
            nameof(ForumPost.IsApproved),
            nameof(ForumPost.LikesCount),
            nameof(ForumPost.PointsEarned),
            nameof(ForumPost.CreateDate)
        );

        form.AddOrderBy(nameof(ForumPost.CreateDate), SortType.Descending);
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(ForumPost.Topic));
        AddField(nameof(ForumPost.CustomerTenant));
        AddField(nameof(ForumPost.ParentPost));
        AddField(nameof(ForumPost.Content), eControlTypeId.MultilineTextInput);
        AddField(nameof(ForumPost.IsBestAnswer));
        AddField(nameof(ForumPost.IsApproved));
        AddField(nameof(ForumPost.PointsEarned));
        AddField(nameof(ForumPost.LikesCount), eControlPropertyId.ReadOnly);
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}
