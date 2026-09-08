using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Points.Queries;

public record GetReferralPointsQuery : IRequest<GetReferralPointsQueryResponse>;

public record GetReferralPointsQueryResponse
{
    public List<PointTransactionDto> Transactions { get; set; } = [];
}

public class GetReferralPointsQueryHandler(
    IPointService pointService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<GetReferralPointsQuery, GetReferralPointsQueryResponse>
{
    public async Task<GetReferralPointsQueryResponse> Handle(GetReferralPointsQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        // دریافت تراکنش‌های امتیاز مربوط به معرفی
        var allTransactions = await pointService.GetPointTransactionsAsync(customerId, 1, 100, cancellationToken);
        
        // فیلتر کردن تراکنش‌هایی که مربوط به معرفی هستند
        var referralTransactions = allTransactions.Items
            .Where(t => t.Type.Contains("Referral", StringComparison.OrdinalIgnoreCase) || 
                       t.Title.Contains("معرفی", StringComparison.OrdinalIgnoreCase))
            .Select(t => new PointTransactionDto
            {
                Id = t.Id.ToString(),
                TransactionDate = t.CreatedAt,
                PointTypeName = "امتیاز طلایی",
                PointTypeColor = "#FFD700",
                Amount = (int)t.Amount,
                TransactionType = t.Type,
                Description = t.Description ?? t.Title,
                ReferenceId = null,
                ReferenceType = "Referral",
                ExpirationDate = null,
                Status = "Completed"
            }).ToList();
        
        return new GetReferralPointsQueryResponse
        {
            Transactions = referralTransactions
        };
    }
}

