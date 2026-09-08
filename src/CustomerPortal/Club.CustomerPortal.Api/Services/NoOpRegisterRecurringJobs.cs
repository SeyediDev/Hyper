using Neo.Application.Features.Queue;

namespace Hyper.CustomerPortal.Api.Services;

/// <summary>
/// No-op implementation of IRegisterRecurringJobs for CustomerPortal
/// CustomerPortal doesn't need recurring jobs, so this is a temporary stub
/// </summary>
internal sealed class NoOpRegisterRecurringJobs : IRegisterRecurringJobs
{
    public void Register()
    {
        // No-op: CustomerPortal doesn't need recurring jobs
    }
}

