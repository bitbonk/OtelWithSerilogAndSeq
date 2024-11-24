using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using SerilogTracing;
using ILogger = Serilog.ILogger;

namespace OtelWithSerilogAndSeq;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddDefaultLoggingWithSeq(this ILoggingBuilder builder)
    {
        builder.ClearProviders();
        builder.SetMinimumLevel(LogLevel.Trace);
        
        var logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Destructure.ToMaximumDepth(3)
            .Destructure.ToMaximumCollectionCount(10)
            .Destructure.ToMaximumStringLength(100)
            .WriteTo.Seq("http://localhost:5341")
            .Enrich.WithProperty(
                "Application", // TODO: use process.executable.name ?
                Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty)
            .WriteTo.Logger(
                c => c
                    .Enrich.With<ExceptionMessageEnricher>()
                    .WriteTo.Console(
                        LogEventLevel.Information,
                        "{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3} {SourceContext}: {Message} {ExceptionMessage} {NewLine}",
                        theme: AnsiConsoleTheme.Code))
            .CreateLogger();

        Log.Logger = new DisposingLoggerAdapter(logger);
        
        // from https://github.com/serilog-tracing/serilog-tracing?tab=readme-ov-file#enabling-tracing-with-activitylistenerconfigurationtracetosharedlogger
        var listener = new ActivityListenerConfiguration()
            .TraceToSharedLogger();
        
        ((DisposingLoggerAdapter)Log.Logger).SetListener(listener);
        
        builder.AddSerilog();
        
        return builder;
    }

    private sealed class DisposingLoggerAdapter : ILogger, IDisposable
    {
        private readonly ILogger _logger;
        private IDisposable? _listener;

        public DisposingLoggerAdapter(ILogger logger)
        {
            _logger = logger;
        }
        
        public void SetListener(IDisposable listener) => _listener = listener;
        
        public void Write(LogEvent logEvent)
        {
            _logger.Write(logEvent);
        }

        public void Dispose()
        {
            if (_logger is IDisposable disposable)
            {
                disposable.Dispose();
            }
            
            _listener?.Dispose();
        }
    }
}