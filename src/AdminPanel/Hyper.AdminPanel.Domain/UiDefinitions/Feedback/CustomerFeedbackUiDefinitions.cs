namespace Hyper.AdminPanel.Domain.UiDefinitions.Feedback;

public class CustomerFeedbackUiDefinitions : CRUDDefinition<CustomerFeedback>
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
            nameof(CustomerFeedback.Tenant),
            nameof(CustomerFeedback.CustomerTenant),
            nameof(CustomerFeedback.FeedbackType),
            nameof(CustomerFeedback.Title),
            nameof(CustomerFeedback.Status),
            nameof(CustomerFeedback.Priority),
            nameof(CustomerFeedback.SatisfactionScore),
            nameof(CustomerFeedback.IsPublic),
            nameof(CustomerFeedback.AssignedToUser),
            nameof(CustomerFeedback.ResponseDate),
            nameof(CustomerFeedback.ResolvedDate),
            nameof(CustomerFeedback.LikesCount),
            nameof(CustomerFeedback.CommentsCount),
            nameof(CustomerFeedback.CreateDate)
        );

        form.AddOrderBy(nameof(CustomerFeedback.CreateDate), SortType.Descending);
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(CustomerFeedback.Tenant));
        AddField(nameof(CustomerFeedback.CustomerTenant));
        AddField(nameof(CustomerFeedback.FeedbackType));
        AddField(nameof(CustomerFeedback.Title));
        AddField(nameof(CustomerFeedback.Content), eControlTypeId.MultilineTextInput);
        AddField(nameof(CustomerFeedback.Product));
        AddField(nameof(CustomerFeedback.Category));
        AddField(nameof(CustomerFeedback.Status));
        AddField(nameof(CustomerFeedback.Priority));
        AddField(nameof(CustomerFeedback.SatisfactionScore));
        AddField(nameof(CustomerFeedback.IsPublic));
        AddField(nameof(CustomerFeedback.AssignedToUser));
        AddField(nameof(CustomerFeedback.Response), eControlTypeId.MultilineTextInput);
        AddField(nameof(CustomerFeedback.ResponseDate));
        AddField(nameof(CustomerFeedback.ResolvedDate));
        AddField(nameof(CustomerFeedback.InternalNotes), eControlTypeId.MultilineTextInput);
        AddField(nameof(CustomerFeedback.PointsEarned));
        AddField(nameof(CustomerFeedback.Tags));
        AddField(nameof(CustomerFeedback.LikesCount), eControlPropertyId.ReadOnly);
        AddField(nameof(CustomerFeedback.CommentsCount), eControlPropertyId.ReadOnly);
    }

    public new partial class PublicReport : CRUDDefinition.PublicReport
    {
        public override List<string>? Roles => DefaultRoles;
    }
}
