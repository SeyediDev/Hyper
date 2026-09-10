namespace Hyper.Domain.Features;

public interface IEventService
{
    [Telemetry]
    Task<EventResponse> RecordEventAsync(EventRequest eventRequest, CancellationToken cancellationToken);

    public Task<EventTypeParameter?> GetParameterAsync(int eventTypeId, string parameterKey, bool createIfNotExists, CancellationToken cancellationToken);
}

public record EventRequest(TriggerType TriggerType, string CustomerIdentifier, Dictionary<string, string>? Parameters)
{
    public int TenantId { get; set; }
    public string? EventChannel { get; set; }
    public string? EventType { get; set; }
    public int? PromotionId { get; set; }
    public int? PointLevelId { get; set; }
    public int? AwardId { get; set; }
    public int? ProductId { get; set; }
    public string? ProductCategoryKey { get; set; }
    public string? ProductKey { get; set; }
    public int? AssetId { get; set; }
}

public record EventResponse
{
    public Customer Customer { get; set; } = null!;
    public int? EventChannelId { get; set; }
    public int? EventTypeId { get; set; }
    public long EventLogId { get; set; }
}

internal class EventService(
        ICustomerService customerService,
        IQueryRepository<EventType, int> eventTypeRepo,
        IQueryRepository<EventChannel, int> eventChannelRepo,
        ICommandRepository<EventLog, long> eventLogCmdRepo,
        ICommandRepository<EventLogParameter, long> eventLogParameterCmdRepo,
        ICommandRepository<EventTypeParameter, int> eventTypeParameterCmdRepo,
        ICommandRepository<CustomerTenant, int> customerTenantCmdRepo,
        ICustomerSegmentService customerSegmentService
    ) : IEventService
{
    public async Task<EventResponse> RecordEventAsync(EventRequest request, CancellationToken cancellationToken)
    {
        if (request.TenantId <= 0)
        {
            throw new ArgumentException("شناسه اکوسیستم معتبر نمی‌باشد");
        }

        long eventLogId = 0;
        EventChannel? eventChannel=null;
        int? eventTypeId = null;
        if (request.TriggerType == TriggerType.Event)
        {
            eventChannel = await eventChannelRepo.FirstOrDefaultAsync(
                x => x.Key == request.EventChannel && x.TenantId == request.TenantId,
                cancellationToken);
            if (eventChannel == null)
            {
                throw new ArgumentException("منبع تولید رویداد معتبر نمی باشد");
            }

            eventTypeId = await eventTypeRepo.Query()
                .Where(x => x.Key == request.EventType && x.TenantId == request.TenantId)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (eventTypeId == null)
            {
                throw new ArgumentException("رویداد معتبر نمی باشد");
            }
        }

        Customer customer = await customerService.GetCustomerByMobile(request.CustomerIdentifier, true, request.Parameters, cancellationToken)??
            throw new ArgumentException("اطلاعات مشتری معتبر نمی باشد");

        CustomerTenant customerTenant = await EnsureCustomerTenantAsync(customer, request.TenantId, cancellationToken);

        await eventLogCmdRepo.UnitOfWork.DoTransaction(async () =>
        {
            eventLogId = await LogEvent(request, eventChannel?.Id, eventTypeId, customer, customerTenant, cancellationToken);
            await LogEventParameters(request, customer, customerTenant, eventTypeId, eventLogId, cancellationToken);
        });
        
        // بررسی خودکار و عضویت در جامعه‌ها/بازارهای مناسب
        try
        {
            await customerSegmentService.AutoJoinCustomerToEligibleSegmentsAsync(
                customer.Id, 
                eventLogId, 
                cancellationToken);
        }
        catch (Exception)
        {
            // لاگ خطا اما عدم مسدود کردن جریان اصلی
            // TODO: اضافه کردن logger
            // logger.LogWarning(ex, "خطا در بررسی خودکار عضویت جامعه/بازار برای مشتری {CustomerId}", customer.Id);
        }
        
        return new()
        {
            Customer = customer,
            EventChannelId = eventChannel?.Id,
            EventTypeId = eventTypeId,
            EventLogId = eventLogId
        };
    }

    private async Task<long> LogEvent(EventRequest request, int? eventChannelId, int? eventTypeId,
        Customer customer, CustomerTenant customerTenant, CancellationToken cancellationToken)
    {
        EventLog eventLog = new()
        {
            TenantId = request.TenantId,
            CustomerTenantId = customerTenant.Id,
            TriggerType = request.TriggerType,
            EventChannelId = eventChannelId,
            EventTypeId = eventTypeId,
            PromotionId = request.PromotionId,
            PointLevelId = request.PointLevelId,
            AwardId = request.AwardId,
            ProductId = request.ProductId,
            ProductCategoryKey = request.ProductCategoryKey,
            ProductKey = request.ProductKey,
            AssetId = request.AssetId,
        };
        eventLogCmdRepo.Add(eventLog);
        _ = await eventLogCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        return eventLog.Id;
    }

    private async Task<List<EventLogParameter>> LogEventParameters(
        EventRequest eventData, Customer customer, CustomerTenant customerTenant,
        int? eventTypeId, long eventLogId, CancellationToken cancellationToken)
    {
        List<EventLogParameter> parameters = [];
        if (eventTypeId == null || eventData.Parameters == null)
        {
            return parameters;
        }

        foreach (KeyValuePair<string, string> param in eventData.Parameters)
        {
            EventTypeParameter? parameter = await GetParameterAsync(eventTypeId.Value, param.Key, true, cancellationToken);
            EventLogParameter eventLogParameter = new()
            {
                EventLogId = eventLogId,
                ParameterId = parameter?.Id ?? 0,
                Value = param.Value
            };
            parameters.Add(eventLogParameter);
            eventLogParameterCmdRepo.Add(eventLogParameter);
            if (parameter?.CustomerParameterId !=null)
            {
                await customerService.SaveCustomerParameter(customer, customerTenant, parameter.CustomerParameterId.Value, param.Value, eventLogId, cancellationToken);
            }
        }
        _ = await eventLogParameterCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        return parameters;
    }
    
    private async Task<CustomerTenant> EnsureCustomerTenantAsync(Customer customer, int tenantId, CancellationToken cancellationToken)
    {
        CustomerTenant? customerTenant =
            await customerTenantCmdRepo.FirstOrDefaultAsync(
                ct => ct.CustomerId == customer.Id && ct.TenantId == tenantId,
                cancellationToken);

        if (customerTenant == null)
        {
            customerTenant = new CustomerTenant
            {
                CustomerId = customer.Id,
                TenantId = tenantId,
                JoinDate = DateTime.Now,
                IsActive = true
            };
            customerTenantCmdRepo.Add(customerTenant);
            _ = await customerTenantCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (!customerTenant.IsActive)
        {
            customerTenant.IsActive = true;
            customerTenant.LeaveDate = null;
            customerTenantCmdRepo.Update(customerTenant);
            _ = await customerTenantCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        return customerTenant;
    }
    
    public async Task<EventTypeParameter?> GetParameterAsync(int id, CancellationToken cancellationToken)
    {
        return await eventTypeParameterCmdRepo.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    public async Task<EventTypeParameter?> GetParameterAsync(
        int eventTypeId, string parameterKey, bool createIfNotExists, CancellationToken cancellationToken)
    {
        EventTypeParameter? parameter = await eventTypeParameterCmdRepo.FirstOrDefaultAsync(x => x.Key == parameterKey, cancellationToken);
        if (parameter == null && createIfNotExists)
        {
            parameter = new()
            {
                Key = parameterKey,
                Title = parameterKey,
                CreatedBySystem = true,
                IsOptional = true,
                ParameterType = ParameterType.String
            };
            eventTypeParameterCmdRepo.Add(parameter);
            _ = await eventTypeParameterCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        return parameter;
    }
}
