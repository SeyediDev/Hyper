using Hyper.Domain.Features.Channels;

namespace Hyper.Domain.Features.Customers;

/// <summary>
/// سرویس مدیریت جامعه/بازار مشتریان
/// شامل ارزیابی شرایط عضویت و مدیریت عضویت‌ها
/// </summary>
public interface ICustomerSegmentService
{
    /// <summary>
    /// بررسی واجد شرایط بودن مشتری برای عضویت در جامعه/بازار
    /// </summary>
    /// <param name="customerTenantId">شناسه مشتری</param>
    /// <param name="segmentId">شناسه جامعه/بازار</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>آیا مشتری واجد شرایط است</returns>
    [Telemetry]
    Task<CustomerSegmentEligibilityResult> CheckCustomerEligibilityAsync(
        int customerTenantId, int segmentId, AttributesValues attributes,
        CancellationToken cancellationToken);

    /// <summary>
    /// عضو کردن مشتری در جامعه/بازار
    /// </summary>
    /// <param name="customerTenantId">شناسه مشتری</param>
    /// <param name="segmentId">شناسه جامعه/بازار</param>
    /// <param name="eventLogId">شناسه لاگ رویداد (اختیاری)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>شناسه عضویت ایجاد شده</returns>
    [Telemetry]
    Task<CustomerSegmentMembershipResult> JoinCustomerToSegmentAsync(
        int customerTenantId, 
        int segmentId, 
        long? eventLogId,
        CancellationToken cancellationToken);

    /// <summary>
    /// دریافت جامعه‌ها/بازارهایی که مشتری واجد شرایط عضویت در آن‌ها است
    /// </summary>
    /// <param name="tenantId">اکوسیستم</param>
    /// <param name="customerTenantId">شناسه مشتری</param>
    /// <param name="onlyVisibleInPortal">فقط جامعه‌ها/بازارهای قابل نمایش در پرتال</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>لیست جامعه‌ها/بازارهای واجد شرایط</returns>
    [Telemetry]
    Task<List<CustomerSegmentEligibilityInfo>> GetEligibleSegmentsForCustomerAsync(
        int tenantId,
        int customerTenantId, 
        bool onlyVisibleInPortal,
        CancellationToken cancellationToken);

    /// <summary>
    /// بررسی خودکار و عضویت مشتری در جامعه‌ها/بازارهای مناسب پس از رویداد
    /// </summary>
    /// <param name="eventRequest"></param>
    /// <param name="eventResponse"></param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>تعداد جامعه‌ها/بازارهایی که مشتری به آن‌ها اضافه شد</returns>
    [Telemetry]
    Task<List<CustomerSegmentInfo>> AutoJoinCustomerToEligibleSegmentsAsync(
        EventRequest eventRequest, EventResponse eventResponse,
        CancellationToken cancellationToken);

    /// <summary>
    /// بررسی مجدد عضویت در جامعه‌ها/بازارها پس از تغییر پارامتر مشتری
    /// این متد فقط جامعه‌ها/بازارهایی که عضویت دستی نیستند را بررسی می‌کند
    /// </summary>
    /// <param name="customerTenantId">شناسه مشتری</param>
    /// <param name="eventLogId">شناسه لاگ رویداد</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه بررسی (تعداد عضویت‌های اضافه/حذف شده)</returns>
    [Telemetry]
    Task<SegmentMembershipRecheckResult> RecheckSegmentMembershipAfterParameterChangeAsync(
        int customerTenantId,
        long eventLogId,
        AttributesValues? attributes,
        CancellationToken cancellationToken);
}

/// <summary>
/// نتیجه بررسی واجد شرایط بودن برای عضویت در جامعه/بازار
/// </summary>
public record CustomerSegmentEligibilityResult
{
    /// <summary>
    /// آیا مشتری واجد شرایط است
    /// </summary>
    public bool IsEligible { get; init; }

    /// <summary>
    /// آیا مشتری از قبل عضو است
    /// </summary>
    public bool IsAlreadyMember { get; init; }
    public int? MemberId { get; init; }

    /// <summary>
    /// پیام توضیحی (در صورت عدم واجد شرایط بودن)
    /// </summary>
    public string? Message { get; init; }
}

/// <summary>
/// نتیجه بررسی مجدد عضویت در جامعه‌ها/بازارها
/// </summary>
public record SegmentMembershipRecheckResult
{
    /// <summary>
    /// تعداد عضویت‌های اضافه شده
    /// </summary>
    public int AddedMemberships { get; init; }

    /// <summary>
    /// تعداد عضویت‌های حذف شده
    /// </summary>
    public int RemovedMemberships { get; init; }

    /// <summary>
    /// لیست شناسه جامعه‌ها/بازارهایی که عضویت اضافه شد
    /// </summary>
    public List<int> AddedSegmentIds { get; init; } = [];

    /// <summary>
    /// لیست شناسه جامعه‌ها/بازارهایی که عضویت حذف شد
    /// </summary>
    public List<int> RemovedSegmentIds { get; init; } = [];
}

/// <summary>
/// نتیجه عملیات عضویت در جامعه/بازار
/// </summary>
public record CustomerSegmentMembershipResult
{
    /// <summary>
    /// آیا عملیات موفق بود
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// شناسه عضویت ایجاد شده
    /// </summary>
    public int? MembershipId { get; init; }

    /// <summary>
    /// پیام
    /// </summary>
    public string? Message { get; init; }
}

/// <summary>
/// اطلاعات جامعه/بازار واجد شرایط
/// </summary>
public record CustomerSegmentEligibilityInfo
{
    /// <summary>
    /// شناسه جامعه/بازار
    /// </summary>
    public int SegmentId { get; init; }

    /// <summary>
    /// عنوان جامعه/بازار
    /// </summary>
    public string Title { get; init; } = null!;

    /// <summary>
    /// توضیحات جامعه/بازار
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// آدرس تصویر
    /// </summary>
    public string? ImageUrl { get; init; }

    /// <summary>
    /// تعداد اعضا
    /// </summary>
    public int MemberCount { get; init; }

    /// <summary>
    /// آیا مشتری عضو است
    /// </summary>
    public bool IsMember { get; init; }

    /// <summary>
    /// نحوه عضویت
    /// </summary>
    public CustomerSegmentJoinMode JoinMode { get; init; }

    /// <summary>
    /// مزایا (JSON array deserialize شده)
    /// </summary>
    public string? Benefits { get; init; }
}

public record CustomerSegmentInfo(int Id, string Title, CustomerSegmentInfoType Type)
{
}
public enum CustomerSegmentInfoType { Join, Leave, Member }

/// <summary>
/// پیاده‌سازی سرویس مدیریت جامعه/بازار مشتریان
/// </summary>
internal class CustomerSegmentService(
      IEvaluateFormulaService evaluateFormulaService
    , IQueryRepository<CustomerSegment, int> segmentRepo
    , IQueryRepository<CustomerSegmentMembership, int> membershipQueryRepo
    , IQueryRepository<CustomerTenant, int> customerTenantQueryRepo
    , ICommandRepository<CustomerSegmentMembership, int> membershipCmdRepo
    ) : ICustomerSegmentService
{
    public async Task<CustomerSegmentEligibilityResult> CheckCustomerEligibilityAsync(
        int customerTenantId, int segmentId, AttributesValues? attributes,
        CancellationToken cancellationToken)
    {
        // دریافت جامعه/بازار
        var segment = await segmentRepo.GetByIdAsync(segmentId, cancellationToken);
        if (segment == null)
        {
            return new CustomerSegmentEligibilityResult
            {
                IsEligible = false,
                IsAlreadyMember = false,
                Message = "جامعه/بازار مورد نظر یافت نشد"
            };
        }

		// دریافت رابطه مشتری-اکوسیستم
		CustomerTenant? customerTenant = await customerTenantQueryRepo.GetByIdAsync(customerTenantId, cancellationToken);

        if (customerTenant == null || customerTenant.TenantId != segment.TenantId)
        {
            return new CustomerSegmentEligibilityResult
            {
                IsEligible = false,
                IsAlreadyMember = false,
                Message = "مشتری در این اکوسیستم عضو نیست"
			};
        }

        // بررسی عضویت فعلی
        var existingMembership = await membershipQueryRepo.FirstOrDefaultAsync(
            m => m.CustomerTenantId == customerTenant.Id && m.SegmentId == segmentId,
            cancellationToken);
        var isAlreadyMember = existingMembership != null;

        // بررسی فعال بودن جامعه/بازار
        if (!segment.IsActive)
        {
            return new CustomerSegmentEligibilityResult
            {
                IsEligible = false,
                IsAlreadyMember = isAlreadyMember,
                MemberId = existingMembership?.Id,
                Message = "جامعه/بازار غیرفعال است"
            };
        }

        var isEligible = false;
        // اگر شرط تعیین نشده باشد همیشه true است 
        if (!string.IsNullOrEmpty(segment.Constraint))
        {
            isEligible = true;
        }
        else
        {
            var evaluate = evaluateFormulaService.Evaluate(segment.Constraint!, attributes);
            isEligible = Convert.ToBoolean(evaluate);
        }

        return new CustomerSegmentEligibilityResult
        {
            IsEligible = isEligible,
            IsAlreadyMember = isAlreadyMember,
            MemberId = existingMembership?.Id,
            Message = isEligible ? "مشتری واجد شرایط عضویت است" : "مشتری واجد شرایط عضویت نیست",
        };
    }

    public async Task<CustomerSegmentMembershipResult> JoinCustomerToSegmentAsync(
        int customerTenantId,
        int segmentId,
        long? eventLogId,
        CancellationToken cancellationToken)
    {
        // دریافت جامعه/بازار
        var segment = await segmentRepo.GetByIdAsync(segmentId, cancellationToken);
        if (segment == null)
        {
            return new CustomerSegmentMembershipResult
            {
                Success = false,
                Message = "جامعه/بازار مورد نظر یافت نشد"
            };
        }

		// دریافت رابطه مشتری-اکوسیستم
		CustomerTenant? customerTenant = await customerTenantQueryRepo.GetByIdAsync(customerTenantId, cancellationToken);

        if (customerTenant == null)
        {
            return new CustomerSegmentMembershipResult
            {
                Success = false,
                Message = "مشتری در این اکوسیستم عضو نیست"
			};
        }

        // بررسی عضویت قبلی
        var existingMembership = await membershipQueryRepo.FirstOrDefaultAsync(
            m => m.CustomerTenantId == customerTenant.Id && m.SegmentId == segmentId,
            cancellationToken);

        if (existingMembership != null)
        {
            return new CustomerSegmentMembershipResult
            {
                Success = false,
                Message = "مشتری از قبل عضو این جامعه/بازار است"
            };
        }

        // ایجاد عضویت جدید
        var membership = new CustomerSegmentMembership
        {
            CustomerTenantId = customerTenant.Id,
            CustomerTenant = customerTenant,
            SegmentId = segmentId,
            EventLogId = eventLogId ?? 0 // اگر eventLogId null بود، 0 قرار می‌دهیم (برای عضویت‌های دستی)
        };

        membershipCmdRepo.Add(membership);
        await membershipCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new CustomerSegmentMembershipResult
        {
            Success = true,
            MembershipId = membership.Id,
            Message = $"عضویت در جامعه/بازار {segment.Title} با موفقیت انجام شد"
        };
    }

    public async Task<List<CustomerSegmentEligibilityInfo>>
        GetEligibleSegmentsForCustomerAsync(
        int tenantId,
        int customerTenantId,
        bool onlyVisibleInPortal,
        CancellationToken cancellationToken)
    {
        var result = new List<CustomerSegmentEligibilityInfo>();

        // دریافت جامعه‌ها/بازارهای فعال در سازمان‌های مشتری
        var segments = await segmentRepo.GetAllAsync(
            cancellationToken, s => s.TenantId== tenantId && s.IsActive && !s.IsDeleted);
        var activeSegments = segments
            .Where(s => !onlyVisibleInPortal || s.IsVisibleInPortal)
            .ToList();

        // دریافت عضویت‌های فعلی مشتری
        var memberships = await membershipQueryRepo.GetAllAsync(
            cancellationToken,
            m => m.CustomerTenantId== customerTenantId);
        var customerMembershipIds = memberships
            .Select(m => m.SegmentId)
            .ToHashSet();

        foreach (var segment in activeSegments)
        {
            var isMember = customerMembershipIds.Contains(segment.Id);
            
            // برای جامعه‌ها/بازارهای SystemOnly که کاربر عضو نیست، نمایش نمی‌دهیم
            if (segment.JoinMode == CustomerSegmentJoinMode.SystemOnly && !isMember)
            {
                continue;
            }

            var eligibilityInfo = new CustomerSegmentEligibilityInfo
            {
                SegmentId = segment.Id,
                Title = segment.Title,
                Description = segment.Description,
                ImageUrl = segment.ImageUrl,
                MemberCount = segment.ActualSize,
                IsMember = isMember,
                JoinMode = segment.JoinMode,
                Benefits = segment.Benefits
            };

            result.Add(eligibilityInfo);
        }

        return result;
    }

    public async Task<List<CustomerSegmentInfo>> AutoJoinCustomerToEligibleSegmentsAsync(
        EventRequest eventRequest, EventResponse eventResponse, CancellationToken cancellationToken)
    {
        // دریافت جامعه‌ها/بازارهای فعال با JoinMode = SystemOnly یا WithConditionCheck
        // TODO: باید TenantId از CustomerTenant گرفته شود
        var segments = await segmentRepo.GetAllAsync(cancellationToken);
        var autoJoinSegments = segments
            .Where(s => s.IsActive &&
                       (s.JoinMode == CustomerSegmentJoinMode.SystemOnly || 
                        s.JoinMode == CustomerSegmentJoinMode.WithConditionCheck))
            .ToList();
        var response = new List<CustomerSegmentInfo>();
        foreach (var segment in autoJoinSegments)
        {
            // بررسی واجد شرایط بودن
            var eligibility = await CheckCustomerEligibilityAsync(eventResponse.CustomerTenantId, segment.Id, eventRequest.Attributes?.Customer ??[], cancellationToken);

            if (eligibility.IsEligible)
            {
                if (!eligibility.IsAlreadyMember)
                {
                    // عضو کردن خودکار
                    var result = await JoinCustomerToSegmentAsync(eventResponse.CustomerTenantId, segment.Id, eventResponse.EventLogId, cancellationToken);
                    if (result.Success)
                    {
                        response.Add(new CustomerSegmentInfo(segment.Id, segment.Title, CustomerSegmentInfoType.Join));
                    }
                }
                else
                {
                    response.Add(new CustomerSegmentInfo(segment.Id, segment.Title, CustomerSegmentInfoType.Member));
                }
            }
            else if (!eligibility.IsEligible && eligibility.IsAlreadyMember)
            {
                await LeaveCustomerFromSegmentAsync(eligibility.MemberId ?? 0, cancellationToken);
                response.Add(new CustomerSegmentInfo(segment.Id, segment.Title, CustomerSegmentInfoType.Leave));
            }
        }

        return response;
    }

    private async Task LeaveCustomerFromSegmentAsync(int memberId, CancellationToken cancellationToken)
    {
        await membershipCmdRepo.ExpireAsync(x => x.Id == memberId, cancellationToken);
    }

    public async Task<SegmentMembershipRecheckResult> RecheckSegmentMembershipAfterParameterChangeAsync(
        int customerTenantId, long eventLogId,
        AttributesValues? attributes, CancellationToken cancellationToken)
    {
        var result = new SegmentMembershipRecheckResult
        {
            AddedMemberships = 0,
            RemovedMemberships = 0,
            AddedSegmentIds = [],
            RemovedSegmentIds = []
        };
        
        // دریافت CustomerTenant
        var customerTenant = await customerTenantQueryRepo.GetByIdAsync(customerTenantId, cancellationToken);
        if (customerTenant == null)
        {
            return result;
        }

        // دریافت همه جامعه‌ها/بازارهای فعال
        var segments = await segmentRepo.GetAllAsync(cancellationToken);
        var activeSegments = segments.Where(s => s.IsActive).ToList();

        // دریافت عضویت‌های فعلی مشتری (فقط عضویت‌های غیر دستی)
        var currentMemberships = await membershipQueryRepo.GetAllAsync(
            cancellationToken,
            m => m.CustomerTenantId == customerTenantId && !m.IsManual && !m.IsDeleted);

        var currentMembershipSegmentIds = currentMemberships.Select(m => m.SegmentId).ToHashSet();

        // بررسی هر جامعه/بازار
        foreach (var segment in activeSegments)
        {
            // بررسی واجد شرایط بودن
            var eligibility = await CheckCustomerEligibilityAsync(customerTenantId, segment.Id, attributes, cancellationToken);
            
            bool isCurrentlyMember = currentMembershipSegmentIds.Contains(segment.Id);

            if (eligibility.IsEligible && !isCurrentlyMember)
            {
                // باید عضو شود (فقط برای جامعه‌ها/بازارهای SystemOnly یا WithConditionCheck)
                if (segment.JoinMode == CustomerSegmentJoinMode.SystemOnly || 
                    segment.JoinMode == CustomerSegmentJoinMode.WithConditionCheck)
                {
                    var joinResult = await JoinCustomerToSegmentAsync(customerTenantId, segment.Id, eventLogId, cancellationToken);
                    if (joinResult.Success)
                    {
                        result = result with
                        {
                            AddedSegmentIds = [.. result.AddedSegmentIds, segment.Id],
                            AddedMemberships = result.AddedMemberships + 1
                        };
                    }
                }
            }
            else if (!eligibility.IsEligible && isCurrentlyMember)
            {
                // باید از عضویت خارج شود (فقط برای عضویت‌های غیر دستی)
                var membership = currentMemberships.FirstOrDefault(m => m.SegmentId == segment.Id);
                if (membership != null && !membership.IsManual)
                {
                    membership.IsDeleted = true;
                    membershipCmdRepo.Update(membership);
                    result = result with
                    {
                        RemovedSegmentIds = [.. result.RemovedSegmentIds, segment.Id],
                        RemovedMemberships = result.RemovedMemberships + 1
                    };
                }
            }
        }

        if (result.RemovedMemberships > 0)
        {
            await membershipCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}

