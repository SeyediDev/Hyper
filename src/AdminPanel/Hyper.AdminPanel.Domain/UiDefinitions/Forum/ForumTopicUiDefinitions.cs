namespace Hyper.AdminPanel.Domain.UiDefinitions.Forum;

public class ForumTopicUiDefinitions : CRUDDefinition<ForumTopic>
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
            nameof(ForumTopic.Tenant),
            nameof(ForumTopic.CreatorCustomerTenant),
            nameof(ForumTopic.Title),
            nameof(ForumTopic.Category),
            nameof(ForumTopic.Tags),
            nameof(ForumTopic.IsPinned),
            nameof(ForumTopic.IsClosed),
            nameof(ForumTopic.IsLocked),
            nameof(ForumTopic.ViewsCount),
            nameof(ForumTopic.PostsCount),
            nameof(ForumTopic.LikesCount),
            nameof(ForumTopic.CreateDate)
        );

        form.AddOrderBy(nameof(ForumTopic.CreateDate), SortType.Descending);
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(ForumTopic.Tenant));
        AddField(nameof(ForumTopic.CreatorCustomerTenant));
        AddField(nameof(ForumTopic.Title));
        AddField(nameof(ForumTopic.Content), eControlTypeId.MultilineTextInput);
        AddField(nameof(ForumTopic.Category));
        AddField(nameof(ForumTopic.Tags));
        AddField(nameof(ForumTopic.Product));
        AddField(nameof(ForumTopic.IsPinned));
        AddField(nameof(ForumTopic.IsClosed));
        AddField(nameof(ForumTopic.IsLocked));
        AddField(nameof(ForumTopic.ViewsCount), eControlPropertyId.ReadOnly);
        AddField(nameof(ForumTopic.PostsCount), eControlPropertyId.ReadOnly);
        AddField(nameof(ForumTopic.LikesCount), eControlPropertyId.ReadOnly);
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}
