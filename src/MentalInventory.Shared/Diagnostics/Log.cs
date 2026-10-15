using Microsoft.Extensions.Logging;

namespace MentalInventory.Shared.Diagnostics;

/// <summary>
/// Source-generated log messages: structured, allocation-free when the level is off, and
/// with stable event ids that can be searched and alerted on later.
/// </summary>
/// <remarks>
/// Event id ranges: 1-99 app lifecycle, 100-999 features, 1000+ diagnostics plumbing.
/// Never reuse an id for a different message.
/// </remarks>
public static partial class Log
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information,
        Message = "{ServiceName} {ServiceVersion} started in {Environment}")]
    public static partial void Started(ILogger logger, string serviceName, string serviceVersion, string environment);
}
