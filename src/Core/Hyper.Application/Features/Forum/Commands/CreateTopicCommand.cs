namespace Hyper.Application.Features.Forum.Commands;

/// <summary>
/// ایجاد موضوع جدید در انجمن
/// </summary>
public record CreateTopicCommand : IRequest<int>
{
    [Required]
    public int CustomerTenantId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;
    
    [Required]
    [MaxLength(10000)]
    public string Content { get; set; } = null!;
    
    [MaxLength(100)]
    public string? Category { get; set; }
    
    [MaxLength(500)]
    public string? Tags { get; set; }
    
    public int? ProductId { get; set; }
}

public class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.CustomerTenantId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty().MaximumLength(10000);
    }
}

public class CreateTopicCommandHandler(
    IHyperUnitOfWorkCommand unitOfWork
) : IRequestHandler<CreateTopicCommand, int>
{
    public async Task<int> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        const long topicPoints = 15;

        var customerTenantRepo = unitOfWork.Repository<CustomerTenant, int>();
        CustomerTenant? customerTenant = await customerTenantRepo.FirstOrDefaultAsync(
            ct => ct.Id == request.CustomerTenantId,
            cancellationToken) ?? throw new System.ComponentModel.DataAnnotations.ValidationException("رابطه مشتری-اکوسیستم یافت نشد");

        var topic = new ForumTopic
        {
            TenantId = customerTenant.TenantId,
            CreatorCustomerTenantId = customerTenant.Id,
            CreatorCustomerTenant = customerTenant,
            Title = request.Title,
            Content = request.Content,
            Category = request.Category,
            Tags = request.Tags,
            ProductId = request.ProductId,
            ViewsCount = 0,
            PostsCount = 0,
            LikesCount = 0
        };

        unitOfWork.Repository<ForumTopic, int>().Add(topic);

        customerTenant.CurrentPointsBalance = (customerTenant.CurrentPointsBalance ?? 0) + topicPoints;
        customerTenant.TotalPointsEarned = (customerTenant.TotalPointsEarned ?? 0) + topicPoints;
        customerTenantRepo.Update(customerTenant);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return topic.Id;
    }
}



