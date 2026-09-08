namespace Hyper.Domain.Features;

/// <summary>
/// سرویس مدیریت کلیدهای مستاجر
/// </summary>
public interface ITenantKeyService
{
    Task<int> GetTenantIdByKeyAsync(string tenantKey);
    Task<string> GetTenantKeyByIdAsync(int tenantId);
}

/// <summary>
/// پیاده‌سازی سرویس مدیریت کلیدهای مستاجر
/// </summary>
public class TenantKeyService(
    IQueryRepository<Hyper.Domain.Entities.Tenants.Tenant, int> tenantQueryRepo,
    ILogger<TenantKeyService> logger) : ITenantKeyService
{

    /// <summary>
    /// دریافت شناسه مستاجر بر اساس کلید
    /// </summary>
    public async Task<int> GetTenantIdByKeyAsync(string tenantKey)
    {
        try
        {
            var tenant = await tenantQueryRepo.FirstOrDefaultAsync(t => t.Key == tenantKey, CancellationToken.None);
            if (tenant == null)
            {
                logger.LogWarning("مستاجر با کلید {TenantKey} یافت نشد", tenantKey);
                throw new ArgumentException($"مستاجر با کلید {tenantKey} یافت نشد");
            }

            return tenant.Id;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در دریافت شناسه مستاجر برای کلید {TenantKey}", tenantKey);
            throw;
        }
    }

    /// <summary>
    /// دریافت کلید مستاجر بر اساس شناسه
    /// </summary>
    public async Task<string> GetTenantKeyByIdAsync(int tenantId)
    {
        try
        {
            var tenant = await tenantQueryRepo.GetByIdAsync(tenantId, CancellationToken.None);
            if (tenant == null)
            {
                logger.LogWarning("مستاجر با شناسه {TenantId} یافت نشد", tenantId);
                throw new ArgumentException($"مستاجر با شناسه {tenantId} یافت نشد");
            }

            return tenant.Key;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در دریافت کلید مستاجر برای شناسه {TenantId}", tenantId);
            throw;
        }
    }
}
