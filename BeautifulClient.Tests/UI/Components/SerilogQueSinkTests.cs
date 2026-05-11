using BeautifulClient.Utilities;
using Serilog;

namespace BeautifulClient.Tests.UI.Components;

public class SerilogQueSinkTests
{
    [Fact]
    public void Emit_QueuesFormattedLogEntry()
    {
        SerilogQueSink.ClearLogs();
        var sink = new SerilogQueSink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .CreateLogger();

        logger.Warning("Fan {FanId} became {State}", 2, "OFF");

        string[] logs = SerilogQueSink.GetLogs();
        Assert.Single(logs);
        Assert.Contains("Warning", logs[0]);
        Assert.Contains("Fan 2 became", logs[0]);
        Assert.Contains("OFF", logs[0]);
    }

    [Fact]
    public void Emit_DropsOldestEntries_WhenQueueExceedsMaximum()
    {
        SerilogQueSink.ClearLogs();
        var sink = new SerilogQueSink();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(sink)
            .CreateLogger();

        for (int i = 1; i <= 12; i++)
        {
            logger.Information("entry-{Index}", i);
        }

        string[] logs = SerilogQueSink.GetLogs();
        Assert.Equal(10, logs.Length);
        Assert.DoesNotContain(logs, entry => entry.EndsWith("entry-1", StringComparison.Ordinal));
        Assert.DoesNotContain(logs, entry => entry.EndsWith("entry-2", StringComparison.Ordinal));
        Assert.Contains(logs, entry => entry.EndsWith("entry-12", StringComparison.Ordinal));
    }
}
