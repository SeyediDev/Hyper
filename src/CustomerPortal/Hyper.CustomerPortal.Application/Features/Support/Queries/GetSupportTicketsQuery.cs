using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.CallCenter;
using Hyper.Domain.Entities.CallCenter.Enums;
using Hyper.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Features.Support.Queries;

public record GetSupportTicketsQuery : IRequest<GetSupportTicketsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public InteractionStatus? Status { get; set; }
}

public record GetSupportTicketsQueryResponse
{
    public PaginatedList<SupportTicketDto> Tickets { get; set; } = null!;
}

public record SupportTicketDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public InteractionStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public PriorityLevel Priority { get; set; }
    public string PriorityName { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? DurationMinutes { get; set; }
    public int? SatisfactionScore { get; set; }
    public string? CustomerFeedback { get; set; }
    public string? TicketId { get; set; }
    public string? InteractionTypeName { get; set; }
}

public class GetSupportTicketsQueryHandler(
    ICustomerRequesterUser requesterUser,
    IQueryRepository<CustomerInteraction, long> interactionRepository,
    IQueryRepository<CustomerTenant> customerTenantRepository) : IRequestHandler<GetSupportTicketsQuery, GetSupportTicketsQueryResponse>
{
    public async Task<GetSupportTicketsQueryResponse> Handle(GetSupportTicketsQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var tenantId = requesterUser.TenantId;

        var customerTenant = await customerTenantRepository
            .FirstOrDefaultAsync(ct => ct.CustomerId == customerId && ct.TenantId == tenantId && !ct.IsDeleted, cancellationToken);

        if (customerTenant == null)
        {
            return new GetSupportTicketsQueryResponse
            {
                Tickets = new PaginatedList<SupportTicketDto>([], 0, request.PageNumber, request.PageSize)
            };
        }

        var query = interactionRepository.Query()
            .Where(i => i.CustomerTenantId == customerTenant.Id && !i.IsDeleted);

        if (request.Status.HasValue)
        {
            query = query.Where(i => i.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var interactions = await query
            .Include(i => i.InteractionType)
            .OrderByDescending(i => i.StartTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var ticketDtos = interactions.Select(i => new SupportTicketDto
        {
            Id = i.Id,
            Title = i.Title,
            Description = i.Description,
            Status = i.Status,
            StatusName = GetStatusName(i.Status),
            Priority = i.Priority,
            PriorityName = GetPriorityName(i.Priority),
            StartTime = i.StartTime,
            EndTime = i.EndTime,
            DurationMinutes = i.DurationMinutes,
            SatisfactionScore = i.SatisfactionScore,
            CustomerFeedback = i.CustomerFeedback,
            TicketId = i.TicketId,
            InteractionTypeName = i.InteractionType?.Title
        }).ToList();

        return new GetSupportTicketsQueryResponse
        {
            Tickets = new PaginatedList<SupportTicketDto>(ticketDtos, totalCount, request.PageNumber, request.PageSize)
        };
    }

    private static string GetStatusName(InteractionStatus status)
    {
        return status switch
        {
            InteractionStatus.InProgress => "در حال انجام",
            InteractionStatus.Completed => "تکمیل شده",
            InteractionStatus.Cancelled => "لغو شده",
            InteractionStatus.PendingFollowUp => "در انتظار پیگیری",
            InteractionStatus.Closed => "بسته شده",
            _ => status.ToString()
        };
    }

    private static string GetPriorityName(PriorityLevel priority)
    {
        return priority switch
        {
            PriorityLevel.None => "بدون اولویت",
            PriorityLevel.Low => "پایین",
            PriorityLevel.Normal => "عادی",
            PriorityLevel.Medium => "متوسط",
            PriorityLevel.High => "بالا",
            _ => priority.ToString()
        };
    }
}