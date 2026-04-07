using Serilog.Core;
using Serilog.Events;
using Spectre.Console;

namespace BeautifulClient.UI.Components;

public class SerilogQueSink(IFormatProvider? formatProvider = null) : ILogEventSink
{
    private static Queue<string> LogQueue { get; } = [];
    private int MaxLogs { get; set; } = 10;

    /// <inheritdoc/>>
    public void Emit(LogEvent logEvent)
    {
        LogEventLevel level = logEvent.Level;
        
        string message = Markup.Escape(logEvent.RenderMessage());
        string timestamp = logEvent.Timestamp.ToString("HH:mm:ss");
        
        var colour = level switch
        {
            LogEventLevel.Warning => "yellow",
            LogEventLevel.Error or LogEventLevel.Fatal => "red",
            LogEventLevel.Debug or LogEventLevel.Verbose => "grey",
            _ => "white"
        };

        string messageBuild = $"[{colour}]{timestamp} {logEvent.Level,-7:G}[/] {Markup.Escape(message)}";
        
        lock (LogQueue)
        {
            if (LogQueue.Count >= MaxLogs)
                LogQueue.Dequeue();
            
            LogQueue.Enqueue(messageBuild);
        }
    }

    public static string[] GetLogs()
    {
        lock (LogQueue)
        {
            return LogQueue.ToArray();
        }
    }

    /// <summary>
    /// Removes all queued log entries.
    /// </summary>
    public static void ClearLogs()
    {
        lock (LogQueue)
        {
            LogQueue.Clear();
        }
    }
}
