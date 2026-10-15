using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace MentalInventory.Shared.Diagnostics;

/// <summary>
/// The app's one tracing source and one meter. Built on the BCL types that OpenTelemetry
/// reads natively, so an exporter can be added later without touching any call site.
/// See docs/adr/0005.
/// </summary>
/// <remarks>
/// Without a listener, <see cref="ActivitySource.StartActivity(string, ActivityKind)"/>
/// returns null and instruments record nothing, so instrumenting code costs next to nothing
/// until something subscribes. Always use <c>?.</c> on the returned activity.
/// </remarks>
public static class Telemetry
{
    /// <summary>The service name to report as OpenTelemetry's <c>service.name</c>.</summary>
    public const string ServiceName = "mental-inventory";

    /// <summary>The name of the activity source and meter. Listeners subscribe by this name.</summary>
    public const string SourceName = "MentalInventory";

    public static string ServiceVersion { get; } =
        typeof(Telemetry).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? "0";

    public static ActivitySource ActivitySource { get; } = new(SourceName, ServiceVersion);

    public static Meter Meter { get; } = new(SourceName, ServiceVersion);
}
