using Hyper.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Features.Helps.Queries;

public record GetFaqsQuery : IRequest<GetFaqsQueryResponse>
{
    public int? CategoryId { get; set; }
    public LanguageId? LanguageId { get; set; }
}

public record GetFaqsQueryResponse
{
    public List<FaqCategoryDto> Categories { get; set; } = [];
    public List<FaqDto> Faqs { get; set; } = [];
}

public record FaqCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int SortIndex { get; set; }
}

public record FaqDto
{
    public int Id { get; set; }
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
    public int SortIndex { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
}

public class GetFaqsQueryHandler(
    IQueryRepository<Faq> faqRepository,
    IQueryRepository<FaqCategory> faqCategoryRepository) : IRequestHandler<GetFaqsQuery, GetFaqsQueryResponse>
{
    public async Task<GetFaqsQueryResponse> Handle(GetFaqsQuery request, CancellationToken cancellationToken)
    {
        var categories = await faqCategoryRepository.Query()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Id)
            .Select(c => new FaqCategoryDto
            {
                Id = c.Id,
                Name = c.Title,
                SortIndex = c.Id
            })
            .ToListAsync(cancellationToken);

        var faqQuery = faqRepository.Query()
            .Where(f => !f.IsDeleted);

        if (request.CategoryId.HasValue)
        {
            faqQuery = faqQuery.Where(f => f.FaqCategoryId == request.CategoryId.Value);
        }

        if (request.LanguageId.HasValue)
        {
            faqQuery = faqQuery.Where(f => f.LanguageId == request.LanguageId.Value);
        }

        var faqs = await faqQuery
            .Include(f => f.FaqCategory)
            .OrderBy(f => f.SortIndex)
            .Select(f => new FaqDto
            {
                Id = f.Id,
                Question = f.Question,
                Answer = f.Answer,
                SortIndex = f.SortIndex,
                CategoryId = f.FaqCategoryId,
                CategoryName = f.FaqCategory.Title
            })
            .ToListAsync(cancellationToken);

        return new GetFaqsQueryResponse
        {
            Categories = categories,
            Faqs = faqs
        };
    }
}