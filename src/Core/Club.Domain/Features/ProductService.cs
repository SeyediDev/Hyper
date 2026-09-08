
namespace Club.Domain.Features;

public interface IProductService
{
    [Telemetry]
    Task<PurchaseProductOrServiceResponse> PurchaseProductOrService(PurchaseProductOrServiceRequest request, CancellationToken cancellationToken);
}

public record PurchaseProductOrServiceRequest
{
    public string CustomerId { get; set; } = null!;
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public record PurchaseProductOrServiceResponse
{
    public string CustomerId { get; set; } = null!;
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public long? PointsAwarded { get; set; }
    public decimal TotalPrice { get; set; }
}

internal class ProductService(
    ILogger<ProductService> logger,
    ICustomerService customerService,
    IEventService eventService,
    IPromotionService promotionService,
    ICommandRepository<CustomerTransaction, long> customerTransactionCmdRepo,
    ICommandRepository<Product, int> productServiceCmdRepo,
    IProductPriceProvider? productPriceProvider = null
    ) : IProductService
{
    /// <summary>
    /// مشتری خرید محصول را انجام می‌دهد
    /// در این حالت مشتری امتیاز کسب می‌کند (نه امتیاز می‌پردازد)
    /// </summary>
    public async Task<PurchaseProductOrServiceResponse> PurchaseProductOrService(PurchaseProductOrServiceRequest request, CancellationToken cancellationToken)
    {
        // Get customer
        Customer? customer = await customerService.GetCustomer(request.CustomerId, false, null, cancellationToken)
            ?? throw new NullReferenceException(nameof(customer)); // TODO 404

        // Get product/service
        Product? product = await productServiceCmdRepo.FirstOrDefaultAsync(
            x => x.Id == request.ProductId, cancellationToken)
            ?? throw new NullReferenceException("محصول یافت نشد"); // TODO 404

        if (!product.IsActive)
            throw new InvalidOperationException("محصول فعال نیست"); // TODO 400

        // دریافت قیمت (از کانال یا از فیلد ثابت)
        decimal? productPrice = product.Price;
        if (product.FetchPriceFromChannel && 
            !string.IsNullOrEmpty(product.PriceChannelKey) &&
            !string.IsNullOrEmpty(product.ChannelProductKey) &&
            productPriceProvider != null)
        {
            try
            {
                productPrice = await productPriceProvider.GetProductPriceAsync(
                    product.Id,
                    request.CustomerId,
                    cancellationToken);
                
                if (!productPrice.HasValue)
                {
                    logger.LogWarning(
                        "Failed to fetch price from channel for product {ProductId}, using default price",
                        request.ProductId);
                    productPrice = product.Price; // Fallback to default
                }
                else
                {
                    logger.LogInformation(
                        "Fetched price {Price} from channel for product {ProductId}",
                        productPrice.Value, request.ProductId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Error fetching price from channel for product {ProductId}, using default price",
                    request.ProductId);
                productPrice = product.Price; // Fallback to default
            }
        }

        // Record event
        EventResponse eventResponse = await eventService.RecordEventAsync(
            new(TriggerType.PurchaseProductOrService, request.CustomerId, null)
            {
                TenantId = product.TenantId,
                ProductId = request.ProductId
            }, cancellationToken);

        // Award points if configured
        long? pointsAwarded = null;
        if (product.PointsEarnable.HasValue && product.PointsEarnable.Value > 0)
        {
            // TODO: Determine which point to award (may need to query point configuration)
            // For now, assuming a default point exists
            IEnumerable<CustomerTransaction> customerBalances = await customerService.GetCustomerPointBalances(customer.Id, product.TenantId, cancellationToken);
            
            // Find the first available point
            var firstPointBalance = customerBalances.FirstOrDefault();
            if (firstPointBalance != null)
            {
                long pointsToAward = product.PointsEarnable.Value * request.Quantity;
                
                CustomerTransaction creditTransaction = new()
                {
                    TransactionType = CustomerTransactionType.Credit,
                    Credit = pointsToAward,
                    Balance = firstPointBalance.Balance + pointsToAward,
                    TenantId = product.TenantId,
                    CustomerTenantId = firstPointBalance.CustomerTenantId,
                    CustomerTenant = firstPointBalance.CustomerTenant,
                    PointId = firstPointBalance.PointId,
                    EventLogId = eventResponse.EventLogId,
                };
                customerTransactionCmdRepo.Add(creditTransaction);
                await customerTransactionCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
                
                pointsAwarded = pointsToAward;
                logger.LogInformation("Awarded {Points} points for product/service purchase", pointsToAward);
            }
        }

        // Update product/service statistics
        product.PurchaseCount += request.Quantity;
        if (productPrice.HasValue)
        {
            product.TotalRevenue += productPrice.Value * request.Quantity;
        }
        productServiceCmdRepo.Update(product);
        await productServiceCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Trigger scoring rules
        await promotionService.ProcessEventAsync(
            new PromotionProcessingRequest(
                product.TenantId,
                (int)eventResponse.EventLogId,
                0,
                0,
                eventResponse.Customer,
                null)
            {
                TriggerType = TriggerType.PurchaseProductOrService,
                ProductId = product.Id
            }, cancellationToken);

        decimal totalPrice = productPrice.HasValue ? productPrice.Value * request.Quantity : 0;
        
        return new PurchaseProductOrServiceResponse
        {
            CustomerId = request.CustomerId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            PointsAwarded = pointsAwarded,
            TotalPrice = totalPrice
        };
    }
}
