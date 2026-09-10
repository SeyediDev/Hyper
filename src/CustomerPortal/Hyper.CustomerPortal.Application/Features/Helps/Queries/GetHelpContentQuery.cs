using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Features.Helps.Queries;

public record GetHelpContentQuery : IRequest<GetHelpContentQueryResponse>
{
    public LanguageId? LanguageId { get; set; }
}

public record GetHelpContentQueryResponse
{
    public List<HelpContentDto> HelpContents { get; set; } = [];
}

public record HelpContentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int SortIndex { get; set; }
}

public class GetHelpContentQueryHandler(IQueryRepository<Domain.Entities.Common.Help> helpRepository) 
    : IRequestHandler<GetHelpContentQuery, GetHelpContentQueryResponse>
{
    public async Task<GetHelpContentQueryResponse> Handle(GetHelpContentQuery request, CancellationToken cancellationToken)
    {
        var query = helpRepository.Query()
            .Where(h => !h.IsDeleted);

        if (request.LanguageId.HasValue)
        {
            query = query.Where(h => h.LanguageId == request.LanguageId.Value);
        }

        var helpContents = await query
            .OrderBy(h => h.Id)
            .Select(h => new HelpContentDto
            {
                Id = h.Id,
                Title = h.Content.Substring(0, Math.Min(100, h.Content.Length)), // استفاده از بخشی از محتوا به عنوان عنوان
                Content = h.Content,
                SortIndex = h.Id
            })
            .ToListAsync(cancellationToken);

        return new GetHelpContentQueryResponse
        {
            HelpContents = helpContents
        };
    }
}