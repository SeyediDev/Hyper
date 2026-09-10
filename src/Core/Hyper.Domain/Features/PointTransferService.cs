namespace Hyper.Domain.Features;

public interface IPointTransferService
{
    Task<PointTransferResult> TransferPointsAsync(PointTransferRequest request, CancellationToken cancellationToken);
}

public record PointTransferRequest
{
    public int TenantId { get; set; }
    public string SourceCustomerMobile { get; set; } = null!;
    public string DestinationCustomerMobile { get; set; } = null!;
    public int PointId { get; set; }
    public long Amount { get; set; }
    public int EventChannelId { get; set; }
    public string? Description { get; set; }
}

public record PointTransferResult
{
    public long SourceTransactionId { get; set; }
    public long DestinationTransactionId { get; set; }
    public long EventLogId { get; set; }
    public long AmountTransferred { get; set; }
    public long? CommissionCharged { get; set; }
}

internal class PointTransferService(
    IQueryRepository<Customer, int> customerQueryRepo,
    ICommandRepository<Customer, int> customerCmdRepo,
    IQueryRepository<CustomerTenant, int> customerTenantQueryRepo,
    ICommandRepository<CustomerTenant, int> customerTenantCmdRepo,
    IQueryRepository<Point, int> pointQueryRepo,
    IQueryRepository<EventChannel, int> eventChannelQueryRepo,
    IQueryRepository<PointConversionRate, int> conversionRateQueryRepo,
    IQueryRepository<CustomerTransaction, long> transactionQueryRepo,
    ICommandRepository<CustomerTransaction, long> transactionCmdRepo,
    ICommandRepository<EventLog, long> eventLogCmdRepo
    ) : IPointTransferService
{
    public async Task<PointTransferResult> TransferPointsAsync(PointTransferRequest request, CancellationToken cancellationToken)
    {
        // 1. Validate Point exists in Tenant
        var point = await pointQueryRepo.FirstOrDefaultAsync(
            p => p.Id == request.PointId && p.TenantId == request.TenantId, 
            cancellationToken);
        
        if (point == null)
            throw new InvalidOperationException($"امتیاز با شناسه {request.PointId} در اکوسیستم {request.TenantId} یافت نشد");

        // 1.1 Validate EventChannel exists in Tenant
        var eventChannel = await eventChannelQueryRepo.FirstOrDefaultAsync(
            ec => ec.Id == request.EventChannelId && ec.TenantId == request.TenantId,
            cancellationToken);

        if (eventChannel == null)
            throw new InvalidOperationException($"کانال رویداد با شناسه {request.EventChannelId} در اکوسیستم {request.TenantId} یافت نشد");

        // 2. Find or Create Source Customer
        var sourceCustomer = await customerQueryRepo.FirstOrDefaultAsync(
            c => c.MobileNo == request.SourceCustomerMobile, 
            cancellationToken);
        
        if (sourceCustomer == null)
            throw new InvalidOperationException("مشتری مبدا یافت نشد");

        // 3. Validate Source Customer belongs to Tenant
        var sourceCustomerTenant = await customerTenantQueryRepo.FirstOrDefaultAsync(
            ct => ct.CustomerId == sourceCustomer.Id && ct.TenantId == request.TenantId && ct.IsActive,
            cancellationToken);
        
        if (sourceCustomerTenant == null)
            throw new InvalidOperationException("مشتری مبدا در این اکوسیستم عضو نیست");

        // 4. Find or Create Destination Customer
        var destinationCustomer = await customerQueryRepo.FirstOrDefaultAsync(
            c => c.MobileNo == request.DestinationCustomerMobile,
            cancellationToken);

        if (destinationCustomer == null)
        {
            // Create new customer
            destinationCustomer = new Customer
            {
                MobileNo = request.DestinationCustomerMobile,
                FirstName = request.DestinationCustomerMobile, // Temporary
                LastName = request.DestinationCustomerMobile  // Temporary
            };
            customerCmdRepo.Add(destinationCustomer);
            await customerCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        // 5. Check or Create Destination Customer Tenant
        var destinationCustomerTenant = await customerTenantQueryRepo.FirstOrDefaultAsync(
            ct => ct.CustomerId == destinationCustomer.Id && ct.TenantId == request.TenantId,
            cancellationToken);

        if (destinationCustomerTenant == null)
        {
            // Create CustomerTenant for destination
            destinationCustomerTenant = new CustomerTenant
            {
                CustomerId = destinationCustomer.Id,
                TenantId = request.TenantId,
                JoinDate = DateTime.Now,
                IsActive = true
            };
            customerTenantCmdRepo.Add(destinationCustomerTenant);
            await customerTenantCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (!destinationCustomerTenant.IsActive)
        {
            throw new InvalidOperationException("مشتری مقصد در این اکوسیستم غیرفعال است");
        }

        // 6. Calculate Source Balance
        var sourceTransactions = await transactionQueryRepo.GetAllAsync(
            cancellationToken,
            ct => ct.CustomerTenantId == sourceCustomerTenant.Id && ct.PointId == request.PointId && ct.TenantId == request.TenantId);
        
        var sourceBalance = sourceTransactions.OrderByDescending(ct => ct.Id).FirstOrDefault()?.Balance ?? 0;

        // 7. Check Conversion Rate & Commission
        var conversionRate = await conversionRateQueryRepo.FirstOrDefaultAsync(
            pcr => pcr.FromPointId == request.PointId 
                && pcr.ToPointId == request.PointId 
                && pcr.IsActive, 
            cancellationToken);

        long commissionAmount = 0;
        int? commissionPointId = null;

        if (conversionRate?.CommissionAmount > 0 && conversionRate.CommissionPointId.HasValue)
        {
            commissionAmount = conversionRate.CommissionAmount.Value;
            commissionPointId = conversionRate.CommissionPointId.Value;

            // Check commission balance
            var commissionTransactions = await transactionQueryRepo.GetAllAsync(
                cancellationToken,
                ct => ct.CustomerTenantId == sourceCustomerTenant.Id && ct.PointId == commissionPointId && ct.TenantId == request.TenantId);
            
            var commissionBalance = commissionTransactions.OrderByDescending(ct => ct.Id).FirstOrDefault()?.Balance ?? 0;

            if (commissionBalance < commissionAmount)
                throw new InvalidOperationException($"موجودی کارمزد ناکافی است. موجودی فعلی: {commissionBalance}، کارمزد مورد نیاز: {commissionAmount}");
        }

        // 8. Validate sufficient balance
        if (sourceBalance < request.Amount)
            throw new InvalidOperationException($"موجودی ناکافی است. موجودی فعلی: {sourceBalance}، مقدار درخواستی: {request.Amount}");

        // 9. Create EventLog
        var eventLog = new EventLog
        {
            TenantId = request.TenantId,
            CustomerTenantId = sourceCustomerTenant.Id,
            EventChannelId = request.EventChannelId,
            TriggerType = TriggerType.PointTransfer
        };

        eventLogCmdRepo.Add(eventLog);
        await eventLogCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        // 10. Create Debit Transaction for Source Customer
        var sourceTransaction = new CustomerTransaction
        {
            TenantId = request.TenantId,
            CustomerTenantId = sourceCustomerTenant.Id,
            CustomerTenant = sourceCustomerTenant,
            PointId = request.PointId,
            Debit = request.Amount,
            Credit = null,
            Balance = sourceBalance - request.Amount,
            TransactionType = CustomerTransactionType.PointTransferOut,
            EventLogId = eventLog.Id
        };

        transactionCmdRepo.Add(sourceTransaction);

        // 11. Calculate Destination Balance
        var destTransactions = await transactionQueryRepo.GetAllAsync(
            cancellationToken,
            ct => ct.CustomerTenantId == destinationCustomerTenant.Id && ct.PointId == request.PointId && ct.TenantId == request.TenantId);
        
        var destBalance = destTransactions.OrderByDescending(ct => ct.Id).FirstOrDefault()?.Balance ?? 0;

        // 12. Create Credit Transaction for Destination Customer
        var destinationTransaction = new CustomerTransaction
        {
            TenantId = request.TenantId,
            CustomerTenantId = destinationCustomerTenant.Id,
            CustomerTenant = destinationCustomerTenant,
            PointId = request.PointId,
            Debit = null,
            Credit = request.Amount,
            Balance = destBalance + request.Amount,
            TransactionType = CustomerTransactionType.PointTransferIn,
            EventLogId = eventLog.Id
        };

        transactionCmdRepo.Add(destinationTransaction);

        // 13. Charge Commission if applicable
        if (commissionAmount > 0 && commissionPointId.HasValue)
        {
            var commissionTransactions2 = await transactionQueryRepo.GetAllAsync(
                cancellationToken,
                ct => ct.CustomerTenantId == sourceCustomerTenant.Id && ct.PointId == commissionPointId && ct.TenantId == request.TenantId);
            
            var commissionBalance = commissionTransactions2.OrderByDescending(ct => ct.Id).FirstOrDefault()?.Balance ?? 0;

            var commissionTransaction = new CustomerTransaction
            {
                TenantId = request.TenantId,
                CustomerTenantId = sourceCustomerTenant.Id,
                CustomerTenant = sourceCustomerTenant,
                PointId = commissionPointId.Value,
                Debit = commissionAmount,
                Credit = null,
                Balance = commissionBalance - commissionAmount,
                TransactionType = CustomerTransactionType.TransferCommission,
                EventLogId = eventLog.Id
            };

            transactionCmdRepo.Add(commissionTransaction);
        }

        // 14. Save all changes
        await transactionCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new PointTransferResult
        {
            SourceTransactionId = sourceTransaction.Id,
            DestinationTransactionId = destinationTransaction.Id,
            EventLogId = eventLog.Id,
            AmountTransferred = request.Amount,
            CommissionCharged = commissionAmount > 0 ? commissionAmount : null
        };
    }
}

