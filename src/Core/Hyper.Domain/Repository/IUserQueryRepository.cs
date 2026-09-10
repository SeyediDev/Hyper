using User = Hyper.Domain.Entities.Common.User;

namespace Hyper.Domain.Repository;

public interface IUserQueryRepository : IQueryRepository<User, UserId>
{
    Task<User?> GetByMobileAsync(long mobile, int countryCode, CancellationToken cancellationToken);
}
