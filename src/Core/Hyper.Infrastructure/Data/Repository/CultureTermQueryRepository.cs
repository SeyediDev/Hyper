using Neo.Domain.Entities.Common;
using Neo.Domain.Features.Client;
using Neo.Domain.Repository;
using Hyper.Domain.Repository;
using Hyper.Infrastructure.Data.Repository.Hyper;

namespace Hyper.Infrastructure.Data.Repository;
public class CultureTermQueryRepository(IHyperUnitOfWorkQuery queryUnitOfWork, IRequesterUser requesterUser)
    : QueryHyperEntityRepository<CultureTerm, int>(queryUnitOfWork), ICultureTermQueryRepository
{
    public async Task<Dictionary<int, List<CultureTerm>>> GetTerms(string subjectTitle, List<int> subjectIds, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(requesterUser.Lang))
        {
            // Or throw an exception, depending on your requirements  
            return [];
        }
        var list = await GetAllAsync(cancellationToken, x =>
            x.SubjectTitle == subjectTitle &&
            x.Language.Name == requesterUser.Lang &&
            subjectIds.Contains(x.SubjectId));

        return list.GroupBy(x => x.SubjectId).ToDictionary(x => x.Key, x => x.ToList()); // Explicitly create the dictionary  
    }

    public async Task<Dictionary<int, List<CultureTerm>>> GetTerms(string subjectTitle, string subjectField, List<int> subjectIds, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(requesterUser.Lang))
        {
            // Or throw an exception, depending on your requirements  
            return [];
        }
        var list = await GetAllAsync(cancellationToken, x =>
            x.SubjectTitle == subjectTitle &&
            x.SubjectField == subjectField &&
            x.Language.Name == requesterUser.Lang &&
            subjectIds.Contains(x.SubjectId));

        return list.GroupBy(x => x.SubjectId).ToDictionary(x => x.Key, x => x.ToList()); // Explicitly create the dictionary  
    }
}
