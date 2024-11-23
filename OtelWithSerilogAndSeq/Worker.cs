using System.Diagnostics;

namespace OtelWithSerilogAndSeq;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ActivitySource _activitySource;

    public Worker(ILogger<Worker> logger, Instrumentation instrumentation)
    {
        _logger = logger;
        _activitySource = instrumentation.ActivitySource;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var counter = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            counter++;
            // ReSharper disable once ExplicitCallerInfoArgument
            using var myActivity = _activitySource.StartActivity("Execution");
            myActivity?.SetTag("ExecutionID", counter);
            _logger.LogInformation("Starting execution {ExecutionCount}", counter);
            await Task.Delay(1000, stoppingToken);
            await ExecuteSubActivityAsync(stoppingToken);
            _logger.LogInformation("Completed execution {ExecutionCount}", counter);
        }
    }

    private async Task ExecuteSubActivityAsync(CancellationToken stoppingToken)
    {
        for (var i = 0; i < 3; i++)
        {
            using var subActivity = _activitySource.StartActivity("Sub execution");
            _logger.LogInformation("  Starting sub execution {SubExecutionCount}", i);
            subActivity?.SetTag("Sub.ExecutionID", i);
            await Task.Delay(500, stoppingToken);
            _logger.LogInformation("  Completed sub execution {SubExecutionCount}", i);
        }
    }
}