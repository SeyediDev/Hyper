/*using Hyper.Domain.Models;

namespace Hyper.Domain.Features;

/// <summary>
/// سرویس پردازش قوانین معرف
/// </summary>
public interface IReferrerRuleProcessingService
{
    Task<Result> ProcessReferrerRuleActionsAsync(string referrerCode, int newCustomerId, int tenantId, long eventLogId);
    Task<Result> ApplyReferrerActionsAsync(RuleAction action, int referrerCustomerId, int referredCustomerId, long eventLogId);
}

/// <summary>
/// پیاده‌سازی سرویس پردازش قوانین معرف
/// </summary>
public class ReferrerRuleProcessingService(
    IReferrerCodeValidationService referrerCodeValidationService,
    //IQueryRepository<RuleAction, int> ruleActionQueryRepo,
    IQueryRepository<Customer, int> customerQueryRepo,
    ICommandRepository<Customer, int> customerCmdRepo,
    ICommandRepository<CustomerTransaction, long> customerTransactionCmdRepo,
    //IQueryRepository<Point, int> pointQueryRepo,
    ILogger<ReferrerRuleProcessingService> logger) : IReferrerRuleProcessingService
{

    /// <summary>
    /// پردازش عملیات‌های قانون معرف
    /// </summary>
    public async Task<Result> ProcessReferrerRuleActionsAsync(string referrerCode, int newCustomerId, int tenantId, long eventLogId)
    {
        try
        {
            // اعتبارسنجی کد معرف
            var validationResult = await referrerCodeValidationService.ValidateReferrerCodeAsync(referrerCode, tenantId);
            if (!validationResult.IsSuccess)
                return Result.Failure(validationResult.ErrorMessage);

            var referrerCodeEntity = validationResult.Data!;
            var referrerCustomerId = referrerCodeEntity.CustomerId;

            // دریافت عملیات‌های مربوط به ثبت معرف
            var actionsResult = await referrerCodeValidationService.GetReferrerActionsAsync(referrerCodeEntity.Id);
            if (!actionsResult.IsSuccess)
                return Result.Failure(actionsResult.ErrorMessage);

            var actions = actionsResult.Data!;

            // اعمال عملیات‌ها
            foreach (var action in actions)
            {
                var applyResult = await ApplyReferrerActionsAsync(action, referrerCustomerId, newCustomerId, eventLogId);
                if (!applyResult.IsSuccess)
                {
                    logger.LogWarning("خطا در اعمال عملیات {ActionId}: {Error}", 
                        action.Id, applyResult.ErrorMessage);
                }
            }

            logger.LogInformation("عملیات‌های قانون معرف پردازش شد: {ReferrerCode} -> {NewCustomerId}", 
                referrerCode, newCustomerId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در پردازش عملیات‌های قانون معرف: {ReferrerCode}", referrerCode);
            return Result.Failure("خطا در پردازش عملیات‌های قانون معرف");
        }
    }

    /// <summary>
    /// اعمال عملیات معرف
    /// </summary>
    public async Task<Result> ApplyReferrerActionsAsync(RuleAction action, int referrerCustomerId, int referredCustomerId, long eventLogId)
    {
        try
        {
            switch (action.ActionOnWho)
            {
                case RuleActionOnWho.Customer:
                    return await ApplyActionToCustomerAsync(action, referredCustomerId, eventLogId);
                
                case RuleActionOnWho.Referrer:
                    return await ApplyActionToCustomerAsync(action, referrerCustomerId, eventLogId);
                
                case RuleActionOnWho.Both:
                    var referrerResult = await ApplyActionToCustomerAsync(action, referrerCustomerId, eventLogId);
                    var referredResult = await ApplyActionToCustomerAsync(action, referredCustomerId, eventLogId);
                    return referrerResult.IsSuccess && referredResult.IsSuccess ? Result.Success() : Result.Failure("خطا در اعمال عملیات");
                
                default:
                    return Result.Failure("نوع عملیات پشتیبانی نمی‌شود");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در اعمال عملیات معرف: {ActionId}", action.Id);
            return Result.Failure("خطا در اعمال عملیات معرف");
        }
    }

    /// <summary>
    /// اعمال عملیات به مشتری
    /// </summary>
    private async Task<Result> ApplyActionToCustomerAsync(RuleAction action, int customerId, long eventLogId)
    {
        try
        {
            var customer = await customerQueryRepo.GetByIdAsync(customerId, CancellationToken.None);
            if (customer == null)
                return Result.Failure("مشتری یافت نشد");

            switch (action.ActionKind)
            {
                case RuleActionKind.CreditPoint:
                    return await ApplyCreditPointAsync(action, customer, eventLogId);
                
                case RuleActionKind.DebitPoint:
                    return await ApplyDebitPointAsync(action, customer, eventLogId);
                
                case RuleActionKind.SetPointBalance:
                    return await ApplySetPointBalanceAsync(action, customer, eventLogId);
                
                case RuleActionKind.ReferrerRegistration:
                    return await ApplyReferrerRegistrationAsync(action, customer, eventLogId);
                
                default:
                    logger.LogWarning("نوع عملیات پشتیبانی نمی‌شود: {ActionKind}", action.ActionKind);
                    return Result.Success(); // ادامه پردازش سایر عملیات
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در اعمال عملیات به مشتری: {CustomerId}", customerId);
            return Result.Failure("خطا در اعمال عملیات به مشتری");
        }
    }

    /// <summary>
    /// اعمال افزایش امتیاز
    /// </summary>
    private async Task<Result> ApplyCreditPointAsync(RuleAction action, Customer customer, long eventLogId)
    {
        try
        {
            if (action.Point == null)
                return Result.Failure("نوع امتیاز مشخص نشده");

            var amount = CalculateAmount(action);
            if (amount <= 0)
                return Result.Failure("مقدار امتیاز نامعتبر");

            // ایجاد تراکنش امتیاز
            var transaction = new CustomerTransaction
            {
                TenantId = 1, // TODO: باید از context دریافت شود
                CustomerId = customer.Id,
                PointId = action.Point.Id,
                Credit = amount,
                Balance = customer.ReferralEarnedPoints + amount,
                TransactionType = CustomerTransactionType.Credit,
                EventLogId = eventLogId,
                PromotionId = action.PromotionId,
                PromotionActionId = action.Id
            };

            await customerTransactionCmdRepo.AddAsync(transaction);

            // به‌روزرسانی امتیازات مشتری
            customer.ReferralEarnedPoints += (int)amount;
            customerCmdRepo.Update(customer);

            logger.LogInformation("امتیاز به مشتری اضافه شد: {CustomerId} -> {Amount}", 
                customer.Id, amount);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در اعمال افزایش امتیاز: {CustomerId}", customer.Id);
            return Result.Failure("خطا در اعمال افزایش امتیاز");
        }
    }

    /// <summary>
    /// اعمال کاهش امتیاز
    /// </summary>
    private async Task<Result> ApplyDebitPointAsync(RuleAction action, Customer customer, long eventLogId)
    {
        try
        {
            if (action.Point == null)
                return Result.Failure("نوع امتیاز مشخص نشده");

            var amount = CalculateAmount(action);
            if (amount <= 0)
                return Result.Failure("مقدار امتیاز نامعتبر");

            // بررسی موجودی
            if (customer.ReferralEarnedPoints < amount)
                return Result.Failure("موجودی امتیاز کافی نیست");

            // ایجاد تراکنش امتیاز
            var transaction = new CustomerTransaction
            {
                TenantId = 1, // TODO: باید از context دریافت شود
                CustomerId = customer.Id,
                PointId = action.Point.Id,
                Debit = amount,
                Balance = customer.ReferralEarnedPoints - amount,
                TransactionType = CustomerTransactionType.Debit,
                EventLogId = eventLogId,
                PromotionId = action.PromotionId,
                PromotionActionId = action.Id
            };

            await customerTransactionCmdRepo.AddAsync(transaction);

            // به‌روزرسانی امتیازات مشتری
            customer.ReferralEarnedPoints -= (int)amount;
            customerCmdRepo.Update(customer);

            logger.LogInformation("امتیاز از مشتری کسر شد: {CustomerId} -> {Amount}", 
                customer.Id, amount);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در اعمال کاهش امتیاز: {CustomerId}", customer.Id);
            return Result.Failure("خطا در اعمال کاهش امتیاز");
        }
    }

    /// <summary>
    /// اعمال مقداردهی امتیاز
    /// </summary>
    private async Task<Result> ApplySetPointBalanceAsync(RuleAction action, Customer customer, long eventLogId)
    {
        try
        {
            if (action.Point == null)
                return Result.Failure("نوع امتیاز مشخص نشده");

            var amount = CalculateAmount(action);
            if (amount < 0)
                return Result.Failure("مقدار امتیاز نامعتبر");

            var oldBalance = customer.ReferralEarnedPoints;
            var difference = amount - oldBalance;

            // ایجاد تراکنش امتیاز
            var transaction = new CustomerTransaction
            {
                TenantId = 1, // TODO: باید از context دریافت شود
                CustomerId = customer.Id,
                PointId = action.Point.Id,
                Credit = difference > 0 ? difference : null,
                Debit = difference < 0 ? Math.Abs(difference) : null,
                Balance = amount,
                TransactionType = difference > 0 ? CustomerTransactionType.Credit : CustomerTransactionType.Debit,
                EventLogId = eventLogId,
                PromotionId = action.PromotionId,
                PromotionActionId = action.Id
            };

            await customerTransactionCmdRepo.AddAsync(transaction);

            // به‌روزرسانی امتیازات مشتری
            customer.ReferralEarnedPoints = (int)amount;
            customerCmdRepo.Update(customer);

            logger.LogInformation("امتیاز مشتری تنظیم شد: {CustomerId} -> {Amount}", 
                customer.Id, amount);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در اعمال مقداردهی امتیاز: {CustomerId}", customer.Id);
            return Result.Failure("خطا در اعمال مقداردهی امتیاز");
        }
    }

    /// <summary>
    /// اعمال ثبت معرف
    /// </summary>
    private Task<Result> ApplyReferrerRegistrationAsync(RuleAction action, Customer customer, long eventLogId)
    {
        try
        {
            // این عملیات قبلاً در ProcessReferrerRegistrationAsync انجام شده
            logger.LogInformation("عملیات ثبت معرف اعمال شد: {CustomerId}", customer.Id);
            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در اعمال ثبت معرف: {CustomerId}", customer.Id);
            return Task.FromResult(Result.Failure("خطا در اعمال ثبت معرف"));
        }
    }

    /// <summary>
    /// محاسبه مقدار عملیات
    /// </summary>
    private long CalculateAmount(RuleAction action)
    {
        try
        {
            switch (action.AmountMethod)
            {
                case RuleActionAmountMethod.FixAmount:
                    return long.TryParse(action.Amount, out var fixedAmount) ? fixedAmount : 0;
                
                case RuleActionAmountMethod.FromParameter:
                    // TODO: پیاده‌سازی محاسبه بر اساس پارامتر
                    return 0;
                
                case RuleActionAmountMethod.FromFormula:
                    // TODO: پیاده‌سازی محاسبه بر اساس فرمول
                    return 0;
                
                default:
                    return 0;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در محاسبه مقدار عملیات: {ActionId}", action.Id);
            return 0;
        }
    }
}
*/