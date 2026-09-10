using Hyper.Domain.Entities.Common;
using Neo.Bpms.Domain.Models.Security.Authentication;
using Neo.Bpms.Domain.Models.Security.Authorization;
using Neo.Common.Extensions;
using Neo.Domain.Constants;
using Neo.Domain.Entities.Common;

namespace Hyper.AdminPanel.Web.Infrastructure;

internal class IdentityUserService(IQueryRepository<User, UserId> userRepository, IRequesterUser requesterUser)
    : IIdentityUserService
{
    public async Task<IdentityUser> GetIdentityUserAsync(string userName, CancellationToken cancellationToken = default)
    {
        long mobile = userName.ToInt64OrDefault();
        User? userRecord = await userRepository.FirstOrDefaultAsync(x => x.Mobile == mobile, cancellationToken);
        if (userRecord != null)
        {
            var user = userRecord.ExpireDate is null ?
                new IdentityUser(userRecord.Id.ToString())
                {
                    UserName = userRecord.Mobile.ToString(),
                    OTPSeed = userRecord.OTPSeed,
                    MobileNo = userRecord.Mobile.ToString(),
                    PhoneNumber = userRecord.Mobile.ToString(),
                    NationalNumber = userRecord.NationalCode?.ToString("D10"),
                    Culture = requesterUser.Lang,
                    FirstName = userRecord.FirstName,
                    LastName = userRecord.LastName,
                    CreationDate = userRecord.CreateDate,
                    StartDate = userRecord.CreateDate,
                    EndDate = userRecord.ExpireDate,
                    IsAdmin = true
                } : null;
            user?.SetRoles(requesterUser.Claims());
            return user!;
        }
        return null!;
    }

    public async Task<List<IdentityRole>> GetIdentityRoles()
    {
        await Task.CompletedTask;
        return [ new IdentityRole() { Code=Roles.Admin, Name = Roles.Admin, Id=1 } ];
    }
}