namespace Hyper.Application.Features.Common.Queries;

public record FaqQuery() : IRequest<FaqQueryResponse>
{
}

public record FaqQueryResponse(List<FaqDto> Items)
{
}

public record FaqDto
{
    public string Question { get; set; } = null!;
    public string Answer { get; set; } = null!;
}

public class FaqQueryHandler(IRequesterUser user
    , IQueryRepository<Faq, int> queryRepository
    ) : IRequestHandler<FaqQuery, FaqQueryResponse>
{
    public async Task<FaqQueryResponse> Handle(FaqQuery request, CancellationToken cancellationToken)
    {
        var userLangId = await user.GetLangIdAsync(cancellationToken);
        List<FaqDto> faqs = (await queryRepository.GetAllAsync(cancellationToken, x => x.LanguageId == userLangId ))
            .Select(x => new FaqDto
        {
            Question = x.Question,
            Answer = x.Answer
        }).ToList();
        return new FaqQueryResponse(faqs);
    }
}
