using System.Diagnostics;

namespace OtelWithSerilogAndSeq;

// from https://opentelemetry.io/docs/languages/net/instrumentation/#setting-up-an-activitysource
/// <summary>
///     It is recommended to use a custom type to hold references for ActivitySource.
///     This avoids possible type collisions with other components in the DI container.
/// </summary>
public class Instrumentation : IDisposable
{
    internal const string ActivitySourceName = "my-server";
    internal const string ActivitySourceVersion = "1.0.0";

    public Instrumentation()
    {
        ActivitySource = new ActivitySource(ActivitySourceName, ActivitySourceVersion);
    }

    public ActivitySource ActivitySource { get; }

    public void Dispose()
    {
        ActivitySource.Dispose();
    }
}