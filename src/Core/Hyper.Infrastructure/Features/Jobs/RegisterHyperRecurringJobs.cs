using Neo.Application.Features.Queue;
using Microsoft.Extensions.Logging;

namespace Hyper.Infrastructure.Features.Jobs;

/// <summary>
/// ثبت jobهای دوره‌ای (recurring jobs) سیستم باشگاه
/// این کلاس باید در Application layer استفاده شود
/// </summary>
public class RegisterHyperRecurringJobs(
    IRecurringJobsManager recurringJobsManager,
    ILogger<RegisterHyperRecurringJobs> logger) : IRegisterRecurringJobs
{
    public void Register()
    {
        logger.LogInformation("Registering Hyper recurring jobs at {Time}", DateTime.Now);
        
        // Get job types dynamically from service provider
        var jobTypes = new[]
        {
            "IProcessScheduledLotteriesJob",
            "IProcessScheduledPromotionsJob",
            "IScheduleLotteriesJob",
            "IExpireCustomerPlansJob"
        };

        foreach (var jobTypeName in jobTypes)
        {
            try
            {
                // Find type in loaded assemblies
                var jobType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == jobTypeName && typeof(IRecurringJob).IsAssignableFrom(t));

                if (jobType != null)
                {
                    logger.LogInformation("Registering {JobName}", jobTypeName);
                    
                    // Call AddOrUpdate via reflection
                    var method = typeof(IRecurringJobsManager).GetMethod("AddOrUpdate");
                    var genericMethod = method?.MakeGenericMethod(jobType);
                    genericMethod?.Invoke(recurringJobsManager, null);
                }
                else
                {
                    logger.LogWarning("Job type {JobName} not found", jobTypeName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error registering job {JobName}", jobTypeName);
            }
        }
        
        logger.LogInformation("Successfully registered all Hyper recurring jobs");
    }
}

