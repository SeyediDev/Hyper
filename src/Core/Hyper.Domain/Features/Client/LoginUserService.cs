using Neo.Domain.Entities;
using Neo.Domain.Features.Client;
using Hyper.Domain.Repository;
using User = Hyper.Domain.Entities.Common.User;

namespace Hyper.Domain.Features.Client;

internal class LoginUserService(
    IIdpService idpService, IRequesterUser requesterUser,
    IUserQueryRepository userRepository, ICommandRepository<User, UserId> userCommandRepo
    ) : ILoginUserService<UserId>
{
    public async Task<IUser<UserId>?> FindUser(string mobile, int countryCode, CancellationToken cancellationToken)
    {
        return await userRepository.GetByMobileAsync(long.Parse(mobile), countryCode, cancellationToken);
    }
    public async Task<IUser<UserId>> RegisterUser(string mobile, int countryCode, byte[] otpSeed,
        Func<IUser<UserId>, Task>? SetUseParametersInRegistration, CancellationToken cancellationToken)
    {
        User user = new()
        {
            Mobile = long.Parse(mobile),
            CountryCode = countryCode,
            OTPSeed = otpSeed,
            Verified = false,
        };
        SetUseParametersInRegistration?.Invoke(user);
        userCommandRepo.Add(user);
        await userCommandRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task VerifyUser(string mobile, int countryCode, CancellationToken cancellationToken)
    {
        var userEntity = await userRepository.GetByMobileAsync(long.Parse(mobile), countryCode, cancellationToken);
        var idpUser = await idpService.GetUserAsync(mobile);
        var role = requesterUser.AppName?.ToLower()!;
        if (idpUser is null)
        {
            idpUser = await idpService.AddUserAsync(userEntity!.Id.ToString(), mobile, role);
        }
        else
        {
            if (idpUser.Roles == null || !idpUser.Roles.Any(r => r == role))
            {
                await idpService.AddUserRoleAsync(idpUser.id, role);
                //idpUser.Roles ??= [];
                //idpUser.Roles.Add(role);
            }
        }
    }
}
