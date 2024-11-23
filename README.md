This Repo tries to make OTEL's traces and spans working with Serilog and Seq 
as demonstrated here https://github.com/serilog-tracing/serilog-tracing

The main difference here is that Serilog is not used for logging and tracing but instead the `Microsoft.Extensions.Logging` 
abstraction and `System.Diagnostics.ActivitySource` are used.
Serilog is merely used as for the sinks.

In Seq the traces and spans do not show up:

![Seq Screenshot without traces and spans](SeqScreenshot.png)