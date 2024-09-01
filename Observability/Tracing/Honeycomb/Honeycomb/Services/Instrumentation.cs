using System.ComponentModel;
using System.Diagnostics;

namespace Honeycomb.Services;

/* Anywhere in your application where you write manual tracing code should configure an ActivitySource, which will
be how you trace operations with Activity elements. A .NET activity becomes an OpenTelemetry span.

It’s recommended to define ActivitySource once per app/service but you can instantiate several ActivitySources if that
suits your scenario. */

/// <summary>
/// It is recommended to use a custom type to hold references for ActivitySource.
/// This avoids possible type collisions with other components in the DI container.
/// </summary>
public class Instrumentation : IDisposable
{
    public Instrumentation(string sourceName, string sourceVersion)
    {
        ActivitySource = new ActivitySource(sourceName, sourceName);
    }

    public ActivitySource ActivitySource { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
    }
}

internal static class TraceTags
{
    public const string ControllerMethod = "service.controller.method";
}