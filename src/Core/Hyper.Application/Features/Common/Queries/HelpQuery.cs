using Neo.Application.Exceptions;

namespace Hyper.Application.Features.Common.Queries;

public record HelpQuery() : IRequest<HelpQueryResponse>
{
}

public record HelpQueryResponse
{
    public string Content { get; set; } = null!;
}

public class HelpQueryHandler(IRequesterUser user, IQueryRepository<Help, int> queryRepository) : IRequestHandler<HelpQuery, HelpQueryResponse>
{
    public async Task<HelpQueryResponse> Handle(HelpQuery request, CancellationToken cancellationToken)
    {
        var userLangId = await user.GetLangIdAsync(cancellationToken);
        var help = await queryRepository.FirstOrDefaultAsync(x => x.LanguageId == userLangId, cancellationToken);
        if (help == null) throw new BadRequestException("محتوایی یافت نشد.");
        return new HelpQueryResponse
        {
            Content = help.Content
        };
    }

}
