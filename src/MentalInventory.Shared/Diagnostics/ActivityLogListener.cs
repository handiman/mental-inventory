using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MentalInventory.Shared.Diagnostics;

/// <summary>
/// Writes every finished activity from <see cref="Telemetry.ActivitySource"/> to the log, so
/// traces show up in the browser console during development. A stand-in until a real
/// exporter exists, not a replacement for one.
/// </summary>
public sealed partial class ActivityLogListener : IDisposable
{
    private readonly ILogger logger;
    private readonly ActivityListener listener;

    public ActivityLogListener(ILogger<ActivityLogListener> logger)
    {
        this.logger = logger;
        listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == Telemetry.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity =>
                LogActivityStopped(activity.DisplayName, activity.Duration.TotalMilliseconds, activity.Status),
        };
        ActivitySource.AddActivityListener(listener);
    }

    public void Dispose() => listener.Dispose();

    [LoggerMessage(EventId = 1000, Level = LogLevel.Debug,
        Message = "Activity {ActivityName} took {DurationMs} ms, status {Status}")]
    private partial void LogActivityStopped(string activityName, double durationMs, ActivityStatusCode status);
}
