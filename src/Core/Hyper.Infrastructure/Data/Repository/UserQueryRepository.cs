using Hyper.Domain.Entities.Common;
using Hyper.Domain.Repository;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Neo.Domain.Entities.Common;

namespace Hyper.Infrastructure.Data.Repository;
public class UserQueryRepository(IHyperUnitOfWorkQuery queryUnitOfWork)
    : QueryHyperEntityRepository<User, UserId>(queryUnitOfWork), IUserQueryRepository
{
    public async Task<User?> GetByMobileAsync(long mobile, int countryCode, CancellationToken cancellationToken)
    {
        return await FirstOrDefaultAsync(x => 
            x.Mobile == mobile && 
            x.CountryCode == countryCode, cancellationToken);
    }
}
