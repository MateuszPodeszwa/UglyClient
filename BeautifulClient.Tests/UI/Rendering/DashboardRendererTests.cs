using BeautifulClient.Tests.UI.TestDoubles;
using BeautifulClient.UI.Components;
using BeautifulClient.UI.Models;
using BeautifulClient.UI.Rendering;
using Serilog;
using Spectre.Console;

namespace BeautifulClient.Tests.UI.Rendering;

public class DashboardRendererTests
{
    [Fact]
    public void RenderDashboard_DisplaysDeviceSectionsAndFeedback()
    {
        var subject = new DashboardRenderer();
        var model = new DashboardModel
        {
            Sensors =
            [
                TestObjectFactory.Sensor(1, 8.0),
                TestObjectFactory.Sensor(2, 22.0),
                TestObjectFactory.Sensor(3, 40.0)
            ],
            Heaters =
            [
                TestObjectFactory.Heater(1, 0),
                TestObjectFactory.Heater(2, 2),
                TestObjectFactory.Heater(3, 5)
            ],
            Fans =
            [
                TestObjectFactory.Fan(1, false),
                TestObjectFactory.Fan(2, true),
                TestObjectFactory.Fan(3, true)
            ],
            RefreshedAt = new DateTimeOffset(2026, 4, 6, 10, 30, 0, TimeSpan.Zero),
            LastFeedback = "Preset applied.",
            IsFeedbackError = false
        };

        AnsiConsole.Record();
        subject.RenderDashboard(model);
        string output = AnsiConsole.ExportText();

        Assert.Contains("ENVIRONMENT CONTROL DASHBOARD", output);
        Assert.Contains("Sensors", output);
        Assert.Contains("Heaters", output);
        Assert.Contains("Fans", output);
        Assert.Contains("COLD", output);
        Assert.Contains("NORM", output);
        Assert.Contains("HOT!", output);
        Assert.Contains("Preset applied.", output);
        Assert.Contains("[Ctrl+F]", output);
    }

    [Fact]
    public void RenderDashboard_ShowsErrorMarkers_WhenFetchesAreMissing()
    {
        var subject = new DashboardRenderer();
        var model = new DashboardModel
        {
            Sensors = [TestObjectFactory.Sensor(1, 20)],
            Heaters = [TestObjectFactory.Heater(2, 2)],
            Fans = [TestObjectFactory.Fan(3, true)],
            ExpectedDeviceCount = 3
        };

        AnsiConsole.Record();
        subject.RenderDashboard(model);
        string output = AnsiConsole.ExportText();

        Assert.Contains("ERR", output);
        Assert.Contains("N/A", output);
    }

    [Fact]
    public void RenderHelp_DisplaysShortcutReferenceAndPresets()
    {
        var subject = new DashboardRenderer();

        AnsiConsole.Record();
        subject.RenderHelp();
        string output = AnsiConsole.ExportText();

        Assert.Contains("Keyboard Shortcuts", output);
        Assert.Contains("Ctrl+F", output);
        Assert.Contains("Ctrl+H", output);
        Assert.Contains("preset warm|cool|balanced|off", output);
    }

    [Fact]
    public void RenderLogs_ShowsEmptyState_WhenNoLogsAreAvailable()
    {
        SerilogQueSink.ClearLogs();
        var subject = new DashboardRenderer();

        AnsiConsole.Record();
        subject.RenderLogs();
        string output = AnsiConsole.ExportText();

        Assert.Contains("No log entries captured yet.", output);
    }

    [Fact]
    public void RenderLogs_ShowsQueuedEntries_WhenLogsExist()
    {
        SerilogQueSink.ClearLogs();
        using var logger = new LoggerConfiguration()
            .WriteTo.Sink(new SerilogQueSink())
            .CreateLogger();
        logger.Information("dashboard-log-entry");

        var subject = new DashboardRenderer();
        AnsiConsole.Record();
        subject.RenderLogs();
        string output = AnsiConsole.ExportText();

        Assert.Contains("Activity Logs", output);
        Assert.Contains("dashboard-log-entry", output);
    }
}
