namespace Club.Domain.Features;

public interface IPointBudgetService
{
    Task<bool> HasBudget(int orgId, int pointId, long amount, Customer customer,
        List<PointLevel>? customerPointLevels, CancellationToken cancellationToken);
}

public class PointBudgetService(
    IQueryRepository<PointBudget, int> budgetRepo,
    IQueryRepository<CustomerTransaction, long> customerTransactionRepo,
    ILogger<PointBudgetService> logger) 
    : IPointBudgetService
{
    public async Task<bool> HasBudget(int orgId, int pointId, long amount, Customer customer, 
        List<PointLevel>? customerPointLevels, CancellationToken cancellationToken)
    {
        List<PointLevel> samePointCustomerPointLevels = customerPointLevels?.Where(x => x.PointId == pointId).ToList() ?? [];
        var budgets = await budgetRepo.GetAllWithIncludeAsync(
            [x=>x.PointLevel], 
            cancellationToken, 
            x => x.Point.TenantId == orgId && x.PointId == pointId);
        foreach (var budget in budgets)
        {
            if(budget.PointLevel != null)
            {
                if(samePointCustomerPointLevels.Any(x=>x.Id== budget.PointLevelId || x.Level<= budget.PointLevel.Level))
                {
                    logger.LogInformation("1. HasBudget({orgId}, {pointId}, {amount}, {customer}, {pointLevelId}) {budgetId} {pointLevel} {level} {message}",
                        orgId, pointId, amount, customer.NationalCode, customerPointLevels.ToJson(), budget.Id, 
                        budget.PointLevel.Id, budget.PointLevel.Level, "برای سطح‌امتیاز مشتری بودجه محدود نیست");
                    return true;
                }
            }
            logger.LogInformation("2. HasBudget({orgId}, {pointId}, {amount}, {customer}, {PointLevelId}) {budgetId} {message}",
                orgId, pointId, amount, customer.NationalCode, customerPointLevels.ToJson(), budget.Id, "کنترل محدودیت بودجه ای");
            var used = CalculateUsedBudget(budget, customer);

            if ((used + amount) > budget.Amount)
            {
                logger.LogInformation("3. HasBudget({orgId}, {pointId}, {amount}, {customer}, {pointLevelId}) {budgetId} {budgetAmount} {used} {message}",
                    orgId, pointId, amount, customer.NationalCode, customerPointLevels.ToJson(), budget.Id, budget.Amount, used, "بودجه تمام شده است");
                return false;
            }

            logger.LogInformation("4. HasBudget({orgId}, {pointId}, {amount}, {customer}, {pointLevelId}) {budgetId} {message}",
                orgId, pointId, amount, customer.NationalCode, customerPointLevels.ToJson(), budget.Id, "بودجه کافی است");
        }
        logger.LogInformation("5. HasBudget({orgId}, {pointId}, {amount}, {customer}, {pointLevelId}) {message}",
            orgId, pointId, amount, customer.NationalCode, customerPointLevels.ToJson(), "بودجه ها کافی است");
        return true;
    }

    private long CalculateUsedBudget(PointBudget budget, Customer customer)
    {
        var query = customerTransactionRepo.Query()
                    .Where(x => x.TenantId == budget.Point.TenantId && x.PointId == budget.PointId);
        if (budget.Scope == PointBudgetScope.PerCustomer)
        {
            query = query.Where(x => x.CustomerTenant.CustomerId == customer.Id);
        }
        if (budget.FromDate!=null)
        {
            query = query.Where(x => x.CreateDate >= budget.FromDate);
        }
        if (budget.ToDate != null)
        {
            query = query.Where(x => x.CreateDate <= budget.ToDate);
        }
        DateTime now = DateTime.Now;
        switch (budget.Kind)
        {
            case PointBudgetKind.Daily:
                query = query.Where(x => x.CreateDate.Date == now.Date);
                break;
            case PointBudgetKind.Weekly:
                query = query.Where(x => x.CreateDate.DayOfWeek == now.DayOfWeek);
                break;
            case PointBudgetKind.Monthly:
                query = query.Where(x => x.CreateDate.Month == now.Month);
                break;
            case PointBudgetKind.Yearly:
                query = query.Where(x => x.CreateDate.Year == now.Year);//TODO It is miladi
                break;
            case PointBudgetKind.InPeriod:
                break;
        }
        return query.Sum(x => (x.Credit!=null? x.Credit.Value :0) - (x.Debit!=null? x.Debit.Value : 0));
    }
}
