using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.Feedback;

namespace Hyper.CustomerPortal.Application.Features.Feedback.Commands;

public record CreateFeedbackCommand : IRequest<CreateFeedbackCommandResponse>
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public FeedbackType FeedbackType { get; set; }
    public int? ProductId { get; set; }
    public string? Category { get; set; }
    public int? SatisfactionScore { get; set; }
    public bool IsPublic { get; set; } = false;
    public string? Tags { get; set; }
}

public record CreateFeedbackCommandResponse
{
    public int FeedbackId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
}

public class CreateFeedbackCommandValidator : AbstractValidator<CreateFeedbackCommand>
{
    public CreateFeedbackCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان بازخورد الزامی است")
            .MaximumLength(200).WithMessage("عنوان نمی‌تواند بیشتر از 200 کاراکتر باشد");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("متن بازخورد الزامی است")
            .MaximumLength(4000).WithMessage("متن بازخورد نمی‌تواند بیشتر از 4000 کاراکتر باشد");

        RuleFor(x => x.SatisfactionScore)
            .InclusiveBetween(1, 5).When(x => x.SatisfactionScore.HasValue)
            .WithMessage("امتیاز رضایت باید بین 1 تا 5 باشد");
    }
}

public class CreateFeedbackCommandHandler(
    ICustomerRequesterUser requesterUser,
    ICommandRepository<CustomerFeedback> feedbackRepository,
	IQueryRepository<CustomerTenant> customerTenantRepository) 
    : IRequestHandler<CreateFeedbackCommand, CreateFeedbackCommandResponse>
{
    public async Task<CreateFeedbackCommandResponse> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var tenantId = requesterUser.TenantId??0;

		var customerTenant = await customerTenantRepository
            .FirstOrDefaultAsync(ct => ct.CustomerId == customerId && ct.TenantId == tenantId && !ct.IsDeleted, cancellationToken);

        if (customerTenant == null)
        {
            return new CreateFeedbackCommandResponse
            {
                Success = false,
                Message = "مشتری یافت نشد"
            };
        }

        var feedback = new CustomerFeedback
        {
            TenantId = tenantId,
            CustomerTenantId = customerTenant.Id,
            Title = request.Title,
            Content = request.Content,
            FeedbackType = request.FeedbackType,
            ProductId = request.ProductId,
            Category = request.Category,
            SatisfactionScore = request.SatisfactionScore,
            IsPublic = request.IsPublic,
            Tags = request.Tags,
            Status = FeedbackStatus.New,
            Priority = FeedbackPriority.Medium
        };

        await feedbackRepository.AddAsync(feedback, cancellationToken);
        await feedbackRepository.SaveChangesAsync(cancellationToken);

        return new CreateFeedbackCommandResponse
        {
            FeedbackId = feedback.Id,
            Success = true,
            Message = "بازخورد با موفقیت ثبت شد"
        };
    }
}

