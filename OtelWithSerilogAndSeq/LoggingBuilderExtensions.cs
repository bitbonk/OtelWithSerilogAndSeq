using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SerilogTracing;

namespace OtelWithSerilogAndSeq;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddDefaultLoggingWithSeq(this ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Trace);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Destructure.ToMaximumDepth(3)
            .Destructure.ToMaximumCollectionCount(10)
            .Destructure.ToMaximumStringLength(100)
            .WriteTo.Seq("http://localhost:5341")
            .Enrich.WithProperty(
                "Application", // TODO: use process.executable.name ?
                Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty)
            .WriteTo.Logger(c => c
                .Enrich.With<ExceptionMessageEnricher>()
                .WriteTo.Console(
                    LogEventLevel.Information,
                    "{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3} {SourceContext}: {Message} {ExceptionMessage} {NewLine}",
                    theme: AnsiConsoleTheme.Code))
            .CreateLogger();

        // from https://github.com/serilog-tracing/serilog-tracing?tab=readme-ov-file#enabling-tracing-with-activitylistenerconfigurationtracetosharedlogger
        // TODO: TraceToSharedLogger returns an IDisposable, it must not be disposed before the app terminates,
        // because then tracing would not work anymore. How can we properly dispose it? 
        _ = new ActivityListenerConfiguration()
            .TraceToSharedLogger();

        builder.AddSerilog(dispose: true);

        return builder;
    }
}