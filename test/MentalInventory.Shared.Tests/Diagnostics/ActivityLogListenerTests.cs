using System.Diagnostics;
using MentalInventory.Shared.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MentalInventory.Shared.Tests.Diagnostics;

public class ActivityLogListenerTests
{
    [Fact]
    public void Finished_activities_from_the_app_are_logged()
    {
        var logger = new ListLogger<ActivityLogListener>();
        using var listener = new ActivityLogListener(logger);

        using (var activity = Telemetry.ActivitySource.StartActivity("Save inventory"))
        {
            Assert.NotNull(activity);
        }

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Debug, entry.Level);
        Assert.Contains("Save inventory", entry.Message);
    }

    [Fact]
    public void Activities_from_other_sources_are_ignored()
    {
        var logger = new ListLogger<ActivityLogListener>();
        using var listener = new ActivityLogListener(logger);
        using var other = new ActivitySource("Someone.Else");

        using (other.StartActivity("Not ours"))
        {
        }

        Assert.Empty(logger.Entries);
    }

    [Fact]
    public void The_version_is_never_empty()
    {
        Assert.False(string.IsNullOrWhiteSpace(Telemetry.ServiceVersion));
    }

    private sealed class ListLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Entries.Add((logLevel, formatter(state, exception)));
    }
}
