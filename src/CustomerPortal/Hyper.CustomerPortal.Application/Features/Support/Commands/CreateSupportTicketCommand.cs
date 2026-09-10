using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.CallCenter;
using Hyper.Domain.Entities.CallCenter.Enums;
using Hyper.Domain.Enums;

namespace Hyper.CustomerPortal.Application.Features.Support.Commands;

public record CreateSupportTicketCommand : IRequest<CreateSupportTicketCommandResponse>
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int InteractionTypeId { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Normal;
}

public record CreateSupportTicketCommandResponse
{
    public long TicketId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
}

public class CreateSupportTicketCommandValidator : AbstractValidator<CreateSupportTicketCommand>
{
    public CreateSupportTicketCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("توضیحات الزامی است")
            .MaximumLength(4000).WithMessage("توضیحات نمی‌تواند بیشتر از 4000 کاراکتر باشد");

        RuleFor(x => x.InteractionTypeId)
            .GreaterThan(0).WithMessage("نوع تعامل الزامی است");

        RuleFor(x => x.ContactPhone)
            .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.ContactPhone))
            .WithMessage("شماره تماس نمی‌تواند بیشتر از 20 کاراکتر باشد");

        RuleFor(x => x.ContactEmail)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.ContactEmail))
            .WithMessage("ایمیل معتبر نیست")
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.ContactEmail))
            .WithMessage("ایمیل نمی‌تواند بیشتر از 200 کاراکتر باشد");
    }
}

public class CreateSupportTicketCommandHandler(
    ICustomerRequesterUser requesterUser,
    ICommandRepository<CustomerInteraction, long> interactionRepository,
    IQueryRepository<CustomerTenant> customerTenantRepository,
    IQueryRepository<InteractionType> interactionTypeRepository) : IRequestHandler<CreateSupportTicketCommand, CreateSupportTicketCommandResponse>
{
    public async Task<CreateSupportTicketCommandResponse> Handle(CreateSupportTicketCommand request, CancellationToken cancellationToken)
    {
        var userId = requesterUser.CustomerId;
        var tenantId = requesterUser.TenantId??0;

        var customerTenant = await customerTenantRepository
            .FirstOrDefaultAsync(ct => ct.CustomerId == userId && ct.TenantId == tenantId && !ct.IsDeleted, cancellationToken);

        if (customerTenant == null)
        {
            return new CreateSupportTicketCommandResponse
            {
                Success = false,
                Message = "مشتری یافت نشد"
            };
        }

        var interactionType = await interactionTypeRepository
            .FirstOrDefaultAsync(it => it.Id == request.InteractionTypeId && !it.IsDeleted, cancellationToken);

        if (interactionType == null)
        {
            return new CreateSupportTicketCommandResponse
            {
                Success = false,
                Message = "نوع تعامل یافت نشد"
            };
        }

        var interaction = new CustomerInteraction
        {
            CustomerTenantId = customerTenant.Id,
            TenantId = tenantId,
            InteractionTypeId = request.InteractionTypeId,
            Title = request.Title,
            Description = request.Description,
            StartTime = DateTime.UtcNow,
            Status = InteractionStatus.InProgress,
            Priority = request.Priority,
            ContactPhone = request.ContactPhone,
            ContactEmail = request.ContactEmail
        };

        await interactionRepository.AddAsync(interaction, cancellationToken);
        await interactionRepository.SaveChangesAsync(cancellationToken);

        return new CreateSupportTicketCommandResponse
        {
            TicketId = interaction.Id,
            Success = true,
            Message = "تیکت پشتیبانی با موفقیت ایجاد شد"
        };
    }
}

