This Repo tries to make OTEL's traces and spans working with Serilog and Seq 
as demonstrated here https://github.com/serilog-tracing/serilog-tracing

The main difference here is that Serilog is not used for logging and tracing but instead the `Microsoft.Extensions.Logging` 
abstraction and `System.Diagnostics.ActivitySource` are used.

The idea is that a framework or library that does the actual logging does not need a dependency to Serilog or Seq.
Only at the application level the user would add Serilog and Seq as a sink.
