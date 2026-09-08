namespace Hyper.Domain.Features.Points;

public interface IPointLevelService
{
    [Telemetry]
    Task<CheckPointLevelResponse> CheckAndUpdateLevel(CheckPointLevelRequest request, CancellationToken cancellationToken);
}

public record CheckPointLevelRequest(int PointId, int CustomerTenantId, long Balance, long EventLogId);

public record CheckPointLevelResponse
{
    public Dictionary<int, int> NewPointLevelIds { get; set; } = [];
}

internal class PointLevelService(
    ICommandRepository<CustomerPointLevel, int> customerPointLevelCmdRepo,
    IQueryRepository<PointLevel, int> pointLevelRepo
    ) : IPointLevelService
{
    public async Task<CheckPointLevelResponse> CheckAndUpdateLevel(CheckPointLevelRequest request, CancellationToken cancellationToken)
    {
        CheckPointLevelResponse response = new();
        IEnumerable<PointLevel> pointLevels = await pointLevelRepo.GetAllAsync(cancellationToken,
            x => x.PointId == request.PointId);
        CustomerPointLevel? oldCustomerPointLevel = await customerPointLevelCmdRepo.FirstOrDefaultAsync(
            x => x.CustomerTenantId == request.CustomerTenantId && x.PointLevel.PointId == request.PointId, cancellationToken);
        PointLevel? pointLevel = pointLevels.Where(x => x.MinXp >= request.Balance).OrderByDescending(x => x.Level).FirstOrDefault();
        if (pointLevel == null)
        {
            //الان با این موجودی شامل هیچ سطحی نمی شه
            if (oldCustomerPointLevel != null)
            {
                //اگر قبلا سطحی داشته منقرض ش کن
                await ExpireCustomerPointLevel(oldCustomerPointLevel, cancellationToken);
            }
        }
        else
        {
            //الان با این موجودی مستحق این سطح است pointLevel
            if (oldCustomerPointLevel != null && oldCustomerPointLevel.PointLevelId != pointLevel?.Id)
            {
                //اگر قبلا سطح متفاوتی داشته منقرض ش کن
                await ExpireCustomerPointLevel(oldCustomerPointLevel, cancellationToken);
            }
            if (oldCustomerPointLevel == null || oldCustomerPointLevel.PointLevelId != pointLevel?.Id)
            {
                //اگر قبلا سطح نداشته یا سطح متفاوتی داشته الان بهش سطح بده
                CustomerPointLevel? customerPointPointLevel = new()
                {
                    CustomerTenantId = request.CustomerTenantId,
                    PointLevelId = pointLevel?.Id ?? 0,
                    EventLogId = request.EventLogId
                };
                customerPointLevelCmdRepo.Add(customerPointPointLevel);
                _ = await customerPointLevelCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
                _ = response.NewPointLevelIds.TryAdd(customerPointPointLevel.PointLevelId, customerPointPointLevel.PointLevelId);
            }
        }
        return response;
    }

    private async Task ExpireCustomerPointLevel(CustomerPointLevel customerPointLevel, CancellationToken cancellationToken)
    {
        customerPointLevel.ExpireDate = DateTime.UtcNow;
        customerPointLevelCmdRepo.Update(customerPointLevel);
        _ = await customerPointLevelCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
