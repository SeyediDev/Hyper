namespace Hyper.Application.Features.Feedback.Commands;

/// <summary>
/// ثبت بازخورد مشتری
/// </summary>
public record SubmitFeedbackCommand : IRequest<int>
{
    [Required]
    public int CustomerTenantId { get; set; }
    
    public FeedbackType FeedbackType { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;
    
    [Required]
    [MaxLength(4000)]
    public string Content { get; set; } = null!;
    
    public int? ProductId { get; set; }
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    public int? SatisfactionScore { get; set; }
    
    public bool IsPublic { get; set; } = false;
    
    [MaxLength(500)]
    public string? Tags { get; set; }
}

public class SubmitFeedbackCommandValidator : AbstractValidator<SubmitFeedbackCommand>
{
    public SubmitFeedbackCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.CustomerTenantId).NotEmpty().WithMessage("رابطه مشتری-اکوسیستم الزامی است");
        RuleFor(x => x.Title).NotEmpty().WithMessage("عنوان الزامی است")
            .MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty().WithMessage("متن بازخورد الزامی است")
            .MaximumLength(4000);
        RuleFor(x => x.SatisfactionScore)
            .InclusiveBetween(1, 5).When(x => x.SatisfactionScore.HasValue)
            .WithMessage("امتیاز رضایت باید بین 1 تا 5 باشد");
    }
}

public class SubmitFeedbackCommandHandler(
    IHyperUnitOfWorkCommand unitOfWork
) : IRequestHandler<SubmitFeedbackCommand, int>
{
    public async Task<int> Handle(SubmitFeedbackCommand request, CancellationToken cancellationToken)
    {
        const long participationPoints = 10;

        var customerTenantRepo = unitOfWork.Repository<CustomerTenant, int>();
        CustomerTenant? customerTenant = await customerTenantRepo.FirstOrDefaultAsync(
            ct => ct.Id == request.CustomerTenantId,
            cancellationToken) ?? throw new System.ComponentModel.DataAnnotations.ValidationException("رابطه مشتری-اکوسیستم یافت نشد");

        var feedback = new CustomerFeedback
        {
            TenantId = customerTenant.TenantId,
            CustomerTenantId = customerTenant.Id,
            CustomerTenant = customerTenant,
            FeedbackType = request.FeedbackType,
            Title = request.Title,
            Content = request.Content,
            ProductId = request.ProductId,
            Category = request.Category,
            SatisfactionScore = request.SatisfactionScore,
            IsPublic = request.IsPublic,
            Tags = request.Tags,
            Status = FeedbackStatus.New,
            Priority = DeterminePriority(request.FeedbackType),
            PointsEarned = participationPoints,
            LikesCount = 0,
            CommentsCount = 0
        };

        unitOfWork.Repository<CustomerFeedback, int>().Add(feedback);

        if (participationPoints > 0)
        {
            customerTenant.CurrentPointsBalance = (customerTenant.CurrentPointsBalance ?? 0) + participationPoints;
            customerTenant.TotalPointsEarned = (customerTenant.TotalPointsEarned ?? 0) + participationPoints;
            customerTenantRepo.Update(customerTenant);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return feedback.Id;
    }

    private static FeedbackPriority DeterminePriority(FeedbackType type)
    {
        return type switch
        {
            FeedbackType.Complaint => FeedbackPriority.High,
            FeedbackType.Criticism => FeedbackPriority.Medium,
            FeedbackType.Question => FeedbackPriority.Medium,
            _ => FeedbackPriority.Low
        };
    }
}



