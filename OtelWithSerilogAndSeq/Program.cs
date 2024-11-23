using OtelWithSerilogAndSeq;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddHostedService<Worker>()
    .AddSingleton<Instrumentation>();
builder.Logging.AddDefaultLoggingWithSeq();

var host = builder.Build();
host.Run();