using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.Customers.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Features.Transactions.Queries;

public record GetTransactionsQuery : IRequest<GetTransactionsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public CustomerTransactionType? TransactionType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public record GetTransactionsQueryResponse
{
    public PaginatedList<TransactionDto> Transactions { get; set; } = null!;
}

public record TransactionDto
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public CustomerTransactionType TransactionType { get; set; }
    public string TransactionTypeName { get; set; } = null!;
    public long? Credit { get; set; }
    public long? Debit { get; set; }
    public long Balance { get; set; }
    public string? Description { get; set; }
    public string? PointName { get; set; }
    public string? RewardName { get; set; }
    public string? PlanName { get; set; }
}

public class GetTransactionsQueryHandler(
    ICustomerRequesterUser requesterUser,
    IQueryRepository<CustomerTransaction, long> transactionRepository,
    IQueryRepository<CustomerTenant> customerTenantRepository) : IRequestHandler<GetTransactionsQuery, GetTransactionsQueryResponse>
{
    public async Task<GetTransactionsQueryResponse> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var userId = requesterUser.CustomerId;
        var tenantId = requesterUser.TenantId??0;

        var customerTenant = await customerTenantRepository.Query()
            .FirstOrDefaultAsync(ct => ct.CustomerId == userId && ct.TenantId == tenantId && !ct.IsDeleted, cancellationToken);

        if (customerTenant == null)
        {
            return new GetTransactionsQueryResponse
            {
                Transactions = new PaginatedList<TransactionDto>([], 0, request.PageNumber, request.PageSize)
            };
        }

        var query = transactionRepository.Query()
            .Where(t => t.CustomerTenantId == customerTenant.Id && !t.IsDeleted);

        if (request.TransactionType.HasValue)
        {
            query = query.Where(t => t.TransactionType == request.TransactionType.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(t => t.CreateDate >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(t => t.CreateDate <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var transactions = await query
            .Include(t => t.Point)
            .Include(t => t.Reward)
            .Include(t => t.ActivePlan)
            .Include(t => t.Promotion)
            .OrderByDescending(t => t.CreateDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var transactionDtos = transactions.Select(t => new TransactionDto
        {
            Id = t.Id,
            CreatedAt = t.CreateDate,
            TransactionType = t.TransactionType,
            TransactionTypeName = GetTransactionTypeName(t.TransactionType),
            Credit = t.Credit,
            Debit = t.Debit,
            Balance = t.Balance,
            Description = GetTransactionDescription(t),
            PointName = t.Point?.Title,
            RewardName = t.Reward?.Title,
            PlanName = t.ActivePlan?.Title
		}).ToList();

        return new GetTransactionsQueryResponse
        {
            Transactions = new PaginatedList<TransactionDto>(transactionDtos, totalCount, request.PageNumber, request.PageSize)
        };
    }

    private static string GetTransactionTypeName(CustomerTransactionType type)
    {
        return type switch
        {
            CustomerTransactionType.Credit => "افزایش",
            CustomerTransactionType.Debit => "کاهش",
            _ => type.ToString()
        };
    }

    private static string? GetTransactionDescription(CustomerTransaction transaction)
    {
        if (transaction.Reward != null)
        {
            return $"خرید پاداش: {transaction.Reward.Title}";
        }

        if (transaction.ActivePlan != null)
        {
            return $"خرید طرح: {transaction.ActivePlan.Title}";
        }

        if (transaction.Promotion != null)
        {
            return $"امتیاز از پویش: {transaction.Promotion.Title}";
        }

        return null;
    }
}

