using Serilog;

namespace DockerRegistryExplorer;

public static class TaskExtensions
{
    /// <summary>
    ///     Explicitly fire-and-forget a task, logging any exception instead of leaving it unobserved.
    /// </summary>
    public static void IgnoreAsync(this Task task)
    {
        task.ContinueWith(
            t => Log.Error(t.Exception, "Unobserved exception in fire-and-forget task"),
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default);
    }
}
