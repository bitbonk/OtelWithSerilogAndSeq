using Serilog.Core;
using Serilog.Events;

namespace OtelWithSerilogAndSeq;

/// <summary>
///     Enriches a  <see cref="LogEvent" /> with the {ExceptionMessage} which contains the
///     <see cref="System.Exception.Message" />
///     of the <see cref="LogEvent.Exception" /> if present.
/// </summary>
public class ExceptionMessageEnricher : ILogEventEnricher
{
    /// <inheritdoc />
    public void Enrich(LogEvent? logEvent, ILogEventPropertyFactory? propertyFactory)
    {
        if (logEvent == null || propertyFactory == null || logEvent.Exception == null)
        {
            return;
        }

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty(
                "ExceptionMessage",
                logEvent.Exception.GetBaseException().Message));
    }
}