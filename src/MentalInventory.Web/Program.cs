using MentalInventory.Shared;
using MentalInventory.Shared.Diagnostics;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Logs go to the browser console. Our own categories are verbose in development; the
// framework only speaks up when something is wrong.
var isDevelopment = builder.HostEnvironment.IsDevelopment();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddFilter("MentalInventory", isDevelopment ? LogLevel.Debug : LogLevel.Information);
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("System", LogLevel.Warning);
builder.Services.AddSingleton<ActivityLogListener>();

var host = builder.Build();

if (isDevelopment)
{
    // Resolving the listener starts it. It is disposed with the host.
    host.Services.GetRequiredService<ActivityLogListener>();
}

Log.Started(
    host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("MentalInventory.Web"),
    Telemetry.ServiceName,
    Telemetry.ServiceVersion,
    builder.HostEnvironment.Environment);

await host.RunAsync();
