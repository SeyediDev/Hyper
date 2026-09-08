using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.CustomerSegments.Enums;
using Hyper.Domain.Features.Customers;

namespace Hyper.CustomerPortal.Application.Features.Communities.Commands;

/// <summary>
/// درخواست عضویت در جامعه
/// </summary>
public record JoinCommunityCommand : IRequest<JoinCommunityCommandResponse>
{
    public int CommunityId { get; set; }
}

public record JoinCommunityCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public string? MembershipId { get; set; }
}

public class JoinCommunityCommandValidator : AbstractValidator<JoinCommunityCommand>
{
    public JoinCommunityCommandValidator()
    {
        RuleFor(x => x.CommunityId)
            .GreaterThan(0)
            .WithMessage("شناسه جامعه نامعتبر است");
    }
}

public class JoinCommunityCommandHandler(
    ICustomerSegmentService customerSegmentService,
    ICustomerRequesterUser requesterUser,
    IQueryRepository<Hyper.Domain.Entities.CustomerSegments.CustomerSegment, int> segmentRepo
) : IRequestHandler<JoinCommunityCommand, JoinCommunityCommandResponse>
{
    public async Task<JoinCommunityCommandResponse> Handle(
        JoinCommunityCommand request, 
        CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;

        // دریافت جامعه
        var segment = await segmentRepo.GetByIdAsync(request.CommunityId, cancellationToken);
        if (segment == null)
        {
            return new JoinCommunityCommandResponse
            {
                Success = false,
                Message = "جامعه مورد نظر یافت نشد"
            };
        }

        // بررسی قابلیت نمایش در پرتال
        if (!segment.IsVisibleInPortal)
        {
            return new JoinCommunityCommandResponse
            {
                Success = false,
                Message = "عضویت در این جامعه از طریق پرتال امکان‌پذیر نیست"
            };
        }

        // بررسی نحوه عضویت
        if (segment.JoinMode == CustomerSegmentJoinMode.SystemOnly)
        {
            return new JoinCommunityCommandResponse
            {
                Success = false,
                Message = "عضویت در این جامعه فقط توسط سامانه انجام می‌شود"
            };
        }

        // بررسی واجد شرایط بودن (در صورتی که نیاز به کنترل شرایط باشد)
        if (segment.JoinMode == CustomerSegmentJoinMode.WithConditionCheck)
        {
            var eligibility = await customerSegmentService.CheckCustomerEligibilityAsync(
                customerId, 
                request.CommunityId, null!/*TODO*/,
                cancellationToken);

            if (!eligibility.IsEligible)
            {
                return new JoinCommunityCommandResponse
                {
                    Success = false,
                    Message = $"شما واجد شرایط عضویت در این جامعه نیستید.{eligibility.Message}"
                };
            }

            if (eligibility.IsAlreadyMember)
            {
                return new JoinCommunityCommandResponse
                {
                    Success = false,
                    Message = "شما از قبل عضو این جامعه هستید"
                };
            }
        }

        // عضو کردن در جامعه
        var result = await customerSegmentService.JoinCustomerToSegmentAsync(
            customerId, 
            request.CommunityId, 
            eventLogId: null, // عضویت دستی
            cancellationToken);

        return new JoinCommunityCommandResponse
        {
            Success = result.Success,
            Message = result.Message ?? "عملیات انجام شد",
            MembershipId = result.MembershipId?.ToString()
        };
    }
}