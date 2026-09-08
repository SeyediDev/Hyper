namespace Hyper.Application.Features.Forum.Commands;

/// <summary>
/// ایجاد پست (پاسخ) در انجمن
/// </summary>
public record CreatePostCommand : IRequest<int>
{
    [Required]
    public int TopicId { get; set; }
    
    [Required]
    public int CustomerTenantId { get; set; }
    
    [Required]
    [MaxLength(10000)]
    public string Content { get; set; } = null!;
    
    public int? ParentPostId { get; set; }
}

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.TopicId).NotEmpty();
        RuleFor(x => x.CustomerTenantId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(10000);
    }
}

public class CreatePostCommandHandler(
    IHyperUnitOfWorkCommand unitOfWork
) : IRequestHandler<CreatePostCommand, int>
{
    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        const long postPoints = 5;

        var topicRepo = unitOfWork.Repository<ForumTopic, int>();
        var topic = await topicRepo.GetAsync(request.TopicId, cancellationToken);
        
        if (topic == null)
            throw new System.ComponentModel.DataAnnotations.ValidationException("موضوع یافت نشد");

        if (topic.IsLocked)
            throw new System.ComponentModel.DataAnnotations.ValidationException("موضوع قفل شده است");

		// اعطای امتیاز برای مشتری در اکوسیستم مربوط به موضوع
		var customerTenantRepo = unitOfWork.Repository<CustomerTenant, int>();
        CustomerTenant? customerTenant = await customerTenantRepo.FirstOrDefaultAsync(
            ct => ct.Id == request.CustomerTenantId,
            cancellationToken) ?? throw new System.ComponentModel.DataAnnotations.ValidationException("رابطه مشتری-اکوسیستم یافت نشد");

        if (customerTenant.TenantId != topic.TenantId)
        {
            throw new System.ComponentModel.DataAnnotations.ValidationException("مشتری متعلق به این انجمن نیست");
        }

        var post = new ForumPost
        {
            TopicId = request.TopicId,
            CustomerTenantId = customerTenant.Id,
            CustomerTenant = customerTenant,
            Content = request.Content,
            ParentPostId = request.ParentPostId,
            PointsEarned = postPoints,
            LikesCount = 0,
            IsApproved = true
        };

        unitOfWork.Repository<ForumPost, int>().Add(post);

        // به‌روزرسانی تعداد پست‌ها
        topic.PostsCount++;
        topicRepo.Update(topic);

        customerTenant.CurrentPointsBalance = (customerTenant.CurrentPointsBalance ?? 0) + postPoints;
        customerTenant.TotalPointsEarned = (customerTenant.TotalPointsEarned ?? 0) + postPoints;
        customerTenantRepo.Update(customerTenant);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return post.Id;
    }
}



