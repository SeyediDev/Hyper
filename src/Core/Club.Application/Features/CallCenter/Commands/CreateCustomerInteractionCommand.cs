namespace Hyper.Application.Features.CallCenter.Commands;

/// <summary>
/// دستور ایجاد تعامل جدید با مشتری
/// </summary>
public record CreateCustomerInteractionCommand : IRequest<long>
{
    public int CustomerTenantId { get; init; }
    public int InteractionTypeId { get; init; }
    public UserId? AgentUserId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime? StartTime { get; init; }
    public PriorityLevel Priority { get; init; } = PriorityLevel.Normal;
    public string? ContactPhone { get; init; }
    public string? ContactEmail { get; init; }
    public string? TicketId { get; init; }
    public string? CallId { get; init; }
}

public class CreateCustomerInteractionCommandValidator : AbstractValidator<CreateCustomerInteractionCommand>
{
    public CreateCustomerInteractionCommandValidator()
    {
        RuleFor(x => x.CustomerTenantId)
            .GreaterThan(0)
            .WithMessage("شناسه مشتری باید بزرگتر از صفر باشد");

        RuleFor(x => x.InteractionTypeId)
            .GreaterThan(0)
            .WithMessage("شناسه نوع تعامل باید بزرگتر از صفر باشد");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("عنوان نمی‌تواند بیشتر از 200 کاراکتر باشد");

        RuleFor(x => x.ContactPhone)
            .MaximumLength(20)
            .WithMessage("شماره تماس نمی‌تواند بیشتر از 20 کاراکتر باشد");

        RuleFor(x => x.ContactEmail)
            .MaximumLength(200)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.ContactEmail))
            .WithMessage("ایمیل معتبر نیست");
    }
}

internal sealed class CreateCustomerInteractionCommandHandler(
    ICommandRepository<CustomerInteraction, long> interactionRepository,
    IQueryRepository<CustomerTenant, int> customerTenantRepository,
    IQueryRepository<InteractionType, int> interactionTypeRepository,
    IHyperUnitOfWorkCommand unitOfWork,
    IRequesterUser requesterUser
) : IRequestHandler<CreateCustomerInteractionCommand, long>
{
    public async Task<long> Handle(CreateCustomerInteractionCommand request, CancellationToken cancellationToken)
    {
        var customerTenant = await customerTenantRepository.FirstOrDefaultAsync(
            x => x.Id == request.CustomerTenantId,
            cancellationToken) ?? throw new InvalidOperationException("مشتری یافت نشد");

        var interactionType = await interactionTypeRepository.FirstOrDefaultAsync(
            x => x.Id == request.InteractionTypeId && x.IsActive,
            cancellationToken) ?? throw new InvalidOperationException("نوع تعامل یافت نشد");

        var interaction = new CustomerInteraction
        {
            CustomerTenantId = request.CustomerTenantId,
            TenantId = customerTenant.TenantId,
            InteractionTypeId = request.InteractionTypeId,
            AgentUserId = request.AgentUserId ?? requesterUser.Id,
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime ?? DateTime.UtcNow,
            Status = InteractionStatus.InProgress,
            Priority = request.Priority,
            ContactPhone = request.ContactPhone,
            ContactEmail = request.ContactEmail,
            TicketId = request.TicketId,
            CallId = request.CallId
        };

        await interactionRepository.AddAsync(interaction);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return interaction.Id;
    }
}

