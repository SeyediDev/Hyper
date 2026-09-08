using Neo.Domain.Dto;

namespace Hyper.Domain.Features.Referal;

/// <summary>
/// سرویس اعتبارسنجی و پردازش کدهای معرف
/// </summary>
public interface IReferrerCodeValidationService
{
    Task<Result<ReferrerCode>> ValidateReferrerCodeAsync(string referrerCode, int tenantId);
    Task<Result> UpdateReferrerCodeUsageAsync(int referrerCodeId);
    Task<Result<List<PromotionAction>>> GetReferrerActionsAsync(int referrerCodeId);
    Task<Result> RegisterReferralAsync(int referrerCodeId, int referredCustomerTenantId, int tenantId, long eventLogId, CancellationToken cancellationToken);
}

/// <summary>
/// پیاده‌سازی سرویس اعتبارسنجی کدهای معرف
/// </summary>
public class ReferrerCodeValidationService(
    IQueryRepository<ReferrerCode, int> referrerCodeQueryRepo,
    ICommandRepository<ReferrerCode, int> referrerCodeCmdRepo,
    IQueryRepository<PromotionAction, int> ruleActionQueryRepo,
    ICommandRepository<CustomerReferrer, int> customerReferrerCmdRepo,
    IQueryRepository<CustomerTenant, int> customerTenantQueryRepo,
    ILogger<ReferrerCodeValidationService> logger)
    : IReferrerCodeValidationService
{

    /// <summary>
    /// اعتبارسنجی کد معرف
    /// </summary>
    public async Task<Result<ReferrerCode>> ValidateReferrerCodeAsync(string referrerCode, int tenantId)
    {
        try
        {
            var code = await referrerCodeQueryRepo.FirstOrDefaultAsync(
                rc => rc.Code == referrerCode && 
                      rc.CustomerTenant.TenantId == tenantId, CancellationToken.None);

            if (code == null)
            {
                logger.LogWarning("کد معرف یافت نشد: {ReferrerCode}", referrerCode);
                return Result<ReferrerCode>.Failure("کد معرف یافت نشد");
            }

            logger.LogInformation("کد معرف معتبر است: {ReferrerCode}", referrerCode);
            return Result<ReferrerCode>.Success(code);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در اعتبارسنجی کد معرف: {ReferrerCode}", referrerCode);
            return Result<ReferrerCode>.Failure("خطا در اعتبارسنجی کد معرف");
        }
    }

    /// <summary>
    /// به‌روزرسانی تعداد استفاده کد معرف
    /// </summary>
    public async Task<Result> UpdateReferrerCodeUsageAsync(int referrerCodeId)
    {
        try
        {
            var referrerCode = await referrerCodeQueryRepo.GetByIdAsync(referrerCodeId, CancellationToken.None);
            if (referrerCode == null)
                return Result.Failure("کد معرف یافت نشد");
            referrerCodeCmdRepo.Update(referrerCode);

            logger.LogInformation("تعداد استفاده کد معرف به‌روزرسانی شد: {ReferrerCode}", referrerCode.Code);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در به‌روزرسانی تعداد استفاده کد معرف: {ReferrerCodeId}", referrerCodeId);
            return Result.Failure("خطا در به‌روزرسانی تعداد استفاده کد معرف");
        }
    }

    /// <summary>
    /// دریافت عملیات‌های مربوط به کد معرف
    /// </summary>
    public async Task<Result<List<PromotionAction>>> GetReferrerActionsAsync(int referrerCodeId)
    {
        try
        {
            var referrerCode = await referrerCodeQueryRepo.GetByIdAsync(referrerCodeId, CancellationToken.None);
            if (referrerCode == null)
                return Result<List<PromotionAction>>.Failure("کد معرف یافت نشد");

            // دریافت عملیات‌های مربوط به ثبت معرف
            var actions = await ruleActionQueryRepo.GetAllAsync(
                CancellationToken.None, 
                ra => ra.ActionKind == PromotionActionKind.ReferrerRegistration);

            return Result<List<PromotionAction>>.Success([.. actions]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در دریافت عملیات‌های کد معرف: {ReferrerCodeId}", referrerCodeId);
            return Result<List<PromotionAction>>.Failure("خطا در دریافت عملیات‌های کد معرف");
        }
    }

    /// <summary>
    /// ثبت معرف
    /// </summary>
    public async Task<Result> RegisterReferralAsync(
        int referrerCodeId,
        int referredCustomerTenantId,
        int tenantId,
        long eventLogId,
        CancellationToken cancellationToken)
    {
        try
        {
            var referrerCode = await referrerCodeQueryRepo.GetByIdAsync(referrerCodeId, cancellationToken);
            if (referrerCode == null)
                return Result.Failure("کد معرف یافت نشد");

            var referrerCustomerTenant = referrerCode.CustomerTenant
                ?? await customerTenantQueryRepo.FirstOrDefaultAsync(
                    ct => ct.Id == referrerCode.CustomerTenantId,
                    cancellationToken)
                ?? throw new InvalidOperationException("رابطه مشتری معرف یافت نشد");

            var existingReferrer = await customerReferrerCmdRepo.FirstOrDefaultAsync(
                x => x.ReferredCustomerTenantId == referredCustomerTenantId &&
                     x.ReferrerCodeId == referrerCodeId,
                cancellationToken);

            if (existingReferrer != null)
            {
                logger.LogInformation("معرفی قبلاً ثبت شده است برای کد معرف {ReferrerCode} و مشتری {ReferredCustomerTenantId}",
                    referrerCode.Code, referredCustomerTenantId);
                return Result.Success();
            }

            var customerReferrer = new CustomerReferrer
            {
                ReferrerCustomerTenantId = referrerCustomerTenant.Id,
                ReferredCustomerTenantId = referredCustomerTenantId,
                ReferrerCodeId = referrerCode.Id,
                EventLogId = eventLogId,
                PromotionId = null,
                PromotionActionId = null
            };

            customerReferrerCmdRepo.Add(customerReferrer);
            await customerReferrerCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("معرفی جدید ثبت شد: کد معرف {ReferrerCode}, مشتری معرف {ReferrerCustomerId}, مشتری دعوت شده {ReferredCustomerTenantId}",
                referrerCode.Code, referrerCustomerTenant.CustomerId, referredCustomerTenantId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در ثبت معرفی برای کد معرف {ReferrerCodeId} و مشتری {ReferredCustomerTenantId}",
                referrerCodeId, referredCustomerTenantId);
            return Result.Failure("خطا در ثبت معرفی");
        }
    }
}
