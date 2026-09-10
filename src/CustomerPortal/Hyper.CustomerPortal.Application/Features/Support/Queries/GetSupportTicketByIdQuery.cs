using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.CallCenter;
using Hyper.Domain.Entities.CallCenter.Enums;
using Hyper.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Features.Support.Queries;

public record GetSupportTicketByIdQuery : IRequest<GetSupportTicketByIdQueryResponse>
{
    public long Id { get; set; }
}

public record GetSupportTicketByIdQueryResponse
{
    public SupportTicketDetailDto? Ticket { get; set; }
}

public record SupportTicketDetailDto
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
    public string? OutcomeNotes { get; set; }
    public string? TicketId { get; set; }
    public string? CallId { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public string? InteractionTypeName { get; set; }
    public string? AgentName { get; set; }
    public bool RequiresFollowUp { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public List<InteractionFollowUpDto> FollowUps { get; set; } = [];
}

public record InteractionFollowUpDto
{
    public long Id { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetSupportTicketByIdQueryHandler(
    ICustomerRequesterUser requesterUser,
    IQueryRepository<CustomerInteraction, long> interactionRepository,
    IQueryRepository<CustomerTenant> customerTenantRepository)
    : IRequestHandler<GetSupportTicketByIdQuery, GetSupportTicketByIdQueryResponse>
{
    public async Task<GetSupportTicketByIdQueryResponse> Handle(GetSupportTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = requesterUser.CustomerId;
        var tenantId = requesterUser.TenantId;

        var customerTenant = await customerTenantRepository
            .FirstOrDefaultAsync(ct => ct.CustomerId == userId && ct.TenantId == tenantId && !ct.IsDeleted, cancellationToken);

        if (customerTenant == null)
        {
            return new GetSupportTicketByIdQueryResponse { Ticket = null };
        }

		var interaction = await interactionRepository.Query()
            .Include(i=> i.InteractionType)
            .Include(i=> i.AgentUser)
            .Include(i=> i.FollowUps)
            .FirstOrDefaultAsync(
                i => i.Id == request.Id && i.CustomerTenantId == customerTenant.Id && !i.IsDeleted, cancellationToken);

        if (interaction == null)
        {
            return new GetSupportTicketByIdQueryResponse { Ticket = null };
        }

        var ticketDto = new SupportTicketDetailDto
        {
            Id = interaction.Id,
            Title = interaction.Title,
            Description = interaction.Description,
            Status = interaction.Status,
            StatusName = GetStatusName(interaction.Status),
            Priority = interaction.Priority,
            PriorityName = GetPriorityName(interaction.Priority),
            StartTime = interaction.StartTime,
            EndTime = interaction.EndTime,
            DurationMinutes = interaction.DurationMinutes,
            SatisfactionScore = interaction.SatisfactionScore,
            CustomerFeedback = interaction.CustomerFeedback,
            OutcomeNotes = interaction.OutcomeNotes,
            TicketId = interaction.TicketId,
            CallId = interaction.CallId,
            ContactPhone = interaction.ContactPhone,
            ContactEmail = interaction.ContactEmail,
            InteractionTypeName = interaction.InteractionType?.Title,
            AgentName = interaction.AgentUser != null ? $"{interaction.AgentUser.FirstName} {interaction.AgentUser.LastName}" : null,
            RequiresFollowUp = interaction.RequiresFollowUp,
            FollowUpDate = interaction.FollowUpDate,
            FollowUps = interaction.FollowUps.Select(f => new InteractionFollowUpDto
            {
                Id = f.Id,
                Notes = f.Notes,
                CreatedAt = f.CreateDate
            }).ToList()
        };

        return new GetSupportTicketByIdQueryResponse { Ticket = ticketDto };
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