using Club.Domain.Entities.Promotions;
using Neo.Domain.Dto;

namespace Club.Domain.Features;

/// <summary>
/// سرویس اعتبارسنجی و پردازش کدهای معرف
/// </summary>
public interface IReferrerCodeValidationService
{
    Task<Result<ReferrerCode>> ValidateReferrerCodeAsync(string referrerCode, int tenantId);
    Task<Result> UpdateReferrerCodeUsageAsync(int referrerCodeId);
    Task<Result<List<PromotionAction>>> GetReferrerActionsAsync(int referrerCodeId);
}

/// <summary>
/// پیاده‌سازی سرویس اعتبارسنجی کدهای معرف
/// </summary>
public class ReferrerCodeValidationService(
    IQueryRepository<ReferrerCode, int> referrerCodeQueryRepo,
    ICommandRepository<ReferrerCode, int> referrerCodeCmdRepo,
    IQueryRepository<PromotionAction, int> ruleActionQueryRepo,
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
                      rc.TenantId == tenantId, CancellationToken.None);

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

            return Result<List<PromotionAction>>.Success(actions.ToList());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در دریافت عملیات‌های کد معرف: {ReferrerCodeId}", referrerCodeId);
            return Result<List<PromotionAction>>.Failure("خطا در دریافت عملیات‌های کد معرف");
        }
    }
}
