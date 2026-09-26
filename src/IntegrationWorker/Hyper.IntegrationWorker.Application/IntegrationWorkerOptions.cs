namespace Hyper.IntegrationWorker.Application;

public sealed class IntegrationWorkerOptions
{
    public TimeSpan CaptureInterval { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan IdleDelay { get; set; } = TimeSpan.FromSeconds(5);

    public bool IsValid() => Valid(CaptureInterval) && Valid(IdleDelay);

    private static bool Valid(TimeSpan interval) =>
        interval >= TimeSpan.FromMilliseconds(1) && interval <= TimeSpan.FromHours(1);
}
